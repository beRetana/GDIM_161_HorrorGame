using UnityEngine;
using System.Collections;
using System;
using Mirror;


namespace Interactions
{
    public class Torch : NetworkPickableItem, IFireable
    {
        private const int SECONDS_PER_MINUTE = 60;

        [Header("Gameplay Stuff")]
        [SerializeField, Range(1f, 20f)] float APROX_WOOD_LIFE_MINUTES = 8f;
        [SerializeField, Range(0f, 1f)] float maxTickVariance_WoodBurn;
        [SerializeField, Range(1f, 1.2f)] float tickSpeedMultiplier_ByFloor;
        [Tooltip("Pyrolysis is the process of thermal decomposition of materials at elevated temperatures, often in an inert atmosphere without access to oxygen.")]
        [SerializeField, Range(1, 100)] int pyrolysisIncrements;
         
        [Header("Model Stuff")]
        [SerializeField] Transform torchAnchor;
        [SerializeField] Transform torchWood;
        [SerializeField] private Transform m_CartoonFireTransform;
        [SerializeField] FireCollision torchFireCollider;

        [Header("Fire Stuff")]
        [SerializeField] ParticleSystem flameVFX;
        [SerializeField] Transform flameBase;
        [SerializeField] Transform flameRed;
        [SerializeField] Transform flameOrange;
        [SerializeField] Transform flameYellow;
        [Space(5)]
        [SerializeField, Tooltip("Exponential Decay, for when the fire burns out")] float flameDecayRate = -1.5f;
        [SerializeField, Tooltip("Exponential Decay, for when the fire burns out")] float lightDecayRate = -.5f;
        [SerializeField] float flameBurnOutTime;
        [Space(5)]
        [SerializeField, Tooltip("Exponential Growth, for when the fire is lit")] float flameGrowRate = 1.5f;
        [SerializeField, Tooltip("(1)/(1+n) is starting size for flame when lit. larger n = smaller start")
            , Range(1.1f, 10f)] float flameGrowCurveB = 5f;

        [SerializeField] Light torchLight;
        [SerializeField] private float m_StartTemp;
        [SerializeField] private float m_EndTemp;


        [SerializeField] LayerMask groundLayers;

        public float BurnTimer { get; private set; }
        private float burnVelocity = 1f;
        private float burnAcelleration = 0f;

        private float pyrolysisTimer;

        private float maxTorchWoodScale;
        private float torchWoodScale;
        private float minTorchWoodScale;

        private float maxFireLocalYPos;

        private float minFlameSize;
        private float flameSize;
        private float maxLightIntensity;
        private float lightIntensity = -1f;

        private bool isDropping = false;
        private const float SMOTHER_RADIUS = 0.5f;
        private bool isLit;
        public bool IsLit() { return isLit; }

        protected override void Start()
        {
            base.Start();
            BurnTimer = SECONDS_PER_MINUTE * APROX_WOOD_LIFE_MINUTES;
            pyrolysisTimer = BurnTimer / pyrolysisIncrements;
            isLit = false;
            maxTorchWoodScale = torchWood.localScale.y;
            maxFireLocalYPos = flameBase.localPosition.y;
            minFlameSize = 0.2f;
            maxLightIntensity = torchLight.intensity;

            ToggleFlame(false);
        }

        protected void Update()
        {
            if (!isLit) return;
            UpdateFlameOrientation();
            SmotherCheck();
        }

        protected void FixedUpdate()
        {
            //Debug.Log(BurnTimer);
            //Debug.Log(pyrolysisTimer);
            if (!isLit) return;
            UpdateTimers();
            Burn();
            UpdatePyrolysis();
            LightFlame();
        }
        public override void UseItem(int playerId, InputData context)
        {
            Debug.Log("Using torch");
            PlayerManager.Instance.GetPlayer(playerId).GetComponent<PlayerAnimator>().RaiseHand();
        }


        private void SmotherCheck()
        {
            if (!isLit || !isDropping) return;
            if (GroundCheck()) SmotherFlame();
        }

        private bool GroundCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            return Physics.CheckSphere(spherePosition, SMOTHER_RADIUS, groundLayers, QueryTriggerInteraction.Ignore);
        }

        #region pyrolysis

        private void UpdatePyrolysis()
        {
            if (pyrolysisTimer <= 0)
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.TorchFlicker, this.transform.position);
                WoodPyrolysis();
                pyrolysisTimer = APROX_WOOD_LIFE_MINUTES * SECONDS_PER_MINUTE / pyrolysisIncrements;
            }
        }

        private void WoodPyrolysis()
        {
            float burntRatio = ((BurnTimer / SECONDS_PER_MINUTE) / APROX_WOOD_LIFE_MINUTES);
            torchWoodScale = maxTorchWoodScale * burntRatio;
            torchWood.localScale = new Vector3(torchWood.localScale.x, torchWoodScale, torchWood.localScale.z);

            float flameBaseNewLocalPosY = maxFireLocalYPos * burntRatio;
            flameBase.localPosition = new Vector3(flameBase.localPosition.x, flameBaseNewLocalPosY, flameBase.localPosition.z);

            ScaleFlameScale(Mathf.Max(burntRatio, minFlameSize));
        }

        private void UpdateTimers()
        {
            burnVelocity = CalcSmoothRandom(burnVelocity);
            //Debug.Log($"burnAcceleration = {burnAcelleration}");
            //Debug.Log($"burnVelocity = {burnVelocity}");

            pyrolysisTimer -= burnVelocity * Time.fixedDeltaTime;
            BurnTimer -= burnVelocity * Time.fixedDeltaTime; // * tickSpeedMultiplier_ByFloor
        }

        private void Burn()
        {
            if (BurnTimer <= 0) BlowOutFlame();
        }

        private float CalcSmoothRandom(float x)
        {
            burnAcelleration = UnityEngine.Random.Range(0f, 1f) * .1f - .05f;
            x += burnAcelleration;
            x = Mathf.Clamp(x, 1-maxTickVariance_WoodBurn, 1+maxTickVariance_WoodBurn);
            return x;
        }

        #endregion pyrolysis

        #region flame_core
        private void UpdateFlameOrientation()
        {
            flameBase.eulerAngles = Vector3.up;
        }

        private void ToggleFlame(bool setOn)
        {
            isLit = setOn;
            flameVFX.gameObject.SetActive(setOn);
        }
        public void BlowOutFlame() // via wind
        {
            if (!isLit) return;
            StartCoroutine(BurnOutFire(flameBurnOutTime, flameDecayRate, lightDecayRate));
        }
        public void LightFlame()
        {
            if (isServer) RpcLightingObject();
        }

        [ClientRpc]
        private void RpcLightingObject()
        {
            if (isLit) return;
            FlameFullExtinguish();
            isLit = true;
            StartCoroutine(IgniteFire(flameGrowRate, flameGrowCurveB));
        }

        public void SmotherFlame()
        {
            if (!isLit) return;
            flameSize = 0f;
            lightIntensity = 0f;
            ScaleFlameScale(0);
            SetVisualLightIntensity(0);
            isDropping = false;
            ToggleFlame(false);
        }


        #endregion flame_core

        #region flame_helpers
        private void ScaleFlameScale(float scalar)
        {
            Vector3 newFlameScale = Vector3.one * scalar;

            flameRed.localScale = newFlameScale;
            flameOrange.localScale = newFlameScale;
            flameYellow.localScale = newFlameScale;
            torchLight.colorTemperature = m_EndTemp + (m_StartTemp - m_EndTemp) * scalar; // Lerping starting temperature to ending temperature.

        }
        private void SetVisualLightIntensity(float intensePercent)
        {
            lightIntensity = Mathf.Min(maxLightIntensity * intensePercent * intensePercent, maxLightIntensity);
            torchLight.intensity = lightIntensity;
        }
        private void FlameFullSize()
        {
            isLit = true;
            torchWoodScale = maxTorchWoodScale;
            flameSize = minFlameSize;
            torchLight.intensity = maxLightIntensity;
            ScaleFlameScale(1f);
        }
        private void FlameFullExtinguish()
        {
            isLit = false;
            torchWoodScale = 0f;
            flameSize = 0f;
            torchLight.intensity = 0f;
            ScaleFlameScale(0f);
            ToggleFlame(false);
        }

        private void NetworkDestroyTorch()
        {
            Debug.Log("Ending Torch");
            HandInventory inventory = PlayerManager.Instance.GetPlayer(OwnerPlayerID).GetComponent<HandInventory>();
            
            if (inventory.PeekAtDominant() != this)
            {
                inventory.SwapAction();
            }
            
            inventory.DropAction();
            transform.root.gameObject.SetActive(false);

            if (isServer) RpcTurnOff();
            else CmdTurnOff();
        }

        [Command]
        private void CmdTurnOff()
        {
            RpcTurnOff();
        }

        [ClientRpc]
        private void RpcTurnOff()
        {
            transform.root.gameObject.SetActive(false);
            _interactableItem.SetInteractive(false);
            gameObject.SetActive(false);
        }

        #endregion flame_helpers

        private void KillTorch()
        {
            FlameFullExtinguish();
            NetworkDestroyTorch();
            AudioManager.instance.PlayOneShot(FMODEvents.instance.TorchExtinguish, this.transform.position);
        }


        #region flame_animations
        private IEnumerator BurnOutFire(float burnOutTime, float flameExpDecayRate, float lightExpDecayRate)
        {
            //exponential decay
            for (float delta = 0f;  delta < burnOutTime; delta += Time.deltaTime)
            {
                flameSize = minFlameSize * Mathf.Exp(flameExpDecayRate * delta);
                lightIntensity = maxLightIntensity * Mathf.Exp(lightExpDecayRate * delta);

                ScaleFlameScale(flameSize);

                SetVisualLightIntensity(lightIntensity);

                yield return null;
            }
            KillTorch();
        }


        /// <summary>
        /// 1 / (1 + b * e^(-kx))
        /// </summary>
        private IEnumerator IgniteFire(float flameGrowthRate, float flameGrowthB)
        {
            ToggleFlame(true);

            // to ensure full light intensity before the for loop ends
            float lightIntensityM = (-1f) * flameGrowthRate / (Mathf.Log(.1f / flameGrowthB));

            for(float delta = 0f; flameSize < 1f && delta < 5f; delta += Time.deltaTime)
            {
                flameSize = 1.05f / (1f + flameGrowthB * Mathf.Exp(-1f * flameGrowthRate * delta));
                ScaleFlameScale(flameSize);
                SetVisualLightIntensity(delta * lightIntensityM);

                yield return null;
            }
            flameSize = 1f;
            FlameFullSize();
        }
        #endregion flame_animations

    }
}

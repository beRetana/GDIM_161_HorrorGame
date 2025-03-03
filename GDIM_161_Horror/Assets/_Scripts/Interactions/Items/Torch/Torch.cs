using UnityEngine;
using System.Collections;
using Codice.Client.Common.GameUI;
using System;
using Player;

namespace Interactions
{
    public class Torch : PickableItem
    {
        private const int SECONDS_PER_MINUTE = 60;

        [Header("Gameplay Stuff")]
        [SerializeField, Range(1f, 20f)] float approxWoodLife_Minutes;
        [SerializeField, Range(0f, 1f)] float maxTickVariance_WoodBurn;
        [SerializeField, Range(1f, 1.2f)] float tickSpeedMultiplier_ByFloor;
        [Tooltip("Pyrolysis is the process of thermal decomposition of materials at elevated temperatures, often in an inert atmosphere without access to oxygen.")]
        [SerializeField, Range(1, 100)] int pyrolysisIncrements;
         
        [Header("Model Stuff")]
        [SerializeField] Transform torchAnchor;
        [SerializeField] Transform torchWood;
        [SerializeField] SphereCollider torchFireCollider;

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

        public float BurnTimer { get; private set; }
        private float burnVelocity = 1f;
        private float burnAcelleration = 0f;

        private float pyrolysisTimer;

        private float maxTorchWoodScale;
        private float torchWoodScale;
        private float minTorchWoodScale;

        private float maxFireLocalYPos;

        private float maxFlameSize;
        private float flameSize;
        private float maxLightIntensity;
        private float lightIntensity = 1f;

        public bool Lit { get; private set; }

        protected override void Start()
        {
            base.Start();
            BurnTimer = SECONDS_PER_MINUTE * approxWoodLife_Minutes;
            pyrolysisTimer = BurnTimer / pyrolysisIncrements;
            Lit = false;
            maxTorchWoodScale = torchWood.localScale.y;
            maxFireLocalYPos = flameBase.localPosition.y;
            maxFlameSize = flameRed.localScale.y;
            maxLightIntensity = torchLight.intensity;
            torchFireCollider.enabled = false;

            ToggleFlame(false);
            LightFlame();
        }

        private void Update()
        {
            if (!Lit) return;
            UpdateFlameOrientation(); 
        }

        private void FixedUpdate()
        {
            //Debug.Log(BurnTimer);
            //Debug.Log(pyrolysisTimer);
            if (!Lit) return;
            UpdateTimers();
            Burn();
            UpdatePyrolysis();
            LightFlame();
        }
        public override void UseItem(int playerId)
        {
            // extend torch in arm
            // enable torch collider
            // check if torch colliding with fire
            // OR
            // check if handinventory raycast hitting campfire or player

            // ?light torch
            Debug.Log("Using torch");
            PlayerManager.Instance.GetPlayer(playerId).GetComponent<HandInventory>().GetArms().ToggleHandMoveOutOrIn(null);
        }

        private void UpdatePyrolysis()
        {
            if (pyrolysisTimer <= 0)
            {
                WoodPyrolysis();
                pyrolysisTimer = approxWoodLife_Minutes * SECONDS_PER_MINUTE / pyrolysisIncrements;
            }
        }

        private void WoodPyrolysis()
        {
            torchWoodScale = maxTorchWoodScale * BurnTimer / SECONDS_PER_MINUTE / approxWoodLife_Minutes;
            torchWood.localScale = new Vector3(torchWood.localScale.x, torchWoodScale, torchWood.localScale.z);

            float flameBaseNewLocalPosY = maxFireLocalYPos * BurnTimer / SECONDS_PER_MINUTE / approxWoodLife_Minutes;
            flameBase.localPosition = new Vector3(flameBase.localPosition.x, flameBaseNewLocalPosY, flameBase.localPosition.z);
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

        private void UpdateFlameOrientation()
        {
            flameBase.eulerAngles = Vector3.up;
        }

        private void ToggleFlame(bool setOn)
        {
            Lit = setOn;
            flameVFX.gameObject.SetActive(setOn);
        }

        private void FlameFullSize()
        {
            Lit = true;
            torchWoodScale = maxTorchWoodScale;
            flameSize = maxFlameSize;
            torchLight.intensity = maxLightIntensity;
            ScaleFlameScale(1f);
        }

        private void FlameFullExtinguish()
        {
            Lit = false;
            torchWoodScale = 0f;
            flameSize = 0f;
            torchLight.intensity = 0f;
            ScaleFlameScale(0f);
        }

        public void BlowOutFlame() // via wind
        {
            if (!Lit) return;
            StartCoroutine(BurnOutFire(flameBurnOutTime, flameDecayRate, lightDecayRate));
        }

        private IEnumerator BurnOutFire(float burnOutTime, float flameExpDecayRate, float lightExpDecayRate)
        {
            //exponential decay
            for (float delta = 0f;  delta < burnOutTime; delta += Time.deltaTime)
            {
                flameSize = maxFlameSize * Mathf.Exp(flameExpDecayRate * delta);
                lightIntensity = maxLightIntensity * Mathf.Exp(lightExpDecayRate * delta);

                ScaleFlameScale(flameSize / maxFlameSize);

                SetVisualLightIntensity(lightIntensity);

                yield return null;
            }
            FlameFullExtinguish();
        }

        private void ScaleFlameScale(float scalar)
        {
            Vector3 newFlameScale = Vector3.one * scalar;

            flameRed.localScale = newFlameScale;
            flameOrange.localScale = newFlameScale;
            flameYellow.localScale = newFlameScale;
        }

        public void LightFlame()
        {
            if (Lit) return;
            FlameFullExtinguish();
            StartCoroutine(IgniteFire(flameGrowRate, flameGrowCurveB));
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

        private void SetVisualLightIntensity(float intensePercent)
        {
            lightIntensity = Mathf.Min(maxLightIntensity * intensePercent * intensePercent, maxLightIntensity);
            torchLight.intensity = lightIntensity;
        }

        //public void BurnOutFlame() // via end of wood
        //public void SmotherFlame() // via dropping

        //watch velocity (and air pressure) or burn out

    }
}

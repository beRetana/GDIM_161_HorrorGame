using UnityEngine;
using System.Collections;
using Codice.Client.Common.GameUI;
using System;

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
        [SerializeField] TorchModel model;
        [SerializeField] Transform torchAnchor;
        [SerializeField] Transform torchWood;

        [Header("Fire Stuff")]
        [SerializeField] ParticleSystem flameVFX;
        [SerializeField] Transform flameBase;
        [SerializeField] Transform flameRed;
        [SerializeField] Transform flameOrange;
        [SerializeField] Transform flameYellow;
        [SerializeField, Tooltip("Exponential Decay, for when the fire burns out")] float flameDecayRate = 1.5f;
        [SerializeField, Tooltip("Exponential Decay, for when the fire burns out")] float lightDecayRate = .5f;
        [SerializeField] float flameBurnOutTime;
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
            Lit = true;
            maxTorchWoodScale = torchWood.localScale.y;
            torchWoodScale = maxTorchWoodScale;
            maxFireLocalYPos = flameBase.localPosition.y;
            maxFlameSize = flameRed.localScale.y;
            flameSize = maxFlameSize;
            maxLightIntensity = torchLight.intensity;
        }

        private void Update()
        {
            if (!Lit) return;
            UpdateFlameOrientation(); 
        }

        private void FixedUpdate()
        {
            Debug.Log(BurnTimer);
            Debug.Log(pyrolysisTimer);
            if (!Lit) return;
            UpdateTimers();
            Burn();
            UpdatePyrolysis();
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
            Debug.Log($"burnAcceleration = {burnAcelleration}");
            Debug.Log($"burnVelocity = {burnVelocity}");

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

        public override void UseItem(int playerId)
        {
            //if used on campfire || player
            //  light torch
            //else
            //  point torch
        }

        private void ToggleFlame(bool setOn)
        {
            Lit = setOn;
            flameVFX.gameObject.SetActive(setOn);
        }

        public void BlowOutFlame() // via wind
        {
            if (!Lit) return;
            StartCoroutine(BurnOutFire(flameBurnOutTime));
        }

        private IEnumerator BurnOutFire(float burnOutTime)
        {
            //exponential decay
            for (float delta = 0f;  delta < burnOutTime; delta += Time.deltaTime)
            {
                flameSize = maxFlameSize * Mathf.Exp(-1 * flameDecayRate * delta);
                lightIntensity = maxLightIntensity * Mathf.Exp(-1 * lightDecayRate * delta);

                Vector3 newFlameScale = Vector3.one * (flameSize / maxFlameSize);

                flameRed.localScale = newFlameScale;
                flameOrange.localScale = newFlameScale;
                flameYellow.localScale = newFlameScale;

                torchLight.intensity = lightIntensity;

                yield return null;
            }
            ToggleFlame(false);
        }

        public void LightFlame()
        {
            if (Lit) return;
            ToggleFlame(true);
        }

        //public void BurnOutFlame() // via end of wood
        //public void SmotherFlame() // via dropping

        //watch velocity (and air pressure) or burn out

        //slowly burn down
    }
}

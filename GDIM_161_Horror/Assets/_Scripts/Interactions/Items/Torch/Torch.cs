using UnityEngine;
using System.Collections;
using Codice.Client.Common.GameUI;

namespace Interactions
{
    public class Torch : PickableItem
    {
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

        public float BurnTimer { get; private set; }
        private float burnVelocity = 1f;
        private float burnAcelleration = 0f;

        private float pyrolysisTimer;

        private float maxTorchWoodScale;
        private float torchWoodScale;
        private float minTorchWoodScale;
        public bool Lit { get; private set; }

        protected override void Start()
        {
            base.Start();
            BurnTimer = 60 * approxWoodLife_Minutes;
            pyrolysisTimer = BurnTimer / pyrolysisIncrements;
            Lit = true;
            maxTorchWoodScale = torchWood.localScale.y;
            torchWoodScale = maxTorchWoodScale;
        }

        private void Update()
        {
            if (!Lit) return;
            UpdateFlameOrientation(); 
        }

        private void FixedUpdate()
        {
            if (!Lit) return;
            Burn();
            UpdatePyrolysis();
        }

        private void UpdatePyrolysis()
        {
            pyrolysisTimer -= burnVelocity * Time.time;
            if (pyrolysisTimer <= 0)
            {
                WoodPyrolysis();
                pyrolysisTimer = BurnTimer / pyrolysisIncrements;
            }
        }

        private void WoodPyrolysis()
        {
            torchWoodScale = torchWoodScale * BurnTimer / 60 / approxWoodLife_Minutes;
            torchWood.localScale = new Vector3(torchWood.localScale.x, torchWoodScale, torchWood.localScale.y);
        }

        private void Burn()
        {
            if (BurnTimer <= 0) BlowOutFlame();
            BurnTimer -= burnVelocity * Time.time; // * tickSpeedMultiplier_ByFloor
            burnVelocity = CalcSmoothRandom(burnVelocity);

            Debug.Log($"burnAcceleration = {burnAcelleration}");
            Debug.Log($"burnVelocity = {burnVelocity}");
        }

        private float CalcSmoothRandom(float x)
        {
            burnAcelleration = Random.Range(0f, 1f) * .1f - .05f;
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

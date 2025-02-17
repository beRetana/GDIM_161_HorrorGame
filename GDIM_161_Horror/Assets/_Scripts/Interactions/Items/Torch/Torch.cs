using UnityEngine;
using System.Collections;

namespace Interactions
{
    public class Torch : PickableItem
    {
        [Header("Gameplay Stuff")]
        [SerializeField] [Range(1f, 20f)] float approxWoodLife_Minutes;
        [SerializeField] [Range(0f, 1f)] float maxTickVariance_WoodBurn;
        [SerializeField] [Range(1f, 1.2f)] float tickSpeedMultiplier_ByFloor;

        [Header("Model Stuff")]
        [SerializeField] TorchModel model;
        [SerializeField] Transform flameBase;

        public float BurnTimer { get; private set; }
        private float burnVelocity = 1f;
        private float burnAcelleration = 0f;
        public bool Lit { get; private set; }

        protected override void Start()
        {
            base.Start();
            BurnTimer = 60 * approxWoodLife_Minutes;
            Lit = true;
        }

        private void Update()
        {
            if(Lit) Burn();
            UpdateFlameOrientation();
        }

        private void Burn()
        {
            if (BurnTimer <= 0) BlowOutFlame();
            BurnTimer -= burnVelocity * Time.deltaTime; // * tickSpeedMultiplier_ByFloor
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

        private void TurnOnFlame(bool setOn)
        {
            model.TurnOnFlame(setOn);
        }

        public void BlowOutFlame() // via wind
        {
            if (!Lit) return;
            Lit = false;
        }

        //public void BurnOutFlame() // via end of wood
        //public void SmotherFlame() // via dropping

        //watch velocity (and air pressure) or burn out

        //slowly burn down
    }
}

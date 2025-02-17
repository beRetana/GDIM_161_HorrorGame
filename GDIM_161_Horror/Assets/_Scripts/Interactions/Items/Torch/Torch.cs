using UnityEngine;

namespace Interactions
{
    public class Torch : PickableItem
    {
        [SerializeField] TorchModel model;
        public bool Lit { get; private set; }

        protected override void Start()
        {
            base.Start();
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

        public void BurnOutFlame() // via end of wood
        {
            if (!Lit) return;
            Lit = false;
        }

        public void SmotherFlame() // via dropping
        {
            if (!Lit) return;
            Lit = false;
        }

        //watch velocity (and air pressure) or burn out

        //slowly burn down
    }
}

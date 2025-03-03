using UnityEngine;

namespace Interactions
{
    public class FireCollision : MonoBehaviour
    {
        [SerializeField] private Torch torch;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (this.IsLit()) return; //check if this is already lit

            if (!col.CompareTag("Fir")) return; //check if other is fire

            FireCollision colFire = col.gameObject.GetComponent<FireCollision>();

            if (!colFire.IsLit()) return; //check if other fire is lit

            torch.LightFlame();
        }


        public bool IsLit()
        {
            return torch.Lit;
        }
    }
}

using UnityEngine;
using Mirror;

namespace Interactions
{
    public class FireCollision : MonoBehaviour
    {
        [SerializeField, Tooltip("Torch / Hearth")] 
        private GameObject maybeFireable;
        private IFireable fireableObject;

        private const string FIRE_TAG = "Fire";

        private void Start()
        {
            fireableObject = maybeFireable.GetComponent<IFireable>();
            if (fireableObject == null) Destroy(this);
        }

        private void OnTriggerEnter(Collider col)
        {
            Debug.Log("Entered Collision");
            if (this.IsLit()) return; //check if this is already lit

            if (!col.CompareTag(FIRE_TAG)) return; //check if other is fire

            FireCollision colFire = col.gameObject.GetComponent<FireCollision>();
            Debug.Log($"COLLIDED FIRE {colFire.gameObject.name}, {this}");

            if (!colFire.IsLit()) return; //check if other fire is lit

            fireableObject.LightFlame();
        }


        public bool IsLit()
        {
            return fireableObject.IsLit();
        }
    }
}

using UnityEngine;

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
            fireableObject = maybeFireable.gameObject.GetComponent<IFireable>();
            if (fireableObject == null) Destroy(this);

        }

        private void OnTriggerEnter(Collider col)
        {
            if (!col.CompareTag(FIRE_TAG)) return; //check if other is fire

            FireCollision colFire = col.gameObject.GetComponent<FireCollision>();
            Debug.Log($"COLLIDED FIRE {colFire.gameObject.name}, {this}");

            if (!colFire.IsLit()) return; //check if other fire is lit

            fireableObject.LightFlame();
        }


        private bool IsLit()
        {
            return fireableObject.IsLit();
        }
    }
}

using UnityEngine;
using Mirror;

namespace Interactions
{
    public class FireCollision : MonoBehaviour
    {
        [SerializeField, Tooltip("Torch / Hearth")]
        private Transform maybeFireable;
        private IFireable fireableObject;

        private const string FIRE_TAG = "Fire";
        private const string GROUND_TAG = "Ground";

        private bool isSmotherable;
        private Torch torch; //only has value if isSmotherable

        private void Start()
        {
            fireableObject = maybeFireable.GetComponent<IFireable>();
            if (fireableObject == null) Destroy(this);

            torch = maybeFireable.GetComponent<Torch>();
            isSmotherable = torch != null;
            Debug.Log($"{fireableObject}, {maybeFireable.name}, {isSmotherable} smotherable");
        }

        private void OnTriggerEnter(Collider col)
        {
            Debug.Log("Entered Collision");
            if (isSmotherable && this.IsLit())
            {
                // smother check
                if (!col.CompareTag(GROUND_TAG)) return;

                torch.SmotherFlame();
                Debug.Log("sending smother");
                return; //check if this is already lit
            }
            else
            {
                // share flame check

                if (!col.CompareTag(FIRE_TAG)) return; //check if other is fire

                FireCollision colFire = col.gameObject.GetComponent<FireCollision>();
                Debug.Log($"COLLIDED FIRE {colFire.gameObject.name}, {this}");

                if (!colFire.IsLit()) return; //check if other fire is lit

                fireableObject.LightFlame();

            }
        }

        private bool IsLit()
        {
            return fireableObject.IsLit();
        }
    }
}

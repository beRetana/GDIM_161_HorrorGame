using UnityEngine;
using Mirror;
using OtherUtils;

namespace Interactions
{
    public class FireCollision : MonoBehaviour, IDebugger
    {
        [SerializeField, Tooltip("Torch / Hearth")] private GameObject maybeFireable;
        private bool _debugger;
        private IFireable fireableObject;

        private void Start()
        {
            fireableObject = maybeFireable.gameObject.GetComponent<IFireable>();
            if (fireableObject == null) Destroy(this);
        }

        private void OnTriggerEnter(Collider col)
        {
            if (!col.gameObject.TryGetComponent<FireCollision>(out FireCollision colFire)) return;

            Debugger($"COLLIDED FIRE {colFire.gameObject.name}, {this}");

            if (!colFire.IsLit()) return; //check if other fire is lit

            this.fireableObject.LightFlame();
        }

        private bool IsLit()
        {
            return fireableObject.IsLit();
        }

        public void Debugger(object log)
        {
            if (_debugger) Debug.Log(log);
        }

        public void SetDebugActive(bool active)
        {
            _debugger = active;
        }
    }
}

using UnityEngine;
using Mirror;
using OtherUtils;

namespace Interactions
{
    public class FireCollision : MonoBehaviour, IDebugger
    {
        [SerializeField, Tooltip("Torch / Hearth")] private GameObject maybeFireable;
        protected bool _debugger;
        protected IFireable fireableObject;

        protected virtual void Start()
        {
            fireableObject = maybeFireable.gameObject.GetComponent<IFireable>();
            if (fireableObject == null) Destroy(this);
        }

        protected virtual void OnTriggerEnter(Collider col)
        {
            FireCollision colFire;

            if (!col.gameObject.TryGetComponent<FireCollision>(out colFire)) return;
            if (fireableObject == null) return;
            Debugger($"COLLIDED FIRE {colFire.gameObject.name}, {this}");

            if (!colFire.IsLit()) return; //check if other fire is lit

            this.fireableObject.LightFlame();
        }

        public virtual bool IsLit()
        {
            if (fireableObject == null) return false;
            return fireableObject.IsLit();
        }

        public virtual void Debugger(object log)
        {
            if (_debugger) Debug.Log(log);
        }

        public virtual void SetDebugActive(bool active)
        {
            _debugger = active;
        }
    }
}

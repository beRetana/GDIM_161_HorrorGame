using Mirror;
using UnityEngine;

namespace AI
{
    public abstract class NetworkAIActor : NetworkBehaviour
    {
        public abstract void TransitionOfBehaviors();

        public abstract void AbortBehaviors();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_FSM{

    /// <summary>
    /// Base class for any AI actor that will be created to ensure individual 
    /// behavior scripts have what they need to function.
    /// </summary>
    public abstract class AIActor : MonoBehaviour{

        public abstract void TransitionOfBehaviors();

        public abstract void AbortBehaviors();

    }
}

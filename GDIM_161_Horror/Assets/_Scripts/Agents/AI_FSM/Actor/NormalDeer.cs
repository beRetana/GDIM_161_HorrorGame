using UnityEngine;
using AI_FSM;
using System.Collections;

namespace AI
{
    public class NormalDeer : NetworkAIActor
    {
        private Wander _wander;

        private void Start()
        {
            _wander = GetComponent<Wander>();
            StartCoroutine(StartSequence());
        }

        private IEnumerator StartSequence()
        {
            yield return new WaitForSecondsRealtime(1f);
            EnableWander();
        }

        private void EnableWander()
        {
            _wander.Enable();
        }

        public override void AbortBehaviors()
        {
            _wander.Disable();
        }

        public override void TransitionOfBehaviors()
        {
            // there are no transitions here.
        }
    }
}

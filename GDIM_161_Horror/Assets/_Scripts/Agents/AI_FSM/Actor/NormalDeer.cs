using UnityEngine;
using AI_FSM;
using System.Collections;
using UnityEngine.AI;

namespace AI
{
    public class NormalDeer : NetworkAIActor
    {
        private Wander _wander;
        private DeerAnimator _animator;
        private NavMeshAgent _controller;

        private void Start()
        {
            _wander = GetComponent<Wander>();
            _animator = GetComponent<DeerAnimator>();
            _controller = GetComponent<NavMeshAgent>();
            StartCoroutine(StartSequence());
        }

        void Update()
        {
            _animator.SetSpeed(Mathf.Clamp01(_controller.velocity.magnitude/_controller.speed));
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

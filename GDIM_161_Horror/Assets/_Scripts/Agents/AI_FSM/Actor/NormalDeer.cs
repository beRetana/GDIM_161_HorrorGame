using UnityEngine;
using FMODUnity;
using AI_FSM;
using System.Collections;
using UnityEngine.AI;
using FMOD.Studio;

namespace AI
{
    public class NormalDeer : NetworkAIActor
    {
        private Wander _wander;
        private Rigidbody _rigidbody;
        private DeerAnimator _animator;
        private NavMeshAgent _controller;
        private void Start()
        {
            if (!isServer) return;

            _controller = GetComponent<NavMeshAgent>();
            _wander = GetComponent<Wander>();
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<DeerAnimator>();
        }

        void Update()
        {
            if (!isServer) return;
            // Get normalized speed (0 to 1)
            float currentSpeed = Mathf.Clamp01(_controller.velocity.magnitude / _controller.speed);

            // Update animation speed
            _animator.SetSpeed(currentSpeed);

            // Handle footstep sounds based on speed changes
           // HandleFootstepSounds(currentSpeed);

            // Trigger jump animation if deer is moving upward
            if (_rigidbody.linearVelocity.y > 0.001f)
            {
                _animator.OnJump();
            }
        }

        private void OnEnable()
        {
            EnableWander();
        }

        private void OnDisable()
        {
            AbortBehaviors();
        }

        private IEnumerator StartSequence()
        {
            yield return new WaitForSecondsRealtime(1f);
            if (isServer) EnableWander();
        }

        private void EnableWander()
        {
            if (!isServer) return;
            _wander.StartBehaviour();
        }

        public override void AbortBehaviors()
        {
            if (!isServer) return;
            _wander.StopBehaviour();
        }

        public override void TransitionOfBehaviors()
        {
            // No transitions
        }
    }
}

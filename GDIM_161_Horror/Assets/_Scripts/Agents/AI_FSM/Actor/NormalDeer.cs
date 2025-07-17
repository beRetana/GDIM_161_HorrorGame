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

        private bool m_WanderEnabled;

        private void Start()
        {
            if (!isServer) return;

            _controller = GetComponent<NavMeshAgent>();
            _wander = GetComponent<Wander>();
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<DeerAnimator>();
            EnableWander();
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

        public void OnEnable()
        {
            EnableWander();
        }

        public void OnDisable()
        {
            AbortBehaviors();
        }

        private void EnableWander()
        {
            if (!isServer || m_WanderEnabled || _wander == null) return;
            _wander.StartBehaviour();
            m_WanderEnabled = true;
        }

        public override void AbortBehaviors()
        {
            if (!isServer) return;
            _wander.StopBehaviour();
            m_WanderEnabled = false;
        }

        public override void TransitionOfBehaviors()
        {
            // No transitions
        }
    }
}

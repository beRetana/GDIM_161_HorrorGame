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

        
        
        //[SerializeField] private EventReference _deerWalkFootstep;
        //[SerializeField] private EventReference _deerRunFootstep;
        private bool isWalking = false;
        private bool isRunning = false;

       // private const float WalkThreshold = 0.1f; // Adjust for when to start playing walking sound
       // private const float RunThreshold = 0.5f;  // Adjust for when to start playing running sound

        private void Start()
        {
            _wander = GetComponent<Wander>();
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<DeerAnimator>();
            _controller = GetComponent<NavMeshAgent>();
            StartCoroutine(StartSequence());
        }

        void Update()
        {
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

        // private void HandleFootstepSounds(float currentSpeed)
        // {
        //     // Handle walking sound
        //     if (!isWalking && currentSpeed >= WalkThreshold && currentSpeed < RunThreshold)
        //     {
        //         PlayDeerwalkFootstep();
        //         isWalking = true;
        //         isRunning = false;
        //     }
        //     else if (isWalking && (currentSpeed < WalkThreshold || currentSpeed >= RunThreshold))
        //     {
               
                
        //         isWalking = false;
        //     }

        //     // Handle running sound
        //     if (!isRunning && currentSpeed >= RunThreshold)
        //     {
        //         // Start running sound
                
        //         PlayDeerrunFootstep();
        //         isRunning = true;
        //     }
        //     else if (isRunning && currentSpeed < RunThreshold)
        //     {
                
                
        //         isRunning = false;
        //     }
        // }
        
        // public void PlayDeerwalkFootstep()
        // {
        //     RuntimeManager.PlayOneShot(_deerWalkFootstep, transform.position);
        // }

        // public void PlayDeerrunFootstep()
        // {
        //     RuntimeManager.PlayOneShot(_deerRunFootstep, transform.position);
        // }

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
            // No transitions
        }

        

       
    }
}

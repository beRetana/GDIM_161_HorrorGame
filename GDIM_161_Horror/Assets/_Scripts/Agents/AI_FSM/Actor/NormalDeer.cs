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

        
        private EventInstance deerWalkEventInstance;
        private EventInstance deerRunEventInstance;
        private bool isWalking = false;
        private bool isRunning = false;

        private const float WalkThreshold = 0.3f; // Adjust for when to start playing walking sound
        private const float RunThreshold = 0.6f;  // Adjust for when to start playing running sound

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
            HandleFootstepSounds(currentSpeed);

            // Trigger jump animation if deer is moving upward
            if (_rigidbody.linearVelocity.y > 0.001f)
            {
                _animator.OnJump();
            }
        }

        private void HandleFootstepSounds(float currentSpeed)
        {
            // Handle walking sound
            if (!isWalking && currentSpeed >= WalkThreshold && currentSpeed < RunThreshold)
            {
                // Start walking sound
                deerWalkEventInstance = RuntimeManager.CreateInstance(FMODEvents.instance.deerWalk);
                RuntimeManager.AttachInstanceToGameObject(deerWalkEventInstance, transform);
                deerWalkEventInstance.start();
                isWalking = true;
                isRunning = false;
            }
            else if (isWalking && (currentSpeed < WalkThreshold || currentSpeed >= RunThreshold))
            {
                // Stop walking sound if speed drops below walking threshold or increases to running speed
                deerWalkEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                isWalking = false;
            }

            // Handle running sound
            if (!isRunning && currentSpeed >= RunThreshold)
            {
                // Start running sound
                deerRunEventInstance = RuntimeManager.CreateInstance(FMODEvents.instance.deerRun);
                RuntimeManager.AttachInstanceToGameObject(deerRunEventInstance, transform);
                deerRunEventInstance.start();
                isRunning = true;
            }
            else if (isRunning && currentSpeed < RunThreshold)
            {
                // Stop running sound if speed decreases below running threshold
                deerRunEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                isRunning = false;
            }
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
            // No transitions
        }

        private void OnDestroy()
        {
            // Clean up and stop the footstep sounds when the deer is destroyed
            if (deerWalkEventInstance.isValid())
            {
                deerWalkEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                deerWalkEventInstance.release();
            }

            if (deerRunEventInstance.isValid())
            {
                deerRunEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                deerRunEventInstance.release();
            }
        }
    }
}

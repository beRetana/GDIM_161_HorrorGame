using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_FSM{

    public class Chase : TaskBase{

        [Header("Chase Settings")]
        [SerializeField] private float _chaseSpeed = 5f;
        [SerializeField] private float _catchRange = 3f;

        private GameObject _target;
        private Vector3 _lastKnownLocation;

        private void Update()
        {
            if (_target == null) return;

            Chasing();
        }

        private void Chasing()
        {
            //If the target is out of range, move to the target.
            if ((_target.transform.position - transform.position).magnitude > _catchRange)
            {
                _aiController.MoveTo(_target.transform.position);
            }
            //Else catch the player.
            else
            {
                CatchPlayer();
            }
        }

        /// <summary>
        /// Abort the current task (moving to location) and invoke the 
        /// task completed event as successful.
        /// </summary>
        private void CatchPlayer()
        {
            _aiController.AbortTask();
            _taskCompleted.Invoke(succeful: true);
        }

        /// <summary>
        /// Set the target the AI will chase.
        /// </summary>
        /// <param name="target">The target of the AI.</param>
        public void SetTarget(GameObject target)
        {
            _target = target;
        }

        /// <summary>
        /// Set the target as lost and move to the last known location.
        /// </summary>
        public void SetTargetLost()
        {
            _lastKnownLocation = _target.transform.position;
            _target = null;
            _aiController.AbortTask();
            _aiController.MoveTo(_lastKnownLocation);
            _aiController.onTaskCompleted += OnCheckedLastLocation;
        }

        /// <summary>
        /// Enabling Chase task will increase the speed of the AI to the chase speed.
        /// </summary>
        public override void Enable()
        {
            _aiController.ChangeSpeed(_chaseSpeed);
        }

        /// <summary>
        /// Disabling the Chase task will reset the speed of the AI to the default speed.
        /// </summary>
        public override void Disable()
        {
            _aiController.onTaskCompleted -= OnCheckedLastLocation;
            _aiController.DefaultSpeed();
        }

        /// <summary>
        /// When the AI reaches the last known location, the task is completed
        /// and invokes the event of task completed as failed.
        /// </summary>
        private void OnCheckedLastLocation()
        {
            _taskCompleted.Invoke(succeful: false);
            Disable();
        }
    }
}
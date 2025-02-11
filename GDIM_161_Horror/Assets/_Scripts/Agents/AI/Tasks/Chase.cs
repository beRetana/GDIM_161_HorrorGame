using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI{

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
            if ((_target.transform.position - transform.position).magnitude > _catchRange)
            {
                _aiController.MoveTo(_target.transform.position);
            }
            else
            {
                CatchPlayer();
            }
        }

        private void CatchPlayer()
        {
            _aiController.AbortTask();
            _taskCompleted.Invoke(succeful: true);
        }

        public void SetTarget(GameObject target)
        {
            _target = target;
        }

        public void SetTargetLost()
        {
            _lastKnownLocation = _target.transform.position;
            _target = null;
            _aiController.AbortTask();
            _aiController.MoveTo(_lastKnownLocation);
            _aiController.onTaskCompleted += OnCheckedLastLocation;
        }

        public override void Enable()
        {
            _aiController.ChangeSpeed(_chaseSpeed);
        }

        public override void Disable()
        {
            _aiController.onTaskCompleted -= OnCheckedLastLocation;
            _aiController.DefaultSpeed();
        }

        private void OnCheckedLastLocation()
        {
            _taskCompleted.Invoke(succeful: false);
            Disable();
        }
    }
}
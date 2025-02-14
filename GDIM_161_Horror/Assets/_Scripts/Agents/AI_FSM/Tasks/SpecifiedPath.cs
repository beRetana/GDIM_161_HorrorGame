using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OtherUtils;
using System;

namespace AI_FSM{
    /// <summary>
    /// Task that will make the AI move through a specified path
    /// in a random sequence or ascendand.
    /// </summary>
    public class SpecifiedPath : TaskBase
    {

        [Header("Specified Path Settings")]
        [SerializeField] private List<Transform> _locations;
        [SerializeField] private float _waitTimeAtStop;
        [SerializeField] private bool _randomOrder;

        private Action OnReachedLocation;

        private bool _reachedLocation = false;

        public override void Enable()
        {
            _aiController.onTaskCompleted += UpdateReachedLocation;
            StartCoroutine(MoveThroughPath());
        }

        public override void Disable()
        {
            StopCoroutine(MoveThroughPath());
            _aiController.onTaskCompleted -= UpdateReachedLocation;
        }

        private void UpdateReachedLocation()
        {
            _reachedLocation = true;
        }

        private IEnumerator MoveThroughPath()
        {
            if (_randomOrder)
            {
                ListUtils.Shuffle(_locations);
            }

            foreach (Transform location in _locations)
            {
                _aiController.MoveTo(location.position);
                yield return new WaitUntil(() => _reachedLocation);
                OnReachedLocation.Invoke();
                yield return new WaitForSeconds(_waitTimeAtStop);
                Debug.Log($"Reached: {location.position}");
                _reachedLocation = false;
            }

            _taskCompleted.Invoke(succeful: true);
        }
    }
}


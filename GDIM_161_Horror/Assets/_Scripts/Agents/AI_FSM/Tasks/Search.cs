using System;
using System.Collections;
using UnityEngine;

namespace AI_FSM{

    public class Search : TaskBase{

        [Header("Search Settings")]
        [SerializeField] private float _searchRadius = 3f;
        [SerializeField] private float _searchTime = 3f;
        [SerializeField] private float _searchSpeed = 5f;
        [SerializeField] private int _searchCycles = 3;

        private bool _hasReachedLocation;

        public Action OnSearchLocation;

        //SEARCH BEHAVIOR
        public override void Enable()
        {
            _aiController.ChangeSpeed(_searchSpeed);
            _aiController.onTaskCompleted += UpdateHasReachedLocation;
            StartCoroutine(SearchSequence());
        }

        public override void Disable()
        {
            _aiController.onTaskCompleted -= UpdateHasReachedLocation;
            _aiController.DefaultSpeed();
            StopCoroutine(SearchSequence());
        }

        private void UpdateHasReachedLocation()
        {
            _hasReachedLocation = true;
        }

        /// <summary>
        /// The AI will move to random locations wait (search) then move again
        /// for as many cycles as determined.
        /// </summary>
        IEnumerator SearchSequence()
        {
            for (int i = 0; i < _searchCycles; i++)
            {
                _aiController.MoveToRandomLocation(_searchRadius);
                yield return new WaitUntil(() => _hasReachedLocation);
                OnSearchLocation.Invoke();
                yield return new WaitForSeconds(_searchTime);
                Debug.Log($"Finished search cycle: {i}");
                _hasReachedLocation = false;
            }

            _taskCompleted.Invoke(succeful:true);
        }
    }
}
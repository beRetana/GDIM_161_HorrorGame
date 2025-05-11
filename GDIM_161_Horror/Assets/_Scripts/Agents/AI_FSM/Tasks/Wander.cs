using System;
using System.Collections;
using UnityEngine;

namespace AI_FSM{

    public class Wander : TaskBase{

        [Header("Wandering Settings")]
        [SerializeField] private float _wanderRadius = 5f;
        [SerializeField] private float _wanderTime = 3f;

        public Action OnReachedLocation;

        //WANDERING BEHAVIOR
        public override void Enable()
        {
            _aiController.MoveToRandomLocation(_wanderRadius);
            _aiController.onTaskCompleted += UpdateReachedLocation;
        }

        public override void Disable()
        {
            StopCoroutine(MoveRandomly());
            _aiController.onTaskCompleted -= UpdateReachedLocation;
        }

        private void UpdateReachedLocation()
        {
            StartCoroutine(MoveRandomly());
        }

        IEnumerator MoveRandomly()
        {
            yield return new WaitForSecondsRealtime(_wanderTime);
            _aiController.MoveToRandomLocation(_wanderRadius);
        }
    }
}


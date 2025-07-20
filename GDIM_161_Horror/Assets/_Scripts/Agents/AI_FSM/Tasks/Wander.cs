using System;
using System.Collections;
using UnityEngine;

namespace AI_FSM{

    public class Wander : TaskBase{

        [Header("Wandering Settings")]
        [SerializeField] private float _wanderRadius = 5f;
        [SerializeField] private float _wanderTime = 3f;

        public Action OnReachedLocation;

        private Coroutine m_Behaviour;

        // This might be called from a different start function
        // before its own start function making the call invalid
        // that's why the coroutine will help wait until components get assigned.
        public override void StartBehaviour()
        {
            StartCoroutine(WaitFor<AIController>(_aiController, EnableBehaviour));
        }

        private void EnableBehaviour()
        {
            _aiController.MoveToRandomLocation(_wanderRadius);
            _aiController.onTaskCompleted += UpdateReachedLocation;
        }

        private IEnumerator WaitFor<T>(T component, Action action) where T : Component
        {
            while (component == null)
            {
                yield return null;
                component = GetComponent<T>();
            }
            
            action();
        }

        public override void StopBehaviour()
        {
            _aiController.onTaskCompleted -= UpdateReachedLocation;
            if (m_Behaviour == null) return;
            StopCoroutine(m_Behaviour);
        }

        private void UpdateReachedLocation()
        {
            m_Behaviour = StartCoroutine(MoveRandomly());
        }

        IEnumerator MoveRandomly()
        {
            yield return new WaitForSecondsRealtime(_wanderTime);
            _aiController.MoveToRandomLocation(_wanderRadius);
        }
    }
}


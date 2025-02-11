using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AI
{
    public class Patrolling : TaskBase{

        [Header("Patrol Settings")]
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float patrolWaitTime = 3f;
        private int currentPatrolIndex = -1;
        
        public override void Enable(){
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            _aiController.MoveTo(patrolPoints[currentPatrolIndex].position);
            _aiController.onTaskCompleted += OnPatrolPointReached;
        }

        public override void Disable(){
            _aiController.onTaskCompleted -= OnPatrolPointReached;
        }

        private void OnPatrolPointReached(object sender, EventArgs e)
        {
            _aiController.onTaskCompleted -= OnPatrolPointReached;
            StartCoroutine(WaitAtPatrolPoint());
        }

        private IEnumerator WaitAtPatrolPoint()
        {
            yield return new WaitForSeconds(patrolWaitTime);
        }
    }
}

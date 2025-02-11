using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI{

    [RequireComponent(typeof(AIController))]
    public class TaskBase : MonoBehaviour{

        protected AIController _aiController;

        /// <summary>
        /// Event that will be triggered when the task is completed.
        /// </summary>
        /// <returns>Returns True if the task was successful and False if it failed.</returns>
        public delegate void TaskCompleted(bool succeful);
        public TaskCompleted _taskCompleted;

        private void Start()
        {
            _aiController = GetComponent<AIController>();
        }

        public void SetAIContorller(AIController newAIController)
        {
            _aiController = newAIController;
        }

        public virtual void Enable(){}

        public virtual void Disable(){}
    }
}

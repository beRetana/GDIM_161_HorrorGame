using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_FSM{

    [RequireComponent(typeof(AIController))]
    public class TaskBase : MonoBehaviour{

        protected AIController _aiController;

        public delegate void TaskNotify(bool successful);

        /// <summary>
        /// Event that will be triggered when the task is completed.
        /// </summary>
        /// <returns>Returns True if the task was successful and False if it failed.</returns>
        public event TaskNotify OnTaskCompleted;

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

        protected void SendNotification(bool successful)
        {
            OnTaskCompleted.Invoke(successful);
        }
    }
}

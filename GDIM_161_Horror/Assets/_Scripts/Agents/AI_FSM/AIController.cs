using System;
using UnityEngine;
using UnityEngine.AI;

namespace AI_FSM{

    [RequireComponent(typeof(NavMeshAgent))]
    public class AIController : MonoBehaviour{

        /// <summary>
        /// The state of the AI.
        /// </summary>
        private enum State
        {
            Idle,
            Started,
            Processing,
            Done
        }

        public enum TaskType
        {
            Movement,
        }

        private NavMeshAgent _agent;
        private State _state = State.Idle;
        private TaskType _taskType = TaskType.Movement;

        private bool _stateTriggered;
        private float _agentDefaultSpeed;

        public Action onTaskCompleted;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agentDefaultSpeed = _agent.speed;
        }

        private void Update()
        {
            switch(_state)
            {
                case State.Idle:
                    if (!_stateTriggered)
                    {
                        Debug.Log($"State of task is {_state}");
                        _stateTriggered = true;
                    }
                    break;
                case State.Started:
                    if (!_stateTriggered)
                    {
                        Debug.Log($"State of task is {_state}");
                        _stateTriggered = true;
                        _state = State.Processing;
                    }
                    break;
                case State.Processing:

                    if(!_stateTriggered)
                    {
                        Debug.Log($"State of task is {_state}");
                        _stateTriggered = true;
                    }

                    switch(_taskType)
                    {
                        case TaskType.Movement:
                            if (_agent.remainingDistance <= _agent.stoppingDistance){
                                _state = State.Done;
                                _stateTriggered = true;
                            }
                            break;
                        default:
                            Debug.LogError("Task type not recognized");
                            break;
                    }
                    
                    break;
                case State.Done:
                    Debug.Log($"State of task is {_state}");
                    _state = State.Idle;
                    onTaskCompleted?.Invoke();
                    break;
            }
        }

        /// <summary>
        /// Move the AI to the target location.
        /// </summary>
        /// <param name="targetLocation">Location to move to.</param>
        public void MoveTo(Vector3 targetLocation){
            _agent.SetDestination(targetLocation);
            _stateTriggered = false;
            _state = State.Started;
        }

        /// <summary>
        /// Move the AI to a random location within the radius.
        /// </summary>
        /// <param name="radius">The radius the AI will move within.</param>
        public void MoveToRandomLocation(float radius){
            float xPos = UnityEngine.Random.Range(-radius,radius);
            float zPos = UnityEngine.Random.Range(-radius,radius);

            MoveTo(new Vector3(xPos, 0, zPos));
        }

        /// <summary>
        /// Move the AI away from the location.
        /// </summary>
        /// <param name="awayLocation"></param>
        public void MoveAway(Vector3 awayLocation){

            MoveTo(-(awayLocation - transform.position));
        }

        /// <summary>
        /// Replace the speed of the AI.
        /// </summary>
        /// <param name="newSpeed">The new speed of the AI</param>
        public void ChangeSpeed(float newSpeed){
            _agent.speed = newSpeed;
        }

        /// <summary>
        /// Reset the speed of the AI to the default speed.
        /// </summary>
        public void DefaultSpeed(){
            _agent.speed = _agentDefaultSpeed;
        }

        /// <summary>
        /// Abort the current task and reset the AI to idle.
        /// </summary>
        public void AbortTask(){
            _state = State.Idle;
            _agent.ResetPath();
            _stateTriggered = false;
            DefaultSpeed();
        }
    }
}

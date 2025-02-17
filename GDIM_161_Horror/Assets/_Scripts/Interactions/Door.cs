using Codice.Client.Common.Threading;
using NUnit.Framework.Constraints;
using System;
using UnityEngine;

namespace Interactions
{
    public class Door : MonoBehaviour
    {
        [Header("Door Settings/Components")]
        [SerializeField] private DoorHandle _rightDoorHandle;
        [SerializeField] private DoorHandle _leftDoorHandle;
        [SerializeField] private float _openDistance = 1.5f;

        private int _playersOnHandles;

        private enum DoorState
        {
            Locked = 1 << 0,
            Locking = 1 << 1,
            Unlocking = 1 << 2,
            Unlocked = 1 << 3,
        }

        private DoorState _doorState;

        private void Start()
        {
            _doorState = DoorState.Locked;
        }

        public void UpdateDoorState()
        {
            switch (_doorState)
            {
                case DoorState.Locked:
                    break;
                case DoorState.Locking:
                    LockingDoors();
                    break;
                case DoorState.Unlocking:
                    UnlockingDoors();
                    break;
                case DoorState.Unlocked:
                    UnlockedDoors();
                    break;
                default:
                    throw new Exception("No State Found");
            }
        }

        private void LockingDoors()
        {
            FreezeDoors();

            _rightDoorHandle.CloseDoor();
            _leftDoorHandle.CloseDoor();

            _doorState = DoorState.Locked;
        }

        private void UnlockingDoors()
        {
           
        }

        private void UnlockedDoors()
        {
            FreezeDoors();
        }

        private void FreezeDoors()
        {
            
        }

        public void OnPlayerHandleInteraction(bool isPlayerOnHandler)
        {
            if (isPlayerOnHandler) ++_playersOnHandles; 
            else --_playersOnHandles;

            if (_playersOnHandles >= 2f ) 
            {
                _doorState = DoorState.Unlocking;
                UpdateDoorState();
            }
        }
    }
}

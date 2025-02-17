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

        private void Update()
        {
            if (_doorState == DoorState.Unlocking) CheckFullyOpened();
        }

        public void UpdateDoorState()
        {
            switch (_doorState)
            {
                case DoorState.Locked:
                    break;
                case DoorState.Unlocking:
                    UnlockingDoors();
                    break;
                case DoorState.Locking:
                    LockingDoors();
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
            _rightDoorHandle.CloseDoor();
            _leftDoorHandle.CloseDoor();
            _doorState = DoorState.Locked;
        }

        private void UnlockingDoors()
        {
            _rightDoorHandle.DoorCanMove();
            _leftDoorHandle.DoorCanMove();
        }

        private void UnlockedDoors()
        {
            _rightDoorHandle.DetachingFromPlayer();
            _leftDoorHandle.DetachingFromPlayer();
            _rightDoorHandle.SetInteractive(false);
            _leftDoorHandle.SetInteractive(false);
        }

        private void CheckFullyOpened()
        {
            if (DistancedEnough(_leftDoorHandle.transform.position) || DistancedEnough(_rightDoorHandle.transform.position)) return;
            _doorState = DoorState.Unlocked;
            UpdateDoorState();
        }

        private bool DistancedEnough(Vector3 position)
        {
            return Vector3.Distance(_rightDoorHandle.transform.position, transform.position) > _openDistance;
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

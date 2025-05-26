using Mirror;
using System;
using UnityEngine;

namespace Interactions
{
    public class DoubleDoor : NetworkBehaviour
    {
        [Header("Door Settings/Components")]
        [SerializeField] private DoorHandle _rightDoorHandle;
        [SerializeField] private DoorHandle _leftDoorHandle;
        [SerializeField] private bool _debugger;

        [SyncVar] private int _playersOnHandles;

        public enum DoorState
        {
            Locked = 1 << 0,
            Unlocking = 1 << 2,
            Unlocked = 1 << 3,
        }

        [SyncVar] private DoorState _doorState;

        private void Start()
        {
            _doorState = DoorState.Locked;
        }

        public void UpdateDoorState(DoorState state)
        {
            if (isServer) RpcDoorState(state);
            else CmdDoorState(state);
        }

        [Command]
        private void CmdDoorState(DoorState state)
        {
            RpcDoorState(state);
        }

        [ClientRpc]
        private void RpcDoorState(DoorState state)
        {
            this._doorState = state;
            switch (_doorState)
            {
                case DoorState.Locked:
                    break;
                case DoorState.Unlocking:
                    UnlockingDoors();
                    break;
                default:
                    throw new Exception("No State Found");
            }
        }

        private void UnlockingDoors()
        {
            _rightDoorHandle.DoorCanMove();
            _leftDoorHandle.DoorCanMove();
        }

        public void OnPlayerHandleInteraction(bool isPlayerOnHandler)
        {
            if (isPlayerOnHandler) ++_playersOnHandles; 
            else --_playersOnHandles;
            Debugger($"DOUBLE DOOR: There are {_playersOnHandles} players on the Handles");
            if (_playersOnHandles >= 2 ) UpdateDoorState(DoorState.Unlocking);
        }

        private void Debugger(object log)
        {
            if (_debugger) Debug.Log(log);
        }
    }
}

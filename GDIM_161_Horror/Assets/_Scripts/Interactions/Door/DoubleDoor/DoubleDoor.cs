using Mirror;
using OtherUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interactions
{
    public class DoubleDoor : NetworkBehaviour, IDebugger
    {
        [Header("Door Settings/Components")]
        [SerializeField] private List<DoorHandle> _doorHandles;
        [SerializeField] private float _openDoorAnimationTime;
        [SerializeField] private float _openDoorDelay = 2f;
        [SerializeField] private float _grabHandlesAnimationTime;
        [SerializeField] private DoorData _rightDoor;
        [SerializeField] private DoorData _leftDoor;

        private List<int> m_PlayersOnDoor;
        private bool _debugger;
        public float DoorAnimTime => _openDoorAnimationTime;
        public float DoorDelay => _openDoorDelay;
        public float HandleGrabAnimTime => _grabHandlesAnimationTime;

        [Serializable]
        struct DoorData
        {
            public Transform DoorTransform;
            public Transform DoorOpenTarget;
        }

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
            m_PlayersOnDoor = new List<int>();
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
            foreach (DoorHandle handle in _doorHandles)
                handle.MovePlayer();

            StartCoroutine(OpenDoors(_rightDoor));
            StartCoroutine(OpenDoors(_leftDoor));
        }

        IEnumerator OpenDoors(DoorData doorData)
        {
            yield return new WaitForSeconds(_openDoorDelay);

            Vector3 doorOriginalPosition = doorData.DoorTransform.position;
            float ratio = 0;
            for (float timeElapsed = 0; ratio <= 1; ratio = timeElapsed / _openDoorAnimationTime)
            {
                doorData.DoorTransform.position = Vector3.Lerp(doorOriginalPosition, doorData.DoorOpenTarget.position, ratio);
                yield return null;
                timeElapsed += Time.deltaTime;
            }
            doorData.DoorTransform.position = doorData.DoorOpenTarget.position;
        }

        public void OnPlayerHandleInteraction(bool isPlayerOnHandler)
        {
            if (isPlayerOnHandler) ++_playersOnHandles; 
            else --_playersOnHandles;
            Debugger($"DOUBLE DOOR: There are {_playersOnHandles} players on the Handles");
            if (_playersOnHandles >= _doorHandles.Count) UpdateDoorState(DoorState.Unlocking);
        }

        public void AddPlayerID(int playerID)
        {
            m_PlayersOnDoor.Add(playerID);
        }

        public void RemovePlayerID(int playerID)
        {
            m_PlayersOnDoor.Remove(playerID);
        }

        public bool IsPlayerOnDoor(int playerID)
        {
            return m_PlayersOnDoor.Contains(playerID);
        }

        public void Debugger(object log)
        {
            if (_debugger) Debug.Log(log);
        }

        public void SetDebugActive(bool active)
        {
            _debugger = active;
        }
    }
}

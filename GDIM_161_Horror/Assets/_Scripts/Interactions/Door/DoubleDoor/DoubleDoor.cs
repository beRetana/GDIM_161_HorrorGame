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
        [SerializeField] private DoorButton[] _doorHandles;
        [SerializeField] private float _openDoorAnimationTime;
        [SerializeField] private float _openDoorDelay = 2f;
        [SerializeField] private DoorData _rightDoor;
        [SerializeField] private DoorData _leftDoor;

        private List<byte> m_PlayersOnDoor;
        private bool _debugger;

        [Serializable]
        struct DoorData
        {
            public Transform DoorTransform;
            public Transform DoorOpenTarget;
        }

        public enum DoorState
        {
            Locked = 1 << 0,
            Unlocking = 1 << 2,
            Unlocked = 1 << 3,
        }

        private DoorState _doorState;
        public DoorState State => _doorState;

        private void Start()
        {
            _doorState = DoorState.Locked;
            m_PlayersOnDoor = new List<byte>();
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
            _doorState = state;
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

        public void AddPlayer(byte playerID)
        {
            if (isServer) RpcAddPlayer(playerID);
            else CmdAddPlayer(playerID);
        }

        [Command]
        private void CmdAddPlayer(byte playerID)
        {
            RpcAddPlayer(playerID);
        }

        [ClientRpc]
        private void RpcAddPlayer(byte playerID)
        {
            m_PlayersOnDoor.Add(playerID);
            if (m_PlayersOnDoor.Count < _doorHandles.Length) return;
            UpdateDoorState(DoorState.Unlocking);
        }
        
        public void RemovePlayer(byte playerID)
        {
            if (isServer) RpcRemovePlayer(playerID);
            else CmdRemovePlayer(playerID);
        }

        [Command]
        private void CmdRemovePlayer(byte playerID)
        {
            RpcRemovePlayer(playerID);
        }

        [ClientRpc]
        private void RpcRemovePlayer(byte playerID)
        {
            m_PlayersOnDoor.Remove(playerID);
        }

        public bool HasPlayerPressed(byte playerID)
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

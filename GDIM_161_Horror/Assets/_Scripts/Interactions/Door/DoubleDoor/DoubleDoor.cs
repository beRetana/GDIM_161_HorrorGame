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
        [SerializeField] private DoorButton[] m_DoorButtons;
        [SerializeField] private float _openDoorAnimationTime;
        [SerializeField] private float _openDoorDelay = 2f;
        [SerializeField] private DoorData m_RightDoor;
        [SerializeField] private DoorData m_LeftDoor;

        [SyncVar] private byte m_PlayersRequired;

        private List<byte> m_PlayersOnDoor;
        private Vector3 m_RightDoorOriginal;
        private Vector3 m_LeftDoorOriginal;
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
            m_RightDoorOriginal = m_RightDoor.DoorTransform.position;
            m_LeftDoorOriginal = m_LeftDoor.DoorTransform.position;
            m_PlayersOnDoor = new List<byte>();

            if (!isServer) return;
            
            if (NewNetworkManager.NewSingleton.ArePlayersReady())
            {
                RpcAdjustButtonNumber();
            }
            NewNetworkManager.NewSingleton.OnPlayersServerReady += RpcAdjustButtonNumber;
        }

        private void OnDisable()
        {
            NewNetworkManager.NewSingleton.OnPlayersServerReady -= RpcAdjustButtonNumber;
        }

        [ClientRpc]
        private void RpcAdjustButtonNumber()
        {
            m_PlayersRequired = (byte)NewNetworkManager.NewSingleton.numPlayers;

            for (int i = m_DoorButtons.Length - 1; i >= m_PlayersRequired; --i)
            {
                m_DoorButtons[i].transform.parent.gameObject.SetActive(false);
            }
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
            StartCoroutine(OpenDoors(m_RightDoor.DoorTransform, m_RightDoorOriginal, m_RightDoor.DoorOpenTarget.position));
            StartCoroutine(OpenDoors(m_LeftDoor.DoorTransform, m_LeftDoorOriginal, m_LeftDoor.DoorOpenTarget.position));
        }

        public void LockingDoors()
        {
            StartCoroutine(OpenDoors(m_RightDoor.DoorTransform, m_RightDoor.DoorOpenTarget.position, m_RightDoorOriginal));
            StartCoroutine(OpenDoors(m_LeftDoor.DoorTransform, m_LeftDoor.DoorOpenTarget.position, m_LeftDoorOriginal));
        }

        IEnumerator OpenDoors(Transform doorTransform, Vector3 startingPosition, Vector3 endingPosition)
        {
            yield return new WaitForSeconds(_openDoorDelay);

            float ratio = 0;
            for (float timeElapsed = 0; ratio <= 1; ratio = timeElapsed / _openDoorAnimationTime)
            {
                doorTransform.position = Vector3.Lerp(startingPosition, endingPosition, ratio);
                yield return null;
                timeElapsed += Time.deltaTime;
            }
            doorTransform.position = endingPosition;
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
            if (m_PlayersOnDoor.Count < m_PlayersRequired) return;
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

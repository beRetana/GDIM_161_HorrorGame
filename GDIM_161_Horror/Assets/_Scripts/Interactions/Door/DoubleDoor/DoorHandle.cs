using MessengerSystem;
using Mirror;
using OtherUtils;
using System.Collections;
using UnityEngine;

namespace Interactions
{
    public class DoorHandle : NetworkBehaviour, IDebugger
    {
        [SerializeField] private DoubleDoor m_DoorsManager;
        [SerializeField] private Transform _playerGrabTarget;
        [SerializeField] private Transform _playerOpenTarget;
        [SerializeField] private FixedJoint _handleJoint;
        [SerializeField] private string _grabDisplayMessage;
        [SerializeField] private string _releaseDisplayMessage;
        
        private InteractableItem m_InteractableItem;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private bool _debugger;

        [SyncVar] private bool _isHandleOnUse;
        [SyncVar] private int m_PlayerUserID = -1;

        private void Start()
        {
            m_InteractableItem = GetComponent<PolyInteractable>();
            m_InteractableItem.SetInteractAction(OnInteracted);
            _targetRotation = _playerGrabTarget.rotation;
        }

        public void OnInteracted(int playerID)
        {
            bool isDifferentPlayerOrEmpty = m_PlayerUserID != playerID;

            /*If the player is handling the door and it is a different player or the handle is empty
              then, reject interaction: this means this player is interacting with another handle */
            if (m_DoorsManager.IsPlayerOnDoor(playerID) & isDifferentPlayerOrEmpty) return;

            /*If the handle is full (-1 means empty) and the player is different, reject interaction
              this means another player is trying to interact*/
            if (m_PlayerUserID != -1 & isDifferentPlayerOrEmpty) return;

            Debugger($"{transform.parent.parent.parent.name}: Player {playerID} is interacting");

            /*By elimination this bool only mean if the player interacting is different, then attach to handle*/
            if (isDifferentPlayerOrEmpty) PlayerGettingOnHandle(playerID);

            /*By elimination this mean, this player has not interacted with the door
              but it is attached to the handle thus unattach them from the handle*/
            else PlayerGettingOffHandle(playerID);
        }

        private void PlayerGettingOnHandle(int playerID)
        {
            Debugger($"Player {playerID} getting ON handle");

            m_DoorsManager.AddPlayerID(playerID);
            PlayerManager.Instance.LockPlayerInput(playerID);
            StartCoroutine(GrabHandleAnimation(playerID));
        }

        private void PlayerGettingOffHandle(int playerID)
        {
            Debugger($"Player {playerID} getting OFF handle");

            m_DoorsManager.RemovePlayerID(playerID);
            m_InteractableItem.SetDisplayMessage(_grabDisplayMessage);
            PlayerManager.Instance.UnlockPlayerInput(playerID);
            DetachingFromPlayer();
            UpdateHandleState(playerID, false);
        }

        private void UpdateDoorManager(bool isPlayerOnHandler)
        {
            m_DoorsManager.OnPlayerHandleInteraction(isPlayerOnHandler);
        }

        private void UpdateHandleState(int playerID, bool isPlayerOnHandle)
        {
            if (isServer) RpcUpdateHandleState(playerID, isPlayerOnHandle);
            else CmdUpdateHandleState(playerID, isPlayerOnHandle);
        }

        [ClientRpc]
        private void RpcUpdateHandleState(int playerID, bool isPlayerOnHandle)
        {
            this.m_PlayerUserID = playerID;
            this._isHandleOnUse = isPlayerOnHandle;
            this.UpdateDoorManager(isPlayerOnHandle);
        }

        [Command]
        private void CmdUpdateHandleState(int playerID, bool isPlayerOnHandle)
        {
            RpcUpdateHandleState(playerID, isPlayerOnHandle);
        }

        public void DetachingFromPlayer()
        {
            _handleJoint.connectedBody = null;
        }

        private void AttachingToPlayer(int playerId)
        {
            PlayerArticulations playerArticulations = PlayerManager.Instance.GetPlayer(playerId).GetComponent<PlayerArticulations>();
            _handleJoint.connectedBody = playerArticulations.PlayerHandRigidbody;
        }

        public void MovePlayer()
        {
            SetInteractive(false);
            StartCoroutine(MovePlayerOpeningDoor());
        }

        public void SetInteractive(bool isInteractive)
        {
            m_InteractableItem.SetInteractive(isInteractive);
        }

        IEnumerator MovePlayerOpeningDoor()
        { 
            yield return new WaitForSeconds(m_DoorsManager.DoorDelay);

            Transform playerTransform = PlayerManager.Instance.GetPlayer(m_PlayerUserID).transform;

            Vector3 playerOriginalPosition = playerTransform.position;
            Vector3 playerTarget = new Vector3(_playerOpenTarget.position.x, playerTransform.position.y,
                _playerOpenTarget.position.z);

            float ratio = 0;
            for (float timeElapsed = 0; ratio <= 1; ratio = timeElapsed / m_DoorsManager.DoorAnimTime)
            {
                playerTransform.position = Vector3.Lerp(playerOriginalPosition, playerTarget, ratio);
                yield return null;
                timeElapsed += Time.deltaTime;
            }

            playerTransform.position = playerTarget;
            PlayerGettingOffHandle(m_PlayerUserID);
        }

        IEnumerator GrabHandleAnimation(int playerId)
        {
            PlayerBase player = PlayerManager.Instance.GetPlayer(playerId);

            Transform playerTransform = player.transform;

            Vector3 playerOriginalPosition = player.transform.position;
            _targetPosition = new Vector3(_playerGrabTarget.position.x, player.transform.position.y, _playerGrabTarget.position.z);

            Quaternion playerOriginalRotation = player.transform.rotation;

            float ratio = 0;
            for(float time = 0; ratio <= 1; time += Time.deltaTime)
            {
                ratio = time / m_DoorsManager.HandleGrabAnimTime;
                player.transform.position = Vector3.Lerp(playerOriginalPosition, _targetPosition, ratio);
                player.transform.rotation = Quaternion.Slerp(playerOriginalRotation, _targetRotation, ratio);

                yield return null;
            }

            Debugger($"{transform.parent.parent.parent.name}: Is moving Player {playerId}");
            AttachingToPlayer(playerId);
            m_InteractableItem.SetDisplayMessage(_releaseDisplayMessage);
            UpdateHandleState(playerId, true);
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

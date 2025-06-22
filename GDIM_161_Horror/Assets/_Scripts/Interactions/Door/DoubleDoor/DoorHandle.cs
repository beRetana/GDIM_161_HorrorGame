using MessengerSystem;
using Mirror;
using OtherUtils;
using System.Collections;
using UnityEngine;

namespace Interactions
{
    public class DoorHandle : NetworkBehaviour, IDebugger
    {
        [SerializeField] private DoubleDoor _doorsManager;
        [SerializeField] private Transform _playerGrabTarget;
        [SerializeField] private Transform _playerOpenTarget;
        [SerializeField] private FixedJoint _handleJoint;
        [SerializeField] private string _grabDisplayMessage;
        [SerializeField] private string _releaseDisplayMessage;
        

        private delegate void UnlockPlayer();
        private UnlockPlayer OnUnlockPlayer;

        private InteractableItem _interactableItem;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private bool _debugger;

        [SyncVar] private bool _isPlayerOnHandle;
        [SyncVar] private int _playerUserID;

        private void Start()
        {
            _interactableItem = GetComponent<PolyInteractable>();
            _interactableItem.SetInteractAction(OnInteracted);
            _targetRotation = _playerGrabTarget.rotation;
        }

        public void OnInteracted(int playerId)
        {
            if (_isPlayerOnHandle && _playerUserID != playerId) return;
            Debugger($"{transform.parent.parent.parent.name}: Player {playerId} is interacting");
            if (!_isPlayerOnHandle) PlayerGettingOnHandle(playerId);
            else PlayerGettingOffHandle(playerId);
        }

        private void PlayerGettingOnHandle(int playerID)
        {
            PlayerManager.Instance.LockPlayerInput(playerID);
            StartCoroutine(GrabHandleAnimation(playerID));
        }

        private void PlayerGettingOffHandle(int playerID)
        {
            _interactableItem.SetDisplayMessage(_grabDisplayMessage);
            PlayerManager.Instance.UnlockPlayerInput(playerID);
            DetachingFromPlayer();
            OnUnlockPlayer = null;
            UpdateHandleState(playerID, false);
        }

        private void UpdateDoorManager(bool isPlayerOnHandler)
        {
            this._doorsManager.OnPlayerHandleInteraction(isPlayerOnHandler);
        }

        private void UpdateHandleState(int playerID, bool isPlayerOnHandle)
        {
            if (isServer) RpcUpdateHandleState(playerID, isPlayerOnHandle);
            else CmdUpdateHandleState(playerID, isPlayerOnHandle);
        }

        [ClientRpc]
        private void RpcUpdateHandleState(int playerID, bool isPlayerOnHandle)
        {
            this._playerUserID = playerID;
            this._isPlayerOnHandle = isPlayerOnHandle;
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
            OnUnlockPlayer = () => { PlayerManager.Instance.UnlockPlayerInput(playerId); };
        }

        public void MovePlayer()
        {
            SetInteractive(false);
            StartCoroutine(MovePlayerOpeningDoor());
        }

        public void SetInteractive(bool isInteractive)
        {
            _interactableItem.SetInteractive(isInteractive);
        }

        IEnumerator MovePlayerOpeningDoor()
        { 
            yield return new WaitForSeconds(_doorsManager.DoorDelay);

            Transform playerTransform = PlayerManager.Instance.GetPlayer(_playerUserID).transform;

            Vector3 playerOriginalPosition = playerTransform.position;
            Vector3 playerTarget = new Vector3(_playerOpenTarget.position.x, playerTransform.position.y,
                _playerOpenTarget.position.z);

            float ratio = 0;
            for (float timeElapsed = 0; ratio <= 1; timeElapsed += Time.deltaTime)
            {
                ratio = Mathf.Clamp01(timeElapsed / _doorsManager.DoorAnimTime);
                playerTransform.position = Vector3.Lerp(playerOriginalPosition, playerTarget, ratio);
                yield return null;
            }

            OnUnlockPlayer?.Invoke();
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
                ratio = time / _doorsManager.HandleGrabAnimTime;
                player.transform.position = Vector3.Lerp(playerOriginalPosition, _targetPosition, ratio);
                player.transform.rotation = Quaternion.Slerp(playerOriginalRotation, _targetRotation, ratio);

                yield return null;
            }

            Debugger($"{transform.parent.parent.parent.name}: Is moving Player {playerId}");
            AttachingToPlayer(playerId);
            _interactableItem.SetDisplayMessage(_releaseDisplayMessage);
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

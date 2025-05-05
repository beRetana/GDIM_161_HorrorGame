using MessengerSystem;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Interactions
{
    public class DoorHandle : NetworkBehaviour
    {
        [SerializeField] private DoubleDoor _doorsManager;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Transform _doorOpenedTarget;
        [SerializeField] private Rigidbody _doorRigidbody;
        [SerializeField] private FixedJoint _handleJoint;
        [SerializeField] private string _grabDisplayMessage;
        [SerializeField] private string _releaseDisplayMessage;
        [SerializeField] private float _animTime;
        [SerializeField] private float _openingDoorsDuration;
        [SerializeField] private bool _debugger;

        private delegate void UnlockPlayer();
        private UnlockPlayer OnUnlockPlayer;

        private InteractableItem _interactableItem;
        private Vector3 _targetPosition, _initialPosition;
        private Quaternion _targetRotation;
        [SyncVar] private bool _isPlayerOnHandle;
        [SyncVar] private int _playerUserID;

        private void Start()
        {
            _interactableItem = GetComponent<InteractableItem>();
            _interactableItem.SetInteractAction(OnInteracted);
            _targetRotation = _targetTransform.rotation;
            _initialPosition = transform.position;
        }

        public void OnInteracted(int playerId)
        {
            if (_isPlayerOnHandle && _playerUserID != playerId) return;
            Debugger($"{gameObject.name}: Player {playerId} is interacting");
            if (!_isPlayerOnHandle) PlayerGettingOnHandle(playerId);
            else PlayerGettingOffHandle(playerId);
        }

        private void PlayerGettingOnHandle(int playerID)
        {
            PlayerManager.Instance.LockPlayerInput(playerID);
            StartCoroutine(MovePlayerAnimation(playerID));
            UpdateHandleState(playerID, true);
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
            _doorRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        private void AttachingToPlayer(int playerId)
        {
            PlayerArticulations playerArticulations = PlayerManager.Instance.GetPlayer(playerId).GetComponent<PlayerArticulations>();
            _handleJoint.connectedBody = playerArticulations.PlayerHandRigidbody;
            _doorRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
            OnUnlockPlayer = () => { PlayerManager.Instance.UnlockPlayerInput(playerId); };
        }

        public void DoorCanMove()
        {
            SetInteractive(false);
            StartCoroutine(OpenDoors(_playerUserID, _openingDoorsDuration, _doorOpenedTarget.position));
        }

        public void SetInteractive(bool isInteractive)
        {
            _interactableItem.SetInteractive(isInteractive);
        }

        IEnumerator OpenDoors(int playerID, float duration, Vector3 target)
        { 
            Transform playerTransform = PlayerManager.Instance.GetPlayer(playerID).transform;
            float timeElapsed = 0;

            Vector3 playerOriginalPosition = playerTransform.position;
            _targetPosition = new Vector3(target.x, playerTransform.position.y, target.z);

            for (; timeElapsed <= _animTime; timeElapsed += Time.deltaTime)
            {
                playerTransform.position = Vector3.Lerp(playerOriginalPosition, _targetPosition, Mathf.Clamp01(timeElapsed/duration));
                yield return null;
            }

            OnUnlockPlayer?.Invoke();
        }

        IEnumerator MovePlayerAnimation(int playerId)
        {
            PlayerBase player = PlayerManager.Instance.GetPlayer(playerId);
            float time = 0;

            Transform playerTransform = player.transform;

            Vector3 playerOriginalPosition = player.transform.position;
            _targetPosition = new Vector3(_targetTransform.position.x, player.transform.position.y, _targetTransform.position.z);

            Quaternion playerOriginalRotation = player.transform.rotation;

            while (time <= _animTime)
            {
                player.transform.position = Vector3.Lerp(playerOriginalPosition, _targetPosition, time / _animTime);
                player.transform.rotation = Quaternion.Slerp(playerOriginalRotation, _targetRotation, time / _animTime);

                yield return null;
                time += Time.deltaTime;
            }

            AttachingToPlayer(playerId);
            _interactableItem.SetDisplayMessage(_releaseDisplayMessage);
        }

        private void Debugger(object log)
        {
            if (_debugger) Debug.Log(log);
        }
    }
}

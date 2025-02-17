using MessengerSystem;
using System.Collections;
using UnityEngine;

namespace Interactions
{
    public class DoorHandle : MonoBehaviour
    {
        [SerializeField] private Door _doorsManager;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Rigidbody _doorRigidbody;
        [SerializeField] private FixedJoint _handleJoint;
        [SerializeField] private float _animTime;
        [SerializeField] private string _grabDisplayMessage;
        [SerializeField] private string _releaseDisplayMessage;

        private delegate void UnlockPlayer();
        private UnlockPlayer OnUnlockPlayer;

        private InteractableItem _interactableItem;
        private Vector3 _targetPosition, _initialPosition;
        private Quaternion _targetRotation;
        private bool _isPlayerOnHandle;

        private void Start()
        {
            _interactableItem = GetComponent<InteractableItem>();
            _interactableItem.SetInteractAction(OnInteracted);
            _targetRotation = _targetTransform.rotation;
            _initialPosition = transform.position;
        }

        public void OnInteracted(int playerId)
        {
            if (!_isPlayerOnHandle ) PlayerGettingOnHandle(playerId);
            else PlayerGettingOffHandle(playerId);
        }

        private void PlayerGettingOffHandle(int playerId)
        {
            _isPlayerOnHandle = false;
            _interactableItem.SetDisplayMessage(_grabDisplayMessage);
            PlayerManager.Instance.UnlockPlayerInput(playerId);
            DetachingFromPlayer();
            OnUnlockPlayer = null;
            _doorsManager.OnPlayerHandleInteraction(isPlayerOnHandler:false);
            StartCoroutine(CloseDoorAnimation());
        }
        
        private void PlayerGettingOnHandle(int playerId)
        {
            _isPlayerOnHandle = true;
            _interactableItem.SetDisplayMessage(_releaseDisplayMessage);
            PlayerManager.Instance.LockPlayerInput(playerId);
            StartCoroutine(MovePlayerAnimation(playerId));
        }

        private void DetachingFromPlayer()
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
            OnUnlockPlayer?.Invoke();
        }
    
        IEnumerator CloseDoorAnimation()
        {
            float time = 0;

            Vector3 leftAtPosition = transform.position;

            while (time <= _animTime)
            {
                transform.position = Vector3.Lerp(leftAtPosition, _initialPosition, time / _animTime);

                yield return null;
                time += Time.deltaTime;
            }
        }

        IEnumerator MovePlayerAnimation(int playerId)
        {
            PlayerBase player = PlayerManager.Instance.GetPlayer(playerId);
            int cameraRootChildIndex = 1;
            float time = 0;

            Transform playerTransform = player.transform;
            Transform playerCameraRoot = player.transform.GetChild(cameraRootChildIndex);

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
            _doorsManager.OnPlayerHandleInteraction(isPlayerOnHandler: true);
        }
    }
}

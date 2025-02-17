using MessengerSystem;
using System.Collections;
using UnityEngine;

namespace Interactions
{
    public class DoorHandle : MonoBehaviour
    {
        [SerializeField] private Door _doorsManager;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private ArticulationBody _handleArticulation;
        [SerializeField] private float _animTime;

        private InteractableItem _interactableItem;
        private Vector3 _targetPosition, _initialPosition;
        private Quaternion _targetRotation;
        private bool _isPlayerOnHandle;
        private int _playerHolding;

        private void Start()
        {
            _interactableItem = GetComponent<InteractableItem>();
            _interactableItem.SetInteractAction(OnInteracted);
            _targetRotation = _targetTransform.rotation;
            _initialPosition = transform.position;
        }

        public void CloseDoor() { StartCoroutine(CloseDoorAnimation());}

        public void OnInteracted(int playerId)
        {
            if (!_isPlayerOnHandle ) PlayerGettingOnHandle(playerId);
            else PlayerGettingOffHandle(playerId);
        }

        private void PlayerGettingOffHandle(int playerId)
        {
            _isPlayerOnHandle = false;
            PlayerManager.Instance.LockPlayerInput(playerId);
            Deattaching(playerId);
            _doorsManager.OnPlayerHandleInteraction(isPlayerOnHandler:false);
        }

        public void Deattaching(int playerId)
        {
            PlayerBase player = PlayerManager.Instance.GetPlayer(playerId);
            PlayerArticulations playerArticulations = player.GetComponent<PlayerArticulations>();
            _handleArticulation.transform.parent = null;
        }

        public void DoorCanMove() { _handleArticulation.immovable = false; }

        public void DoorCannotMove() { _handleArticulation.immovable = true; }

        private void AttachingToPlayer(int playerId)
        {
            PlayerArticulations playerArticulations = PlayerManager.Instance.GetPlayer(playerId).GetComponent<PlayerArticulations>();
            _handleArticulation.transform.parent = playerArticulations.PlayerArticulation.transform;
            _handleArticulation.SnapAnchorToClosestContact();
            _handleArticulation.jointType = ArticulationJointType.PrismaticJoint;
            PlayerManager.Instance.UnlockPlayerInput(playerId);
        }

        private void PlayerGettingOnHandle(int playerId)
        {
            _isPlayerOnHandle = true;
            PlayerManager.Instance.LockPlayerInput(playerId);
            StartCoroutine(MovePlayerAnimation(playerId));
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
                Debug.Log(player.transform.position);
                player.transform.rotation = Quaternion.Slerp(playerOriginalRotation, _targetRotation, time / _animTime);

                yield return null;
                time += Time.deltaTime;
            }

            AttachingToPlayer(playerId);
            _doorsManager.OnPlayerHandleInteraction(isPlayerOnHandler: true);
        }
    }
}

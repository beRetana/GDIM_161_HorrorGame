using MessengerSystem;
using System.Collections;
using UnityEngine;

namespace Interactions
{
    public class DoorHandle : MonoBehaviour
    {
        [SerializeField] private float _animTime;
        [SerializeField] private Transform _targetTransform;

        private InteractableItem _interactableItem;
        private PlayerManager _playerManager;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;

        private void Start()
        {
            _interactableItem = GetComponent<InteractableItem>();
            _interactableItem.SetInteractAction(OnInteracted);
            _playerManager = DataMessenger.GetGameObject(MessengerKeys.GameObjectKey.PlayerManager).GetComponent<PlayerManager>();
            _targetRotation = _targetTransform.rotation;
        }

        public void OnInteracted(int playerId)
        {
            _playerManager.LockPlayerInput(playerId);

            StartCoroutine(MovePlayer(playerId));
            // Move player to location

            // Notify Door
        }

        IEnumerator MovePlayer(int playerId)
        {
            PlayerBase player = _playerManager.GetPlayer(playerId);
            int cameraRootChildIndex = 1;
            float time = 0;

            Transform playerTransform = player.transform;
            Transform playerCameraRoot = player.transform.GetChild(cameraRootChildIndex);

            Vector3 playerOriginalPosition = player.transform.position;
            _targetPosition = new Vector3(_targetTransform.position.x, player.transform.position.y, _targetTransform.position.z);

            Quaternion playerOriginalRotation = player.transform.rotation;

            while (time <= _animTime)
            {
                player.transform.position = Vector3.Lerp(playerOriginalPosition, _targetPosition, time /_animTime);
                player.transform.rotation = Quaternion.Slerp(playerOriginalRotation, _targetRotation, time / _animTime);

                
                yield return null;
                time += Time.deltaTime;
            }
        }
    }
}

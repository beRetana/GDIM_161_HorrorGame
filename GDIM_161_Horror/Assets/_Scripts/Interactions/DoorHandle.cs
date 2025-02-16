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

        private void Start()
        {
            _interactableItem = GetComponent<InteractableItem>();
            _interactableItem.SetInteractAction(OnInteracted);
            _playerManager = DataMessenger.GetGameObject(MessengerKeys.GameObjectKey.PlayerManager).GetComponent<PlayerManager>();
            _targetPosition = new Vector3(_targetTransform.position.x, 0f, _targetTransform.position.z);
        }

        public void OnInteracted(int playerID)
        {
            // Disable player movement 
            PlayerBase playerBase = _playerManager.GetPlayer(playerID);
            playerBase.EnterState(PlayerBase.PlayerStateEnum.Locked);
            Debug.Log($"Player {playerID} has interacted with handle");

            StartCoroutine(MovePlayer(playerBase.gameObject));
            // Move player to location

            // Notify Door
        }

        IEnumerator MovePlayer(GameObject player)
        {
            int cameraRootChildIndex = 1;
            float time = 0;

            Transform playerCameraRoot = player.transform.GetChild(cameraRootChildIndex);
            Vector3 playerOriginalPosition = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
            Vector3 cameraRootRotationY = new Vector3(0, playerCameraRoot.transform.rotation.y, 0f);
            Vector3 playerRotationX = new Vector3(player.transform.rotation.x, 0f, 0f);

            while (time <= _animTime)
            {
                player.transform.position = Vector3.Lerp(playerOriginalPosition, _targetPosition, time /_animTime);
                //playerCameraRoot.rotation = Vector3.Lerp()
                time += Time.deltaTime;

                yield return null;
            }
        }
    }
}

using MessengerSystem;
using UnityEngine;

namespace Interactions
{
    public class DoorHandle : MonoBehaviour
    {
        private InteractableItem _interactableItem;
        private PlayerManager _playerManager;

        private void Start()
        {
            _interactableItem = GetComponent<InteractableItem>();
            _interactableItem.SetInteractAction(OnInteracted);
            _playerManager = DataMessenger.GetGameObject(MessengerKeys.GameObjectKey.PlayerManager).GetComponent<PlayerManager>();
        }

        public void OnInteracted(int playerID)
        {
            // Disable player movement 
            Debug.Log($"Player {playerID} has interacted with handle");
            // Move player to location

            // Notify Door
        }
    }
}

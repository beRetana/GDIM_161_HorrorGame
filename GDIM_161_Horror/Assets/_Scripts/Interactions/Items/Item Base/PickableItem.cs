using MessengerSystem;
using UnityEngine;

namespace Interactions
{
    /// <summary>
    /// This is a base class for items that can be picked up. It uses interactable items.
    /// </summary>
    [RequireComponent(typeof(InteractableItem))]
    public class PickableItem : MonoBehaviour
    {
        private InteractableItem _interactableItem;
        private PlayerManager _playerManager;

        void Start()
        {
            _interactableItem = GetComponent<InteractableItem>();
            _playerManager = DataMessenger.GetGameObject(MessengerKeys.GameObjectKey.PlayerManager).GetComponent<PlayerManager>();
            _interactableItem.SetInteractAction(PickItem);
        }

        protected virtual void PickItem(int playerId)
        {
            _playerManager.GetPlayer(playerId).GetComponent<HandInventory>().PickUpItem(this);
        }

        public virtual void UseItem(int playerId) { }
    }
}

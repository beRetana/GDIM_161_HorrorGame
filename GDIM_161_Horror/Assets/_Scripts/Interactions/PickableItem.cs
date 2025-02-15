using UnityEngine;

namespace Interactions
{
    public class PickableItem : InteractableItem
    {
        public override void Interact(int playerID)
        {
            _playerManager.GetPlayer(playerID).gameObject.GetComponent<HandInventory>().PickUpItem(transform.parent.gameObject);
        }
    }
}

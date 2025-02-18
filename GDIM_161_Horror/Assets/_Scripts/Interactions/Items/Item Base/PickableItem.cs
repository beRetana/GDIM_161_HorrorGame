using UnityEngine;

namespace Interactions
{
    /// <summary>
    /// This is a base class for items that can be picked up. It uses interactable items.
    /// </summary>
    [RequireComponent(typeof(InteractableItem))]
    public class PickableItem : MonoBehaviour
    {
        protected InteractableItem _interactableItem;
        public bool IsPossessed {  get; private set; } // Held in Hand || Moving to Hand

        protected virtual void Start()
        {
            _interactableItem = GetComponent<InteractableItem>();
            _interactableItem.SetInteractAction(PickItem);
        }

        public override string ToString()
        {
            return $"Item: {this.name}";
        }

        protected virtual void PickItem(int playerId) // <= (InteractableItem)this.Interact()
        {
            if (IsPossessed)
            {
                Debug.Log($"tried PICK UP on {this}, but is already possessed");
                return;
            }
            bool success = PlayerManager.Instance.GetPlayer(playerId).GetComponent<HandInventory>().PickUpItem(this);
            if (!success) return;

            SetPossessed(true);
        }

        public virtual void UnPossessItem()
        {
            SetPossessed(false);
        }

        protected virtual void SetPossessed(bool toPossess)
        {
            IsPossessed = toPossess;
            _interactableItem.SetIntactive(!toPossess);
        }

        public virtual void UseItem(int playerId) { }

    }
}

// enum with flags if u want it
/*[Flags]
public enum PickableItemStateEnum
{
    None = 0,               //000
    IsPossessed = 1 << 0,   //001
    IsInHand = 1 << 1       //010
}
public PickableItemStateEnum itemStateEnum = PickableItemStateEnum.None;

private void SetPossessed(bool isPossessed)
{
    if (isPossessed)
    {
        itemStateEnum |= PickableItemStateEnum.IsPossessed;
    }
    else
    {
        itemStateEnum &= ~PickableItemStateEnum.IsPossessed;
        itemStateEnum &= ~PickableItemStateEnum.IsInHand;
    }
}
private void SetInHand(bool isInHand)
{
    if (isInHand)
    {
        itemStateEnum |= PickableItemStateEnum.IsInHand;
        itemStateEnum &= ~PickableItemStateEnum.IsPossessed;
    }
    else
    {
        itemStateEnum &= ~PickableItemStateEnum.IsInHand;
    }
}*/
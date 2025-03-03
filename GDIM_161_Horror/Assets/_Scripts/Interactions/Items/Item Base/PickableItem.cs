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
        public int OwnerPlayerID { get; private set; }

        protected virtual void Start()
        {
            _interactableItem = GetComponent<InteractableItem>();
            _interactableItem.SetInteractAction(PickItem);
        }

        public override string ToString()
        {
            return $"Item: {this.name}";
        }

        protected virtual void PickItem(int playerID) // <= (InteractableItem)this.Interact()
        {
            if (IsPossessed)
            {
                Debug.Log($"tried PICK UP on {this}, but is already possessed");
                return;
            }

            bool success = PlayerManager.Instance.GetPlayer(playerID).GetComponent<HandInventory>().PickUpItem(this);
            if (!success) return;

            SetPossessed(true, playerID);
        }

        public virtual void UnPossessItem()
        {
            SetPossessed(false);
        }

        protected virtual void SetPossessed(bool toPossess, int playerID = 0)
        {
            Debug.Log($"Player {playerID} {(toPossess ? "posessing" : "forfeiting")} {this.name}");
            IsPossessed = toPossess;
            OwnerPlayerID = playerID;
            _interactableItem.SetInteractive(!toPossess);
        }

        public virtual void UseItem(int playerID) { }


        public virtual void OrientItemInHand(bool isLeftHand) 
        {
            Transform parentTransform = transform.parent.transform;
            parentTransform.localPosition = Vector3.zero;
            parentTransform.localEulerAngles = new Vector3(0f, -270f, 0f);
        }
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
using Mirror;
using UnityEngine;

namespace Interactions
{
    /// <summary>
    /// This is a base class for items that can be picked up. It uses interactable items.
    /// </summary>
    [RequireComponent(typeof(InteractableItem))]
    public class PickableItem : MonoBehaviour
    {
        [SerializeField] private PickableItemSO _pickableItemSO;
        [SerializeField] private ItemType _itemType;
        
        protected InteractableItem _interactableItem;

        public PickableItemSO PickableItemSO { get { return _pickableItemSO; } }

        public ItemType ItemType { get { return _itemType; } }
        public bool IsPossessed {  get; private set; } // Held in Hand || Moving to Hand
        public int OwnerPlayerID { get; private set; }

        public override string ToString()
        {
            return $"Item: {this.name}";
        }

        protected virtual void Start()
        {
            _interactableItem = GetComponent<InteractableItem>();
        }

        public virtual void UseItem(int playerID) { }

        public virtual void OrientItemInHand(bool isLeftHand) 
        {
            if (isLeftHand) SetRotationLocation(_pickableItemSO.LeftHandPosition, _pickableItemSO.LeftHandRotation);
            else SetRotationLocation(_pickableItemSO.RightHandPosition, _pickableItemSO.RightHandRotation);
        }

        protected virtual void SetRotationLocation(Vector3 location, Vector3 rotation)
        {
            transform.parent.transform.localPosition = location;
            transform.parent.transform.localEulerAngles = rotation;
        }
    }
}
using System.Collections;
using Interactions;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

/// <summary>
/// Allows the Player to interact with other items and store them in two slots.
/// </summary>
public class HandInventory : NetworkBehaviour
{
    [Header("General Settings")]
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private Transform _rightHandTransform; // hands
    [SerializeField] private Transform _leftHandTransform;
    [SerializeField] private Transform _rightHandSocket; // item slots
    [SerializeField] private Transform _leftHandSocket;

    [Header("Arms")]
    [SerializeField] private Arms _arms;

    [Header("Interaction Physics Settings")]
    [SerializeField] private float _pickUpRange;
    [SerializeField] private float _pickUpForce;
    [SerializeField] private float _linearDrag;
    [SerializeField] private float _throwForce;
    [SerializeField] private MouseUI _mouse;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private PickableItemSO _sonnar;
    [SerializeField] private PickableItemSO _torch;

    [Header("Debugging")]
    [SerializeField] private bool _enableDebugging;

    public Arms GetArms() { return _arms; }

    private class InventorySlot
    {
        private NetworkPickableItem _pickableItem;
        private Rigidbody _itemRigidBody;
        private Transform _itemTransform;
        private bool _isDominant;

        public NetworkPickableItem Item { get { return _pickableItem; } set { _pickableItem = value; } }

        public Rigidbody ItemRigidBody { get { return _itemRigidBody; } set { _itemRigidBody = value; } }

        public Transform ItemTransform { get { return _itemTransform; } set { _itemTransform = value; } }
        public bool IsDominant { get { return _isDominant; } set { _isDominant = value; } }

        public void SetRigidBody(Rigidbody rigidBody, float linearDrag)
        {
            _itemRigidBody = rigidBody;
            // No more physics
            ItemRigidBody.isKinematic = true; 
            ItemRigidBody.useGravity = false;
            ItemRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public Rigidbody RemoveRigidBody()
        {
            Rigidbody rigidBodyToDrop = ItemRigidBody;

            // Re-eanble physics
            ItemRigidBody.isKinematic = false;
            ItemRigidBody.useGravity = true;
            ItemRigidBody.freezeRotation = false;
            _itemRigidBody = null;

            return rigidBodyToDrop;
        }

        public void SetDominant(bool isDom)
        {
            _isDominant = isDom;
        }
    }

    private class InventorySlots
    {
        private InventorySlot L_HandSlot; // 0
        private InventorySlot R_HandSlot; // 1
        public bool IsLHandDom { get; private set; }
        public InventorySlots()
        {
            L_HandSlot = new();
            L_HandSlot.Item = null;
            R_HandSlot = new();
            R_HandSlot.Item = null;
            SetLeftHandDominant(false);
        }

        public override string ToString()
        {
            return $"L: {(IsLHandDom ? "DOM" : "off")}, Item: {((L_HandSlot.Item != null) ? L_HandSlot.Item : "N/A")}   |   " +
                $"R: {(IsLHandDom ? "off" : "DOM")}, Item: {((R_HandSlot.Item != null) ? R_HandSlot.Item : "N/A")}";
        }

        private void SetLeftHandDominant(bool isLHandDom)
        {
            L_HandSlot.SetDominant(isLHandDom);
            R_HandSlot.SetDominant(!isLHandDom);
            IsLHandDom = isLHandDom;
            Debug.Log(this);
        }

        // Indexing
        public InventorySlot this[int index]
        {
            get
            {
                if (index == 0) return L_HandSlot;
                else if (index == 1) return R_HandSlot;
                else throw new System.Exception("Invalid item slot index");
            }
            private set
            {
                if (index == 0) L_HandSlot = value;
                else if (index == 1) R_HandSlot = value;
                else throw new System.Exception("Invalid item slot index");
            }
        }

        /// <summary>
        /// Sets the item passed as the value item for the (dominant hand has priority) Inventory slot.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public InventorySlot AddItemToSlot(NetworkPickableItem item)
        {
           
            InventorySlot selectedHand = GetDominantHand();
            
            if (selectedHand.Item == null)
            {
                selectedHand.Item = item;
                Debug.Log($"Item placed in DOM hand: {(IsLHandDom ? "L" : "R")}");
                return selectedHand;
            }
            selectedHand = GetOffHand();
            if (selectedHand.Item == null)
            {
                selectedHand.Item = item;
                Debug.Log($"Item placed in OFF hand, {(IsLHandDom ? "R" : "L")}");
                return selectedHand;
            }
            return null;
        }

        public InventorySlot RemoveItem(Vector3 throwDir)
        {
            InventorySlot inventorySlotToRemove = GetDominantHand();
            if (inventorySlotToRemove.Item == null) return null;
            inventorySlotToRemove.Item.UnPossessItem();
            inventorySlotToRemove.Item = null;
            Rigidbody temp = inventorySlotToRemove.RemoveRigidBody();
            temp.AddForce(throwDir, ForceMode.Impulse);

            return inventorySlotToRemove;
        }

        public bool SwapDominance()
        {
            SetLeftHandDominant(!IsLHandDom);
            return IsLHandDom;
        }

        public InventorySlot GetDominantHand()
        {
            Debug.Log($"getting DOM hand, {(IsLHandDom ? "L" : "R")}");
            return this[IsLHandDom ? 0 : 1];
        }

        public InventorySlot GetOffHand()
        {
            Debug.Log($"getting OFF hand, {(IsLHandDom ? "R" : "L")}");
            return this[IsLHandDom ? 1 : 0];
        }

        public int GetDominantIndex() { return IsLHandDom ? 0 : 1; }
    }

    private InventorySlots _inventorySlots = new();
    private int _playerID;
    private IInteractable _interactableComponent;

    private const int _LEFT_HAND_ID = 0;
    private const int _RIGHT_HAND_ID = 1;

    void Start()
    {
        if (gameObject.TryGetComponent<PlayerBase>(out PlayerBase playerBase)) _playerID = playerBase.ID();
        SetHandTransforms();
    }

    private void SetHandTransforms()
    {
        _inventorySlots[_LEFT_HAND_ID].ItemTransform = _leftHandTransform;
        _inventorySlots[_RIGHT_HAND_ID].ItemTransform = _rightHandTransform;
    }

    private void Update()
    {
        CheckForRaycastInteractables();
    }

    /// <summary>
    /// Detects interactable items to display options on interacting with that object
    /// </summary>
    private void CheckForRaycastInteractables()
    {
        Ray rayToInteract = _playerCamera.ViewportPointToRay(new Vector3(0.5f,0.5f, 0));

        // If we hit something in the layer.
        if (Physics.Raycast(rayToInteract, out RaycastHit hitInfo, _pickUpRange, _interactableLayer))
        {
            IInteractable childCanvas = hitInfo.transform.GetComponentInChildren<IInteractable>();

            // If we didn't hit something before.
            if (_interactableComponent == null || _interactableComponent.Equals(null))
            {
                // Report it as detected
                _interactableComponent = childCanvas;
                _interactableComponent.Detected(_playerID);
                _mouse?.InteractionEffect();
            } 
            // If we are hitting a different object than before.
            else if (childCanvas != _interactableComponent)
            {
                // Stop animation and start the new one
                _interactableComponent?.StoppedDetecting(_playerID);
                _interactableComponent = childCanvas;
                _interactableComponent?.Detected(_playerID);
            }
        }
        // If we didn't hit anything did we hit something before?
        else if (_interactableComponent != null && !_interactableComponent.Equals(null))
        {
            _interactableComponent.StoppedDetecting(_playerID);
            _mouse?.DefaultEffect();
            _interactableComponent = null;
        }
    }

    public void OnSwap(InputValue value) 
    { 
        bool isLHandDom = _inventorySlots.SwapDominance();
        _arms.SetHandDominancePosition(isLHandDom, !isLHandDom);
    }

    public void OnInteract(InputValue value) 
    {
        if (_inventorySlots[_LEFT_HAND_ID].Item == null || _inventorySlots[_RIGHT_HAND_ID].Item == null)
        {
            _interactableComponent?.Interact(_playerID);
            _interactableComponent = null;
        }
    }

    public void OnDrop(InputValue value) { DropItem(); }

    public void OnThrow(InputValue value) { DropItem(_throwForce); }

    public void OnUseItem(InputValue value) { UseItem(); }

    public void UseItem()
    {
        InventorySlot inventorySlotToUse = _inventorySlots.GetDominantHand();
        NetworkPickableItem itemToUse = inventorySlotToUse?.Item;
        if (itemToUse == null) return;

        itemToUse.UseItem(_playerID);
    }

    public bool PickUpItem(NetworkPickableItem pickableItem)
    {
        InventorySlot handSlot = _inventorySlots.AddItemToSlot(pickableItem);

        if (handSlot == null) return false;

        bool isLeftHandAction = (_inventorySlots.GetDominantIndex() == _LEFT_HAND_ID) ^ (!handSlot.IsDominant);
        handSlot.SetRigidBody(pickableItem.transform.parent.transform.GetComponent<Rigidbody>(), _linearDrag);
        pickableItem.OrientItemInHand(handSlot.ItemTransform, _playerID);

        Debugger($"Player Is placing item: {pickableItem.name} in: {(isLeftHandAction ? "Left" : "Right")} Hand");
        _interactableComponent = null;

        return true;
    }

    private void DropItem(float throwForce = 0)
    {
        _inventorySlots.RemoveItem(transform.forward * throwForce);
    }

    private void Debugger(string log) 
    { 
        if (_enableDebugging) Debug.Log(log);
    }
}

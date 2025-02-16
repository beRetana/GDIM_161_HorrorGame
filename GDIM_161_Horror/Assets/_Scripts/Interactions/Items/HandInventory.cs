using System.Collections;
using Interactions;
using MessengerSystem;
using Mono.CSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Allows the Player to interact with other items and store them in two slots.
/// </summary>
public class HandInventory : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private Transform _leftHandTransform;

    [Header("Interaction Physics Settings")]
    [SerializeField] private float _pickUpRange;
    [SerializeField] private float _pickUpForce;
    [SerializeField] private float _linearDrag;
    [SerializeField] private float _throwForce;

    private class InventorySlot
    {
        private PickableItem _pickableItem;
        private Rigidbody _itemRigidBody;
        private Transform _itemTransform;
        private float _initialLinearDamping;

        public PickableItem Item { get { return _pickableItem; } set { _pickableItem = value; } }

        public Rigidbody ItemRigidBody { get { return _itemRigidBody; } set { _itemRigidBody = value; } }

        public Transform ItemTransform { get { return _itemTransform; } set { _itemTransform = value; } }

        public void SetRigidBody(Rigidbody rigidBody, float linearDrag)
        {
            _itemRigidBody = rigidBody;
            ItemRigidBody.useGravity = false;
            _initialLinearDamping = rigidBody.linearDamping;
            ItemRigidBody.linearDamping = linearDrag;
            ItemRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            ItemRigidBody.transform.parent = ItemTransform;
        }

        public Rigidbody RemoveRigidBody()
        {
            Rigidbody rigidBodyToDrop = ItemRigidBody;
            ItemRigidBody.useGravity = true;
            ItemRigidBody.linearDamping = _initialLinearDamping;
            ItemRigidBody.freezeRotation = false;
            ItemRigidBody.transform.parent = null;
            _itemRigidBody = null;

            return rigidBodyToDrop;
        }
    }

    private class InventorySlots
    {
        private InventorySlot mainHand = new();
        private InventorySlot offHand = new();

        public InventorySlot this[int index]
        {
            get{
                if (index == 0) return mainHand;
                else if (index == 1) return offHand;
                else throw new System.Exception("Invalid item slot index");
            }
            private set{
                if (index == 0) mainHand = value;
                else if (index == 1) offHand = value;
                else throw new System.Exception("Invalid item slot index");
            }
        }

        public InventorySlot GainItem(PickableItem inventorySlotToGain)
        {
            if (mainHand.Item == null) { mainHand.Item = inventorySlotToGain; return mainHand; }
            if (offHand.Item == null) { offHand.Item = inventorySlotToGain; return offHand; }
            return null;
        }

        public InventorySlot RemoveItem()
        {
            if (mainHand.Item == null) return null;
            InventorySlot inventorySlotToRemove = mainHand;

            mainHand.Item = null;
            mainHand.RemoveRigidBody();

            return inventorySlotToRemove;
        }

        public void SwapItems()
        {
            if (mainHand.Item == null && offHand.Item == null) return;

            InventorySlot tempSlot = mainHand;
            mainHand = offHand;
            offHand = tempSlot;
        }
    }

    private InventorySlots _inventorySlots = new();
    private PlayerControls _playerControls;
    private MouseUI _mouse;
    private int _playerID;
    private IInteractable _interactableComponent;

    private const int _MAIN_HAND_ID = 0;
    private const int _OFF_HAND_ID = 1;

    void Awake() 
    {
        _playerControls = new();
        OnEnable();
    }

    void Start()
    {
        _mouse = DataMessenger.GetGameObject(MessengerKeys.GameObjectKey.MouseUI).GetComponent<MouseUI>();
        _playerID = gameObject.GetComponent<PlayerBase>().ID();
        PrepareList();
        DataMessenger.SetGameObject($"Player{_playerID}", gameObject);
        Debug.Log($"{this.name} is attatched to PlayerID:{_playerID}");
    }

    void OnEnable()
    {
        _playerControls.Player.Interact.Enable();
        _playerControls.Player.Drop.Enable();
        _playerControls.Player.Throw.Enable();
        _playerControls.Player.Swap.Enable();
        _playerControls.Player.UseItem.Enable();

        _playerControls.Player.Interact.performed += OnRaycastInteract;
        _playerControls.Player.Drop.performed += OnItemDrop;
        _playerControls.Player.Throw.performed += OnItemThrow;
        _playerControls.Player.Swap.performed += OnHandSwap;
        _playerControls.Player.UseItem.performed += OnUseItem;
    }

    void OnDisable(){
        _playerControls.Player.Interact.performed -= OnRaycastInteract;
        _playerControls.Player.Drop.performed -= OnItemDrop;
        _playerControls.Player.Throw.performed -= OnItemThrow;
        _playerControls.Player.Swap.performed -= OnHandSwap;
        _playerControls.Player.UseItem.performed -= OnUseItem;

        _playerControls.Player.Interact.Disable();
        _playerControls.Player.Drop.Disable();
        _playerControls.Player.Throw.Disable();
        _playerControls.Player.Swap.Disable();
        _playerControls.Player.UseItem.Disable();
    }

    void PrepareList()
    {
        _inventorySlots[_MAIN_HAND_ID].ItemTransform = _rightHandTransform;
        _inventorySlots[_OFF_HAND_ID].ItemTransform = _leftHandTransform;
    }

    void Update()
    {
        CheckForRaycastInteractables();
    }

    private void FixedUpdate()
    {
        MoveItemsPositionsToHands();
    }

    private void CheckForRaycastInteractables()
    {
        Ray rayToInteract = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f, 0));

        // If we hit something in the layer.
        if (Physics.Raycast(rayToInteract, out RaycastHit hitInfo, _pickUpRange, _interactableLayer))
        {
            IInteractable childCanvas = hitInfo.transform.GetComponentInChildren<IInteractable>();

            // If we didn't hit something before.
            if (_interactableComponent == null)
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
                _interactableComponent.StoppedDetecting(_playerID);
                _interactableComponent = childCanvas;
                _interactableComponent?.Detected(_playerID);
            }
        }
        // If we didn't hit anything did we hit something before?
        else if (_interactableComponent != null)
        {
            _interactableComponent.StoppedDetecting(_playerID);
            _mouse?.DefaultEffect();
            _interactableComponent = null;
        }
    }

    void OnHandSwap(InputAction.CallbackContext context) { _inventorySlots.SwapItems(); }
    void OnRaycastInteract(InputAction.CallbackContext context) 
    { 
        if (_inventorySlots[_MAIN_HAND_ID].Item == null || _inventorySlots[_OFF_HAND_ID].Item == null) 
            _interactableComponent?.Interact(_playerID);
    }
    void OnItemDrop(InputAction.CallbackContext context) { DropItem(); }
    void OnItemThrow(InputAction.CallbackContext context) { DropItem(_throwForce); }
    void OnUseItem(InputAction.CallbackContext context) { UseItem(); }

    private void UseItem()
    {
        InventorySlot inventorySlotToUse = _inventorySlots[0];
        PickableItem itemToUse = inventorySlotToUse?.Item;
        if (itemToUse == null) return;

        itemToUse.UseItem(_playerID);
        // TK interact functionality
    }

    public void PickUpItem(PickableItem pickableItem)
    {
        Transform pickableParent = pickableItem.transform.parent;

        if (pickableParent.GetComponent<Rigidbody>() == null) return; // DropItem(); <-??

        InventorySlot inventorySlotOfNewItem = _inventorySlots.GainItem(pickableItem);
        if (inventorySlotOfNewItem == null) return;

        inventorySlotOfNewItem.SetRigidBody(pickableParent.GetComponent<Rigidbody>(), _linearDrag);

        Physics.IgnoreCollision(pickableParent.GetComponent<Collider>(),
                                gameObject.GetComponentInChildren<CapsuleCollider>(), true);

        StartCoroutine(AnimateRotationTowards(pickableParent, inventorySlotOfNewItem.ItemTransform.rotation));
    }

    private void MoveItemsPositionsToHands()
    {
        MoveItemPositionToHand(_inventorySlots[_MAIN_HAND_ID]);
        MoveItemPositionToHand(_inventorySlots[_OFF_HAND_ID]);
    }

    private void MoveItemPositionToHand(InventorySlot inventorySlot)
    {
        if (inventorySlot.Item == null) return;

        Vector3 direction = inventorySlot.ItemRigidBody.transform.parent.position - inventorySlot.ItemRigidBody.position;
        if (Vector3.Magnitude(direction) <= .1f) return;

        inventorySlot.ItemRigidBody.AddForce(direction * _pickUpForce);
    }

    private IEnumerator AnimateRotationTowards(Transform target, Quaternion rotation, float duration = 1f)
    {
        float timer = 0f;
        Quaternion start = target.rotation;
        while(timer < duration)
        {
            target.rotation = Quaternion.Slerp(start, rotation, timer / duration);
            yield return null;
            timer += Time.deltaTime;
        }
        target.rotation = rotation;
    }

    private void DropItem(float throwForce = 0){

        if (_inventorySlots[_MAIN_HAND_ID].Item == null) return;

        Rigidbody itemRigidBodyToDrop = _inventorySlots[_MAIN_HAND_ID].ItemRigidBody;
        Physics.IgnoreCollision(itemRigidBodyToDrop.transform.GetComponent<Collider>(),
                                gameObject.GetComponentInChildren<CapsuleCollider>(), false);
        
        InventorySlot itemSlotToDrop = _inventorySlots.RemoveItem();
        if(itemSlotToDrop == null) return;

        itemRigidBodyToDrop.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }
}

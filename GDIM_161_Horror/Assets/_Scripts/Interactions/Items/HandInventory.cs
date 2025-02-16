using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.GameUI;
using MessengerSystem;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Allows the Player to interact with other items and store them in two slots.
/// </summary>
public class HandInventory : MonoBehaviour
{
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private Transform _leftHandTransform;

    private class InventorySlot
    {
        public InteractableObject interactableObject;
        public Rigidbody rigidbody;
        public Transform transform;
        public float initialLinearDamping;
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

        public InventorySlot GainItem(InteractableObject inventorySlotToGain)
        {
            if (mainHand.interactableObject == null) { mainHand.interactableObject = inventorySlotToGain; return mainHand; }
            if (offHand.interactableObject == null) { offHand.interactableObject = inventorySlotToGain; return offHand; }
            return null;
        }

        public InventorySlot RemoveItem()
        {
            if (mainHand.interactableObject == null) return null;
            InventorySlot inventorySlotToRemove = mainHand;

            mainHand.interactableObject = offHand.interactableObject;
            offHand.interactableObject = null;

            return inventorySlotToRemove;
        }

        public void SwapItems()
        {
            if (mainHand.interactableObject == null || offHand.interactableObject == null) return;
            
            InteractableObject temp = mainHand.interactableObject;
            mainHand.interactableObject = offHand.interactableObject;
            offHand.interactableObject = temp;
        }
    }

    private InventorySlots _inventorySlots = new();
    private PlayerControls _playerControls;
    private MouseUI _mouse;
    private int _playerID;
    private IInteractable _interactableComponent;

    private const int _MAIN_HAND_ID = 0;
    private const int _OFF_HAND_ID = 1;

    [Header("Interaction Physics Settings")]

    [SerializeField] private float _pickUpRange;
    [SerializeField] private float _pickUpForce;
    [SerializeField] private float _linearDrag;
    [SerializeField] private float _throwForce;

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
        _inventorySlots[_MAIN_HAND_ID].transform = _rightHandTransform;
        _inventorySlots[_OFF_HAND_ID].transform = _leftHandTransform;
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
    void OnRaycastInteract(InputAction.CallbackContext context) { _interactableComponent?.Interact(_playerID);}
    void OnItemDrop(InputAction.CallbackContext context) { DropItem(); }
    void OnItemThrow(InputAction.CallbackContext context) { DropItem(_throwForce); }
    void OnUseItem(InputAction.CallbackContext context) { UseItem(); }

    private void UseItem()
    {
        InventorySlot inventorySlotToUse = _inventorySlots[0];
        InteractableObject itemToUse = inventorySlotToUse?.interactableObject;
        if (itemToUse == null) return;

        itemToUse.Interact(_playerID);
        // TK interact functionality
    }

    public void PickUpItem(InteractableObject pickableItem)
    {
        //int freeHandIndex = GetFreeHands();

        if (pickableItem.GetComponent<Rigidbody>() == null) return; // DropItem(); <-??

        InventorySlot inventorySlotOfNewItem = _inventorySlots.GainItem(pickableItem);
        if (inventorySlotOfNewItem == null) return;

        inventorySlotOfNewItem.rigidbody = pickableItem.GetComponent<Rigidbody>();
        inventorySlotOfNewItem.rigidbody.useGravity = false;
        inventorySlotOfNewItem.initialLinearDamping = inventorySlotOfNewItem.rigidbody.linearDamping;
        inventorySlotOfNewItem.rigidbody.linearDamping = _linearDrag;
        inventorySlotOfNewItem.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        inventorySlotOfNewItem.rigidbody.transform.parent = inventorySlotOfNewItem.transform;

        Physics.IgnoreCollision(inventorySlotOfNewItem.interactableObject.GetComponent<Collider>(),
                                gameObject.GetComponentInChildren<CapsuleCollider>(), true);
        //Switch this magic number later
        StartCoroutine(AnimateRotationTowards(inventorySlotOfNewItem.interactableObject.transform, inventorySlotOfNewItem.transform.rotation));
        
    }

    private void MoveItemsPositionsToHands()
    {
        MoveItemPositionToHand(_inventorySlots[_MAIN_HAND_ID]);
        MoveItemPositionToHand(_inventorySlots[_OFF_HAND_ID]);
    }

    private void MoveItemPositionToHand(InventorySlot inventorySlot)
    {
        if (inventorySlot.interactableObject == null) return;

        Vector3 direction = inventorySlot.transform.position - inventorySlot.interactableObject.transform.position;
        if (Vector3.Magnitude(direction) <= .1f) return;

        inventorySlot.rigidbody.AddForce(direction * _pickUpForce);
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
        InventorySlot itemSlotToDrop = _inventorySlots.RemoveItem();
        if(itemSlotToDrop == null) return;

        Rigidbody itemRigidBodyToDrop = itemSlotToDrop.rigidbody;

        itemRigidBodyToDrop.useGravity = true;
        itemRigidBodyToDrop.linearDamping = itemSlotToDrop.initialLinearDamping;
        itemRigidBodyToDrop.freezeRotation = false;

        Physics.IgnoreCollision(itemSlotToDrop.interactableObject.GetComponent<Collider>(), 
                                gameObject.GetComponentInChildren<CapsuleCollider>(), false);

        itemRigidBodyToDrop.transform.parent = null;
        itemRigidBodyToDrop.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        //itemRigidBodyToDrop = null;
    }
}

using System.Collections;
using Interactions;
using Player;
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

    [Header("Arms")]
    [SerializeField] private Arms _arms;

    [Header("Interaction Physics Settings")]
    [SerializeField] private float _pickUpRange;
    [SerializeField] private float _pickUpForce;
    [SerializeField] private float _linearDrag;
    [SerializeField] private float _throwForce;
    [SerializeField] private MouseUI _mouse;
    [SerializeField] private Camera _playerCamera;

    private class InventorySlot
    {
        private PickableItem _pickableItem;
        private Rigidbody _itemRigidBody;
        private Transform _itemTransform;
        private float _initialLinearDamping;
        private bool _isDominant;

        public PickableItem Item { get { return _pickableItem; } set { _pickableItem = value; } }

        public Rigidbody ItemRigidBody { get { return _itemRigidBody; } set { _itemRigidBody = value; } }

        public Transform ItemTransform { get { return _itemTransform; } set { _itemTransform = value; } }
        public bool IsDomninant { get { return _isDominant; } set { _isDominant = value; } }

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

        public InventorySlot GainItem(PickableItem inventorySlotToGain)
        {
            InventorySlot selectedHand = GetDominantHand();
            if (selectedHand.Item == null)
            {
                selectedHand.Item = inventorySlotToGain;
                Debug.Log($"Item placed in DOM hand: {(IsLHandDom ? "L" : "R")}");
                return selectedHand;
            }
            selectedHand = GetOffHand();
            if (selectedHand.Item == null)
            {
                selectedHand.Item = inventorySlotToGain;
                Debug.Log($"Item placed in OFF hand, {(IsLHandDom ? "R" : "L")}");
                return selectedHand;
            }
            return null;
        }

        public InventorySlot RemoveItem()
        {
            InventorySlot inventorySlotToRemove = GetDominantHand();
            if (inventorySlotToRemove.Item == null) return null;

            inventorySlotToRemove.Item = null;
            inventorySlotToRemove.RemoveRigidBody();

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
        _playerID = gameObject.GetComponent<PlayerBase>().ID();
        PrepareList();
    }

    void PrepareList()
    {
        _inventorySlots[_LEFT_HAND_ID].ItemTransform = _leftHandTransform;
        _inventorySlots[_RIGHT_HAND_ID].ItemTransform = _rightHandTransform;
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
        Ray rayToInteract = _playerCamera.ViewportPointToRay(new Vector3(0.5f,0.5f, 0));

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

    public void OnSwap(InputValue value) 
    { 
        bool isLHandDom = _inventorySlots.SwapDominance();
        _arms.SetHandDominancePosition(isLHandDom, !isLHandDom);
    }
    public void OnInteract(InputValue value) 
    { 
        if (_inventorySlots[_LEFT_HAND_ID].Item == null || _inventorySlots[_RIGHT_HAND_ID].Item == null) 
            _interactableComponent?.Interact(_playerID);
    }
    public void OnDrop(InputValue value) { DropItem(); }
    public void OnThrow(InputValue value) { DropItem(_throwForce); }
    public void OnUseItem(InputValue value) { UseItem(); }

    public void UseItem()
    {
        InventorySlot inventorySlotToUse = _inventorySlots[_RIGHT_HAND_ID];
        PickableItem itemToUse = inventorySlotToUse?.Item;
        if (itemToUse == null) return;

        itemToUse.UseItem(_playerID);
        // TK interact functionality
    }

    public bool PickUpItem(PickableItem pickableItem)
    {
        Transform pickableParent = pickableItem.transform.parent;

        if (pickableParent.GetComponent<Rigidbody>() == null) return false; // DropItem(); <-??

        InventorySlot inventorySlotOfNewItem = _inventorySlots.GainItem(pickableItem);
        if (inventorySlotOfNewItem == null) return false;

        inventorySlotOfNewItem.SetRigidBody(pickableParent.GetComponent<Rigidbody>(), _linearDrag);

        Physics.IgnoreCollision(pickableParent.GetComponent<Collider>(),
                                gameObject.GetComponentInChildren<CapsuleCollider>(), true);

        StartCoroutine(AnimateRotationTowards(pickableParent, inventorySlotOfNewItem.ItemTransform.rotation));

        _arms.OutStretchHand(_inventorySlots.IsLHandDom);

        return true;
    }

    private void MoveItemsPositionsToHands()
    {
        MoveItemPositionToHand(_inventorySlots[_LEFT_HAND_ID]);
        MoveItemPositionToHand(_inventorySlots[_RIGHT_HAND_ID]);
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
        InventorySlot dominantSlot = _inventorySlots.GetDominantHand();
        if (dominantSlot.Item == null) return;

        PickableItem itemToDrop = dominantSlot.Item;
        Rigidbody itemRigidBodyToDrop = dominantSlot.ItemRigidBody;

        Physics.IgnoreCollision(itemRigidBodyToDrop.transform.GetComponent<Collider>(),
                                gameObject.GetComponentInChildren<CapsuleCollider>(), false);

        _inventorySlots.RemoveItem();

        itemRigidBodyToDrop.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        itemToDrop.UnPossessItem();
        _arms.OutStretchHand(_inventorySlots.IsLHandDom);
    }
}

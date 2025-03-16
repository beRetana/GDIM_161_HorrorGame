using System.Collections;
using Interactions;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Unity.VisualScripting;

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
        public bool IsDominant { get { return _isDominant; } set { _isDominant = value; } }

        public void SetRigidBody(Rigidbody rigidBody, float linearDrag)
        {
            _itemRigidBody = rigidBody;
            ItemRigidBody.isKinematic = true; // no gravity
            
            // Comment this later
            ItemRigidBody.useGravity = false;
            // Comment this later
            _initialLinearDamping = rigidBody.linearDamping;
            // Comment this later
            ItemRigidBody.linearDamping = linearDrag;
            // Comment this later
            ItemRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            // Comment this later
            ItemRigidBody.transform.parent = ItemTransform;
        }

        public Rigidbody RemoveRigidBody()
        {
            Rigidbody rigidBodyToDrop = ItemRigidBody;
            // Comment this later
            Transform pickableParent = _itemTransform.parent;

            ItemRigidBody.isKinematic = false;//useGravity = true;
            ItemTransform.SetParent(null);

            // Comment this later
            ItemRigidBody.useGravity = true;
            // Comment this later
            ItemRigidBody.linearDamping = _initialLinearDamping;
            // Comment this later
            ItemRigidBody.freezeRotation = false;
            // Comment this later
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

        public InventorySlot GainItem(NetworkPickableItem inventorySlotToGain)
        {
            InventorySlot selectedHand = GetDominantHand();
            
            if (selectedHand.Item == null)
            {
                selectedHand.Item = CreateHeldItem(inventorySlotToGain, selectedHand.ItemTransform);
                //AudioManager.instance.PlayOneShot(FMODEvents.instance.torchGrab, GameObject.FindObjectOfType<HandInventory>().transform.position);


                Debug.Log($"Item placed in DOM hand: {(IsLHandDom ? "L" : "R")}");
                return selectedHand;
            }
            selectedHand = GetOffHand();
            if (selectedHand.Item == null)
            {
                selectedHand.Item = CreateHeldItem(inventorySlotToGain, selectedHand.ItemTransform);
                Debug.Log($"Item placed in OFF hand, {(IsLHandDom ? "R" : "L")}");
                return selectedHand;
            }
            return null;
        }

        private PickableItem CreateHeldItem(NetworkPickableItem itemToDestroy, Transform selectedHand)
        {
            PickableItemSO pickableSO = itemToDestroy.PickableItemSO;
            Transform nonNetworkPrefab = Instantiate(pickableSO.Prefab, selectedHand);
            NetworkServer.Destroy(itemToDestroy.transform.parent.gameObject);
            Debug.Log("Network object Destroyed and Non-Network Created");
            return nonNetworkPrefab.GetChild(0).GetComponent<PickableItem>();
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
        //MoveItemsPositionsToHands();
    }

    private void CheckForRaycastInteractables()
    {
        Ray rayToInteract = _playerCamera.ViewportPointToRay(new Vector3(0.5f,0.5f, 0));

        // If we hit something in the layer.
        if (Physics.Raycast(rayToInteract, out RaycastHit hitInfo, _pickUpRange, _interactableLayer))
        {
            Debug.Log("DETECTED SOMETHING");
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
        PickableItem itemToUse = inventorySlotToUse?.Item;
        if (itemToUse == null) return;

        itemToUse.UseItem(_playerID);
    }

    [Command]
    public void CmdPickUpItem(NetworkPickableItem pickableItem)
    {
        InventorySlot inventorySlotOfNewItem = _inventorySlots.GainItem(pickableItem);
        
        if (inventorySlotOfNewItem == null) return;

        // TK Might have to load to server manually.

        _PutItemInHand(inventorySlotOfNewItem, inventorySlotOfNewItem.Item.transform.parent, inventorySlotOfNewItem.Item);

        _arms.HandMoveOutAndIn((_inventorySlots.GetDominantIndex() == _LEFT_HAND_ID) ^ (!inventorySlotOfNewItem.IsDominant));
        Debug.Log($"Is left Dominant {_inventorySlots.GetDominantIndex() == _LEFT_HAND_ID}");
        Debug.Log($"Is my Slot Dominant {inventorySlotOfNewItem.IsDominant}");
        Debug.Log($"Should I grab with Left {(_inventorySlots.GetDominantIndex() == _LEFT_HAND_ID) ^ (!inventorySlotOfNewItem.IsDominant)}");
    }

    public bool PickUpItem(NetworkPickableItem pickableItem)
    {
        CmdPickUpItem(pickableItem);
        _interactableComponent = null;
        return true;
    }

    private void _PutItemInHand(InventorySlot inventorySlotOfNewItem, Transform pickableParent, PickableItem pickableItem)
    {
        bool isLeftHandAction = (_inventorySlots.GetDominantIndex() == _LEFT_HAND_ID) ^ (!inventorySlotOfNewItem.IsDominant);

        inventorySlotOfNewItem.SetRigidBody(pickableParent.GetComponent<Rigidbody>(), _linearDrag);
        inventorySlotOfNewItem.ItemTransform = pickableParent;

        pickableParent.transform.SetParent(isLeftHandAction ? _leftHandSocket : _rightHandSocket);

        pickableItem.OrientItemInHand(isLeftHandAction);
    }

    private void DropItem(float throwForce = 0)
    {
        CmdDropItem(throwForce);
    }

    [Command]
    public void CmdDropItem(float throwForce)
    {
        bool isThrow = _arms.IsDomOutStretched();

        InventorySlot dominantSlot = _inventorySlots.GetDominantHand();
        if (dominantSlot.Item == null) return;

        PickableItem itemToDrop = dominantSlot.Item;
        PickableItemSO pickableItemSO = itemToDrop.PickableItemSO;
        Transform networkItem = Instantiate(pickableItemSO.NetworkPrefab,
                                            dominantSlot.ItemTransform.position,
                                            dominantSlot.ItemTransform.rotation);

        NetworkServer.Spawn(networkItem.gameObject);

        Rigidbody networkRigidbody = networkItem.GetComponent<Rigidbody>();

        networkRigidbody.isKinematic = false;

        _inventorySlots.RemoveItem();

        Destroy(itemToDrop.transform.parent.gameObject);

        if (isThrow)
        {
            networkRigidbody.AddForce(transform.forward * throwForce, ForceMode.Impulse);
            _arms.ToggleHandMoveOutOrIn(_inventorySlots.IsLHandDom);
        }
        else
        {
            networkRigidbody.AddForce(transform.forward, ForceMode.Impulse);
            _arms.HandMoveOutAndIn(_inventorySlots.IsLHandDom);
        }
    }


    public Arms GetArms() { return _arms; }


    #region graveyard
    private void MoveItemsPositionsToHands()
    {
        //MoveItemPositionToHand(_inventorySlots[_LEFT_HAND_ID]);
        //MoveItemPositionToHand(_inventorySlots[_RIGHT_HAND_ID]);
        return;
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
        while (timer < duration)
        {
            target.rotation = Quaternion.Slerp(start, rotation, timer / duration);
            yield return null;
            timer += Time.deltaTime;
        }
        target.rotation = rotation;
    }

    #endregion graveyard
}

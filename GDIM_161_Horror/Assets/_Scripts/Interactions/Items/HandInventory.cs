using System.Collections;
using Interactions;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.Windows;

/// <summary>
/// Allows the Player to interact with other items and store them in two slots.
/// </summary>
public class HandInventory : NetworkBehaviour
{
    [Header("General Settings")]
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private Transform _rightHandTransform; // hands
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

    [Header("Debugging")]
    [SerializeField] private bool _enableDebugging;
    
    private static bool _staticDebugging;

    private InventorySlots _inventorySlots = new();
    private PlayerControls _playerControls;
    private IInteractable _interactable;
    private int _playerID;

    private const int _LEFT_HAND_ID = 0;
    private const int _RIGHT_HAND_ID = 1;

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
            Debugger(this);
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
                Debugger($"Item placed in DOM hand: {(IsLHandDom ? "L" : "R")}");
                return selectedHand;
            }
            selectedHand = GetOffHand();
            if (selectedHand.Item == null)
            {
                selectedHand.Item = item;
                Debugger($"Item placed in OFF hand, {(IsLHandDom ? "R" : "L")}");
                return selectedHand;
            }
            return null;
        }

        public NetworkPickableItem RemoveItem(Vector3 throwDir, int playerID)
        {
            InventorySlot inventorySlotToRemove = GetDominantHand();
            if (inventorySlotToRemove.Item == null) return null;
            inventorySlotToRemove.RemoveRigidBody();
            inventorySlotToRemove.Item.UnPossessItem(throwDir, playerID);
            NetworkPickableItem holder = inventorySlotToRemove.Item;
            inventorySlotToRemove.Item = null;

            return holder;
        }

        public bool SwapDominance()
        {
            SetLeftHandDominant(!IsLHandDom);
            return IsLHandDom;
        }

        public InventorySlot GetDominantHand()
        {
            Debugger($"getting DOM hand, {(IsLHandDom ? "L" : "R")}");
            return this[IsLHandDom ? 0 : 1];
        }

        public InventorySlot GetOffHand()
        {
            Debugger($"getting OFF hand, {(IsLHandDom ? "R" : "L")}");
            return this[IsLHandDom ? 1 : 0];
        }

        public int GetDominantIndex() { return IsLHandDom ? 0 : 1; }
    }

    void Start()
    {
        if (gameObject.TryGetComponent<PlayerObjectController>(out PlayerObjectController playerController)) _playerID = playerController.PlayerIdNumber;
        _staticDebugging = _enableDebugging;
        Debugger($"The Player ID is: {_playerID}");
        SetHandTransforms();
        SetUpControls();
    }

    private void SetUpControls()
    {
        _playerControls = new();
        _playerControls.Enable();
        _playerControls.Player.Interact.started += OnInteraction;
        _playerControls.Player.Interact.canceled += OnInteraction;
        _playerControls.Player.Interact.performed += OnInteraction;
    }

    private void DisableControls()
    {
        _playerControls.Player.Interact.started -= OnInteraction;
        _playerControls.Player.Interact.canceled -= OnInteraction;
        _playerControls.Player.Interact.performed -= OnInteraction;
        _playerControls.Disable();
    }

    public void OnDisable()
    {
        DisableControls();
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

            IInteractable newInteractable = hitInfo.transform.GetComponentInChildren<IInteractable>();

            // If we didn't hit something in the previous frame.
            if (_interactable == null && newInteractable != null)
            {
                // Report it as detected
                _interactable = newInteractable;
                _interactable.Detected(_playerID);
                _mouse.InteractionEffect();
            } 
            // If we are hitting a different object than in the previous frame.
            else if (newInteractable != _interactable)
            {
                // Stop animation and start the new one
                _interactable.StoppedDetecting(_playerID);
                _interactable = newInteractable;
                _interactable.Detected(_playerID);
            }
        }
        // If we didn't hit anything did we hit something before?
        else if (_interactable != null)
        {
            _interactable.StoppedDetecting(_playerID);
            _mouse.DefaultEffect();
            _interactable = null;
        }
    }

    public void OnSwap(InputValue value) 
    {
        SwapAction();
    }

    [Command]
    private void CmdSwapDominance()
    {
        RpcSwapDominance();
    }

    [ClientRpc]
    private void RpcSwapDominance()
    {
        bool isLHandDom = _inventorySlots.SwapDominance();
        _arms.SetHandDominancePosition(isLHandDom, !isLHandDom);
    }

    public void OnInteraction(InputAction.CallbackContext context) 
    {
        if(_interactable == null) return;

        _interactable.StoppedDetecting(_playerID);
        _mouse.DefaultEffect();

        if (_interactable is PolyInteractable) PolyInteractableSync(context.phase);
        else InteractableSync(context.phase);
    }

    public void OnDrop(InputValue value) 
    {
        Debugger($"Drop: Is Player {_playerID} Server: {isServer}");
        if (isServer) this._inventorySlots.RemoveItem(Vector3.zero, _playerID);
        else CmdDropItem(0f);
    }

    public void OnThrow(InputValue value) 
    {
        ThrowAction();
    }

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
        pickableItem.OrientItemInHand(handSlot.ItemTransform, isLeftHandAction);

        Debugger($"Player Is placing item: {pickableItem.name} in: {(isLeftHandAction ? "Left" : "Right")} Hand");

        Physics.IgnoreCollision(pickableItem.transform.parent.transform.GetComponent<Collider>(), GetComponent<Collider>(), true);

        return true;
    }

    public void DropAllItems()
    {
        StartCoroutine(ThrowAllItems());
    }

    private void ThrowAction()
    {
        Debugger($"Throw-Is Player {_playerID} Server: {isServer}");
        if (isServer) this._inventorySlots.RemoveItem(transform.forward * _throwForce, _playerID);
        else CmdDropItem(_throwForce);
    }

    private void SwapAction()
    {
        if (isServer) RpcSwapDominance();
        else CmdSwapDominance();
    }

    private void InteractableSync(InputActionPhase actionPhase)
    {
        Debugger($"Player {_playerID} Interacted with {_interactable.GetNetworkID().gameObject.name}");
        Debugger($"Is Player {_playerID} The Server: {isServer}");
        try
        {
            if (isServer) ExecuteInteraction(_playerID, _interactable, actionPhase);
            else CmdOnInteract(_interactable.GetNetworkID(), _playerID, actionPhase);
            _interactable = null;
        }
        catch 
        {
            Debug.LogWarning($"Player {_playerID}: Interactable component is set null");
        }
    }

    private void PolyInteractableSync(InputActionPhase actionPhase)
    {
        Debugger($"Player {_playerID} Interacted with {_interactable.GetNetworkID().gameObject.name}");
        Debugger($"Is Player {_playerID} The Server: {isServer}");
        try
        {
            if (isServer) ExecuteInteraction(_playerID, _interactable, actionPhase);
            else CmdOnPolyInteract(_interactable.GetNetworkID(), _playerID,
                (_interactable as PolyInteractable).Order, actionPhase);
            _interactable = null;
        }
        catch
        {
            Debug.LogWarning($"Player {_playerID}: Interactable component is set null");
        }
    }

    [ClientRpc]
    private void RpcOnPolyInteract(NetworkIdentity interactableID, int playerID, PolyInteractableOrder order, InputActionPhase actionPhase)
    {
        Debugger($"RPC OnInteract being called");
        Debugger($"Interactable is: {interactableID.name}");
        if (playerID != _playerID) return;

        PolyInteractable[] interactables = interactableID.GetComponentsInChildren<PolyInteractable>();
        foreach(PolyInteractable interactable in interactables)
        {
            if (interactable.Order == order)
            {
                ExecuteInteraction(_playerID, interactable, actionPhase);
                return;
            }
        }
    }

    [Command]
    private void CmdOnPolyInteract(NetworkIdentity interactableID, int playerID, PolyInteractableOrder order, InputActionPhase actionPhase)
    {
        Debugger($"CMD POLY: Player {playerID} is Interacting with object {interactableID.gameObject.name}");
        RpcOnPolyInteract(interactableID, playerID, order, actionPhase);
    }

    [ClientRpc]
    private void RpcOnInteract(NetworkIdentity interactableID, int playerID, InputActionPhase actionPhase)
    {
        Debugger($"RPC: Player {playerID} is Interacting with object {interactableID.gameObject.name}");
        if (playerID != _playerID) return;
        ExecuteInteraction(playerID, interactableID.GetComponentInChildren<IInteractable>(), actionPhase);
    }

    [Command]
    private void CmdOnInteract(NetworkIdentity interactableID, int playerID, InputActionPhase actionPhase)
    {
        Debugger($"CMD: Player {playerID} is Interacting with object {interactableID.gameObject.name}");
        RpcOnInteract(interactableID, playerID, actionPhase);
    }

    private void ExecuteInteraction(int playerID, IInteractable interactable, InputActionPhase actionPhase)
    {
        switch (actionPhase)
        {
            case InputActionPhase.Started:
                interactable.StartedInteraction(playerID);
                break;
            case InputActionPhase.Canceled:
                interactable.CanceledInteraction(playerID);
                break;
            case InputActionPhase.Performed:
                interactable.PerformedInteraction(playerID);
                break;
        }
    }

    public bool IsInventoryFull()
    {
        return _inventorySlots[_LEFT_HAND_ID].Item != null && _inventorySlots[_RIGHT_HAND_ID].Item != null;
    }

    [ClientRpc]
    private void RpcDropItem(float throwForce)
    {
        Debugger($"RPC drop item being Called with Force: {throwForce}");
        NetworkPickableItem temp = this._inventorySlots.RemoveItem(transform.forward * throwForce, _playerID);
    }

    [Command]
    private void CmdDropItem(float throwForce)
    {
        Debugger($"CMD Drop item being called with Force: {throwForce}");
        RpcDropItem(throwForce);
    }

    private static void Debugger(object log)
    {
        if (_staticDebugging) Debug.Log(log);
    }

    // This is to get references to the players through the network but player manager does this already.
    private NetworkIdentity GetPlayerIdentity()
    {
        return GetComponent<NetworkIdentity>();
    }

    IEnumerator ThrowAllItems()
    {
        ThrowAction();
        SwapAction();
        yield return null;
        ThrowAction();
    }
}

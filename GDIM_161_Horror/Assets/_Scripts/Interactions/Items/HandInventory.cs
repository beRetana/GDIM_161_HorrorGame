using System.Collections;
using Interactions;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using System;
using OtherUtils;

/// <summary>
/// Allows the Player to interact with other items and store them in two slots.
/// </summary>
public class HandInventory : NetworkBehaviour, IDebugger
{
    [Header("General Settings")]
    [SerializeField] private LayerMask m_InteractableLayers;
    [SerializeField] private LayerMask m_Obstructables;
    [SerializeField] private Transform _rightHandTransform; // hands
    [SerializeField] private Transform _leftHandTransform;

    [Space(5f), Header("Interaction Physics Settings")]
    [SerializeField] private float m_PickUpRange;
    [SerializeField] private float _pickUpForce;
    [SerializeField] private float _linearDrag;
    [SerializeField] private float _throwForce;
    [SerializeField] private MouseUI _mouse;
    [SerializeField] private Camera _playerCamera;

    [Space(5f), Header("Debugging")]
    [SerializeField] private bool _enableDebugging;

    public event Action<NetworkPickableItem> OnSwapingHands;
    
    private static bool _staticDebugging;

    private InventorySlots _inventorySlots = new();
    private PlayerAnimator m_PlayerAnimator;
    private PlayerControls _playerControls;
    private IInteractable _interactable;
    private int _playerID;
    private bool m_EnablePickingUp;

    private const int _LEFT_HAND_ID = 0;
    private const int _RIGHT_HAND_ID = 1;

    public int PlayerID => _playerID;
    public bool EnablePickingUp { get { return m_EnablePickingUp; } set { m_EnablePickingUp = value; } }

    private class InventorySlot
    {
        private NetworkPickableItem m_Item;
        private Rigidbody _itemRigidBody;
        private Transform m_HandTransform;
        private bool _isDominant;

        public NetworkPickableItem Item { get { return m_Item; } set { m_Item = value; } }

        public Rigidbody ItemRigidBody { get { return _itemRigidBody; } set { _itemRigidBody = value; } }

        public Transform ItemTransform { get { return m_HandTransform; } set { m_HandTransform = value; } }
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
            if (rigidBodyToDrop == null) return null;
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
            StaticDebugger(this.ToString());
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
                StaticDebugger($"Item placed in DOM hand: {(IsLHandDom ? "L" : "R")}");
                return selectedHand;
            }
            selectedHand = GetOffHand();
            if (selectedHand.Item == null)
            {
                selectedHand.Item = item;
                StaticDebugger($"Item placed in OFF hand, {(IsLHandDom ? "R" : "L")}");
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


        /// <summary>
        /// Switched dominance and returns if in the new state the left hand is dominant.
        /// </summary>
        /// <returns></returns>
        public bool SwapDominance()
        {
            SetLeftHandDominant(!IsLHandDom);
            return IsLHandDom;
        }

        public InventorySlot GetDominantHand()
        {
            StaticDebugger($"getting DOM hand, {(IsLHandDom ? "L" : "R")}");
            return this[IsLHandDom ? 0 : 1];
        }

        public InventorySlot GetOffHand()
        {
            StaticDebugger($"getting OFF hand, {(IsLHandDom ? "R" : "L")}");
            return this[IsLHandDom ? 1 : 0];
        }

        public int GetDominantIndex() { return IsLHandDom ? 0 : 1; }
    }

    private void Start()
    {
        if (gameObject.TryGetComponent<PlayerObjectController>(out PlayerObjectController playerController)) 
            _playerID = playerController.PlayerID;
        _staticDebugging = _enableDebugging;
        Debugger($"The Player ID is: {_playerID}");
        SetHandTransforms();
        m_PlayerAnimator = GetComponent<PlayerAnimator>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (!isLocalPlayer) return;
        SetUpControls();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        m_EnablePickingUp = NewNetworkManager.NewSingleton.IsGameplayScene(scene.name);
        SetControlsActive(m_EnablePickingUp);
    }

    public void SetControlsActive(bool state)
    {
        if (!isLocalPlayer) return;
        if (state) SetUpControls();
        else DisableControls();
    }

    private void SetUpControls()
    {
        if (_playerControls == null) _playerControls = new();
        _playerControls.Enable();
        _playerControls.Player.Interact.started += OnInteraction;
        _playerControls.Player.Interact.canceled += OnInteraction;
        _playerControls.Player.Interact.performed += OnInteraction;
        _playerControls.Player.UseItem.started += OnUsePickable;
        _playerControls.Player.UseItem.canceled += OnUsePickable;
        _playerControls.Player.UseItem.performed += OnUsePickable;
    }

    private void DisableControls()
    {
        if (!isLocalPlayer || _playerControls == null) return;
        _playerControls.Player.Interact.started -= OnInteraction;
        _playerControls.Player.Interact.canceled -= OnInteraction;
        _playerControls.Player.Interact.performed -= OnInteraction;
        _playerControls.Player.UseItem.started -= OnUsePickable;
        _playerControls.Player.UseItem.canceled -= OnUsePickable;
        _playerControls.Player.UseItem.performed -= OnUsePickable;
        _playerControls.Disable();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        DisableControls();
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
        if (!m_EnablePickingUp) return;
        CheckForRaycastInteractables();
    }

    /// <summary>
    /// Detects interactable items to display options on interacting with that object
    /// </summary>
    private void CheckForRaycastInteractables()
    {
        Ray rayToInteract = _playerCamera.ViewportPointToRay(new Vector3(0.5f,0.5f, 0));
        bool wasSomethingHit = Physics.Raycast(rayToInteract, out RaycastHit hitInfo, m_PickUpRange, m_InteractableLayers | m_Obstructables);
        Debugger($"{(wasSomethingHit ? $"{hitInfo.collider.gameObject.name} was hit!" : "Nothing was Hit")}");
        // Did we hit something and is this something in the interactable layer?
        if (wasSomethingHit && (((1 << hitInfo.collider.gameObject.layer) & m_InteractableLayers) != 0))
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
        _inventorySlots.SwapDominance();
        OnSwapingHands?.Invoke(PeekAtDominant());
        m_PlayerAnimator.SwitchHands(!_inventorySlots.IsLHandDom);
    }

    public void OnInteraction(InputAction.CallbackContext context) 
    {
        if(_interactable == null) return;

        _interactable.StopDetecting(_playerID);
        _mouse.DefaultEffect();

        InputData inputData = new(context);

        if (_interactable is PolyInteractable) PolyInteractableSync(inputData);
        else InteractableSync(inputData);
    }

    public void OnDrop(InputValue value) 
    {
        DropAction();
    }

    public void OnThrow(InputValue value) 
    {
        ThrowAction();
    }

    public void OnUsePickable(InputAction.CallbackContext context) 
    {
        UseItem(new InputData(context)); 
    }

    public void UseItem(InputData context)
    {
        if (!isServer) CmdUseItem(context);
        else RpcUseItem(context);
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

    public void DropAction()
    {
        if (!isLocalPlayer) return;
        Debugger($"Drop: Is Player {_playerID} Server: {isServer}");
        if (isServer) this._inventorySlots.RemoveItem(Vector3.zero, _playerID);
        else CmdDropItem(0f);
    }

    private void ThrowAction()
    {
        Debugger($"Throw-Is Player {_playerID} Server: {isServer}");
        if (isServer) this._inventorySlots.RemoveItem(transform.forward * _throwForce, _playerID);
        else CmdDropItem(_throwForce);
    }

    public void SwapAction()
    {
        if (!isLocalPlayer) return;

        if (isServer) RpcSwapDominance();
        else CmdSwapDominance();
    }

    private void InteractableSync(InputData inputData)
    {
        Debugger($"Player {_playerID} Interacted with {_interactable.GetNetworkID().gameObject.name}");
        Debugger($"Is Player {_playerID} The Server: {isServer}");
        try
        {
            if (isServer) ExecuteInteraction(_playerID, _interactable, inputData);
            else CmdOnInteract(_interactable.GetNetworkID(), _playerID, inputData);
            _interactable = null;
        }
        catch 
        {
            Debug.LogWarning($"Player {_playerID}: Interactable component is set null");
        }
    }

    private void PolyInteractableSync(InputData context)
    {
        Debugger($"Player {_playerID} Interacted with {_interactable.GetNetworkID().gameObject.name}");
        Debugger($"Is Player {_playerID} The Server: {isServer}");
        try
        {
            if (isServer)
            {
                Debugger($"The item {(_interactable as PolyInteractable).transform.parent.gameObject.name} has order: {(_interactable as PolyInteractable).Order}");
                RpcOnPolyInteract(_interactable.GetNetworkID(), _playerID,
                (_interactable as PolyInteractable).Order, context);
            }
            else CmdOnPolyInteract(_interactable.GetNetworkID(), _playerID,
                (_interactable as PolyInteractable).Order, context);
            _interactable = null;
        }
        catch
        {
            Debug.LogWarning($"Player {_playerID}: Interactable component is set null");
        }
    }

    [ClientRpc]
    private void RpcOnPolyInteract(NetworkIdentity interactableID, int playerID, PolyInteractableOrder order,
        InputData inputData)
    {
        Debugger($"RPC OnInteract being called");
        Debugger($"Interactable is: {interactableID.name}");
        Debugger($"Poly Interactable of order: {order}");
        if (playerID != _playerID) return;

        PolyInteractable[] interactables = interactableID.GetComponentsInChildren<PolyInteractable>();
        foreach(PolyInteractable interactable in interactables)
        {
            Debugger($"The item is {interactable.name} with order: {interactable.Order}");
            if (interactable.Order == order)
            {
                ExecuteInteraction(_playerID, interactable, inputData);
                return;
            }
        }
    }

    [Command]
    private void CmdOnPolyInteract(NetworkIdentity interactableID, int playerID, PolyInteractableOrder order,
        InputData inputData)
    {
        Debugger($"CMD POLY: Player {playerID} is Interacting with object {interactableID.gameObject.name}");
        RpcOnPolyInteract(interactableID, playerID, order, inputData);
    }

    [ClientRpc]
    private void RpcOnInteract(NetworkIdentity interactableID, int playerID, InputData inputData)
    {
        Debugger($"RPC: Player {playerID} is Interacting with object {interactableID.gameObject.name}");
        if (playerID != _playerID) return;
        ExecuteInteraction(playerID, interactableID.GetComponentInChildren<IInteractable>(), inputData);
    }

    [Command]
    private void CmdOnInteract(NetworkIdentity interactableID, int playerID, InputData inputData)
    {
        Debugger($"CMD: Player {playerID} is Interacting with object {interactableID.gameObject.name}");
        RpcOnInteract(interactableID, playerID, inputData);
    }

    private void ExecuteInteraction(int playerID, IInteractable interactable, InputData context)
    {
        interactable.Interaction(playerID, context);
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

    [Command]
    private void CmdUseItem(InputData context)
    {
        RpcUseItem(context);
    }

    [ClientRpc]
    private void RpcUseItem(InputData context)
    {
        NetworkPickableItem itemToUse = _inventorySlots.GetDominantHand().Item;
        if (itemToUse == null) return;
        itemToUse.UseItem(_playerID, context);
    }

    /// <summary>
    /// Returns the current dominant item or null if there is none.
    /// </summary>
    /// <returns></returns>
    public NetworkPickableItem PeekAtDominant()
    {
        return _inventorySlots.GetDominantHand().Item;
    }

    public static void StaticDebugger(string log)
    {
        if (_staticDebugging) Debug.Log(log);
    }
    public void Debugger(object log)
    {
        if (_enableDebugging) Debug.Log(log);
    }

    // This is to get references to the players through the network but player manager does this already.
    private NetworkIdentity GetPlayerIdentity()
    {
        return GetComponent<NetworkIdentity>();
    }

    public IEnumerator ThrowAllItems()
    {
        ThrowAction();
        SwapAction();
        yield return null;
        ThrowAction();
    }

    public void SetDebugActive(bool active)
    {
        _enableDebugging = active;
    }
}

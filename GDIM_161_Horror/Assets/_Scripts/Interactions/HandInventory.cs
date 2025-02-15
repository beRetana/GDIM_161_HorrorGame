using System.Collections;
using System.Collections.Generic;
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
        public GameObject pickedObject;
        public Rigidbody rigidbody;
        public Transform transform;
        public float initialLinearDamping;
    }

    private List<InventorySlot> _inventorySlots;
    private PlayerControls _playerControls;
    private GameObject _mouse;
    private int _playerID;
    private IInteractable _interactableComponent;

    private const int _RIGHT_HAND_ID = 0;
    private const int _LEFT_HAND_ID = 1;

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
        _mouse = DataMessenger.GetGameObject(MessengerKeys.GameObjectKey.MouseUI);
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

        _playerControls.Player.Interact.performed += OnInteract;
        _playerControls.Player.Drop.performed += OnDrop;
        _playerControls.Player.Throw.performed += OnThrow;
        _playerControls.Player.Swap.performed += OnSwap;
    }

    void OnDisable(){
        _playerControls.Player.Interact.performed -= OnInteract;
        _playerControls.Player.Drop.performed -= OnDrop;
        _playerControls.Player.Throw.performed -= OnThrow;
        _playerControls.Player.Swap.performed -= OnSwap;

        _playerControls.Player.Interact.Disable();
        _playerControls.Player.Drop.Disable();
        _playerControls.Player.Throw.Disable();
        _playerControls.Player.Swap.Disable();
    }

    void PrepareList()
    {
        _inventorySlots = new List<InventorySlot> { new InventorySlot(), new InventorySlot()};
        _inventorySlots[_RIGHT_HAND_ID].transform = _rightHandTransform;
        _inventorySlots[_LEFT_HAND_ID].transform = _leftHandTransform;
    }

    void Update()
    {
        CheckForInteractables();
        MoveItemsToHands();
    }

    /// <summary>
    /// Traces a ray from the center of the screen of a lenght then checks certain conditions.
    /// </summary>
    void CheckForInteractables(){
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
                Debug.Log(_interactableComponent);
                Debug.Log(_playerID);
                _interactableComponent.Detected(_playerID);
                _mouse.GetComponent<MouseUI>().InteractionEffect();
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
            _mouse.GetComponent<MouseUI>()?.DefaultEffect();
            _interactableComponent = null;
        }

    }

    void OnSwap(InputAction.CallbackContext context)
    {
        InventorySlot rightItem = _inventorySlots[_RIGHT_HAND_ID];
        InventorySlot leftItem = _inventorySlots[_LEFT_HAND_ID];

        _inventorySlots[_RIGHT_HAND_ID] = leftItem;
        _inventorySlots[_LEFT_HAND_ID] = rightItem;
    }
 
    void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interacted with: "+ _interactableComponent);
        _interactableComponent?.Interact(_playerID);
    }

    void OnDrop(InputAction.CallbackContext context)
    {
        DropItem();
    }

    void OnThrow(InputAction.CallbackContext context)
    {
        DropItem(_throwForce);
    }

    int GetFreeHands()
    {
        int index = 0;
        foreach(InventorySlot slot in _inventorySlots)
        {
            Debug.Log(slot.pickedObject);
            if(slot.pickedObject == null) return index;

            index++;
        }   

        return -1;
    }

    public void PickUpItem(GameObject pickableItem)
    {
        Debug.Log($"Trying to picking up {pickableItem.name}");

        int freeHandIndex = GetFreeHands();

        if (freeHandIndex != -1 && pickableItem.GetComponent<Rigidbody>())
        {

            Debug.Log($"There is at least one free hand {freeHandIndex}");

            InventorySlot inventorySlot = _inventorySlots[freeHandIndex];
            inventorySlot.pickedObject = pickableItem;
            inventorySlot.rigidbody = pickableItem.GetComponent<Rigidbody>();
            inventorySlot.rigidbody.useGravity = false;
            inventorySlot.initialLinearDamping = inventorySlot.rigidbody.linearDamping;
            inventorySlot.rigidbody.linearDamping = _linearDrag;
            inventorySlot.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            inventorySlot.rigidbody.transform.parent = inventorySlot.transform;

            Physics.IgnoreCollision(inventorySlot.pickedObject.GetComponent<Collider>(),
                                    gameObject.GetComponentInChildren<CapsuleCollider>(), true);
            //Switch this magic number later
            StartCoroutine(AnimateRotationTowards(inventorySlot.pickedObject.transform, inventorySlot.transform.rotation, 1f));

            Debug.Log($"The item has been saved as {_inventorySlots[freeHandIndex].pickedObject.name}");
        }
        else
        {
            Debug.Log($"Trying to drop {pickableItem.name}: No free hands");
            DropItem();
        }
    }

    private void MoveItemsToHands()
    {
        foreach(InventorySlot inventorySlot in _inventorySlots)
        {
            if (inventorySlot.pickedObject == null) continue;

            if (Vector3.Distance(inventorySlot.pickedObject.transform.position, inventorySlot.transform.position) <= .1f) continue;
            
            Vector3 direction = inventorySlot.transform.position - inventorySlot.pickedObject.transform.position;
            inventorySlot.rigidbody.AddForce(direction * _pickUpForce);
        } 
    }

    private IEnumerator AnimateRotationTowards(Transform target, Quaternion rotation, float duration)
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

        foreach(InventorySlot inventorySlot in _inventorySlots)
        {
            if (inventorySlot.pickedObject == null) continue;

            inventorySlot.rigidbody.useGravity = true;
            inventorySlot.rigidbody.linearDamping = inventorySlot.initialLinearDamping;
            inventorySlot.rigidbody.freezeRotation = false;

            Physics.IgnoreCollision(inventorySlot.pickedObject.GetComponent<Collider>(), 
                                    gameObject.GetComponentInChildren<CapsuleCollider>(), false);

            inventorySlot.rigidbody.transform.parent = null;
            inventorySlot.pickedObject = null;
            inventorySlot.rigidbody.AddForce(transform.forward * throwForce, ForceMode.Impulse);
            inventorySlot.rigidbody = null;
            return;
        }
    }
}

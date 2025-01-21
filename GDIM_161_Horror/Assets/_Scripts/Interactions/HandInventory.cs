using System;
using MessengerSystem;
using Unity.VisualScripting;
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

    private struct InventorySlot{
        public GameObject pickedObject;
        public Rigidbody rigidbody;
        public Transform transform;
        public float initialLinearDamping;
    }

    private PlayerControls _playerControls;
    private InventorySlot _leftHand;
    private InventorySlot _rightHand;
    private GameObject _mouse;
    private int playerID;
    private GameObject _player;
    private IInteractable _interactableComponent;

    [Header("Interaction Physics Settings")]

    [SerializeField] private float _pickUpRange;
    [SerializeField] private float _pickUpForce;
    [SerializeField] private float _linearDrag;
    [SerializeField] private float _throwForce;

    void Awake() 
    {
        DataMessenger.SetGameObject(MessengerKeys.GameObjectKey.Player, gameObject);
        _playerControls = new();
        OnEnable();
    }

    

    void Start()
    {
        _mouse = DataMessenger.GetGameObject(MessengerKeys.GameObjectKey.MouseUI);
        _leftHand.transform = _leftHandTransform;
        _rightHand.transform = _rightHandTransform;
        playerID = gameObject.GetComponent<PlayerBase>().ID();
        Debug.Log($"{this.name} is attatched to PlayerID:{playerID}");
    }

    void OnEnable()
    {
        _playerControls.Player.Interact.Enable();
        _playerControls.Player.Drop.Enable();
        _playerControls.Player.Throw.Enable();

        _playerControls.Player.Interact.performed += OnInteract;
        _playerControls.Player.Drop.performed += OnDrop;
        _playerControls.Player.Throw.performed += OnThrow;
    }

    void OnDisable(){
        _playerControls.Player.Interact.Disable();
        _playerControls.Player.Drop.Disable();
        _playerControls.Player.Throw.Disable();
    }

    void Update()
    {
        CheckForInteractables();
        MovePickedObjects();
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
                Debug.Log(interactableComponent);
                Debug.Log(playerID);
                _interactableComponent.Detected(playerID);
                _mouse.GetComponent<MouseUI>().InteractionEffect();
            } 
            // If we are hitting a different object than before.
            else if (childCanvas != _interactableComponent)
            {
                // Stop animation and start the new one
                _interactableComponent.StoppedDetecting(playerID);
                _interactableComponent = childCanvas;
                _interactableComponent?.Detected(playerID);
            }
        }
        // If we didn't hit anything did we hit something before?
        else if (_interactableComponent != null)
        {
            _interactableComponent.StoppedDetecting(playerID);
            _mouse.GetComponent<MouseUI>()?.DefaultEffect();
            _interactableComponent = null;
        }

    }
 
    void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interacted with: "+ _interactableComponent);
        _interactableComponent?.Interact(playerID);
    }

    void OnDrop(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    void OnThrow(InputAction.CallbackContext context)
    {
        /*
        pickedObjRB.useGravity = true;
        pickedObjRB.linearDamping = initialLinearDamping;
        pickedObjRB.freezeRotation = false;
        pickedObj.transform.parent = null;
        Physics.IgnoreCollision(pickedObj.GetComponent<Collider>(), player.GetComponent<CapsuleCollider>(), false);
        pickedObjRB.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        pickedObj = null;
        pickedObjRB = null;
        */
    }

    bool CheckFreeHands()
    {
        if(_rightHand.pickedObject == null)
        {
            return true;
        }

        if(_leftHand.pickedObject == null)
        {
            return true;
        }

        return false;
    }

    InventorySlot GetFreeHand()
    {
        if(_rightHand.pickedObject == null)
        {
            return _rightHand;
        }
        
        return _leftHand;
    }

    public void PickUpObject(GameObject pickableItem)
    {
        if(CheckFreeHands()){

            InventorySlot inventorySlot = GetFreeHand();

            if(pickableItem.GetComponent<Rigidbody>())
            {
                inventorySlot.pickedObject = pickableItem;
                inventorySlot.rigidbody = pickableItem.GetComponent<Rigidbody>();
                inventorySlot.rigidbody.useGravity = false;
                inventorySlot.initialLinearDamping = inventorySlot.rigidbody.linearDamping;
                inventorySlot.rigidbody.linearDamping = _linearDrag;
                inventorySlot.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                inventorySlot.rigidbody.transform.parent = inventorySlot.transform;

                Physics.IgnoreCollision(inventorySlot.pickedObject.GetComponent<Collider>(), _player.GetComponent<CapsuleCollider>(), true);
            }
        }
        else
        {
            OnDrop();
        }
    }

    private void MovePickedObjects()
    {

        void MoveObject(InventorySlot inventorySlot)
        {
            if(Vector3.Distance(inventorySlot.pickedObject.transform.position, inventorySlot.transform.position) > .1f){
                Vector3 direction =  inventorySlot.transform.position - inventorySlot.pickedObject.transform.position;
                inventorySlot.rigidbody.AddForce(direction * _pickUpForce);
            }
        }

        if(_rightHand.pickedObject != null)
        {
            MoveObject(_rightHand);
        }     
        if(_leftHand.pickedObject != null)
        {
            MoveObject(_leftHand);
        } 
    }

    private void OnDrop(){

        if(!CheckFreeHands())
        {
            InventorySlot inventorySlot = GetFreeHand();
            inventorySlot.rigidbody.useGravity = true;
            inventorySlot.rigidbody.linearDamping = inventorySlot.initialLinearDamping;
            inventorySlot.rigidbody.freezeRotation = false;
            Physics.IgnoreCollision(inventorySlot.pickedObject.GetComponent<Collider>(), _player.GetComponent<CapsuleCollider>(), false);
            inventorySlot.rigidbody.transform.parent = null;
            inventorySlot.pickedObject = null;
            inventorySlot.rigidbody = null;
        }
    }
}

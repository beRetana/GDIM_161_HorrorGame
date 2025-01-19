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
    [SerializeField] private LayerMask interactableLayer;

    private struct InventorySlot{
        private Transform location;
        private GameObject pickedObject;
    }

    private PlayerControls playerControls;
    private InventorySlot leftHand;
    private InventorySlot rightHand;
    private GameObject mouse;
    private IInteractable interactableComponent;
    private float initialLinearDamping;

    [Header("Interaction Physics Settings")]

    [SerializeField] private float pickUpRange;
    [SerializeField] private float pickUpForce;
    [SerializeField] private float drag;
    [SerializeField] private float throwForce;

    void Awake() 
    {
        DataMessenger.SetGameObject(MessengerKeys.GameObjectKey.Player, gameObject);
        playerControls = new();
        OnEnable();
    }

    void Start()
    {
        mouse = DataMessenger.GetGameObject(MessengerKeys.GameObjectKey.MouseUI);
    }

    void OnEnable()
    {
        playerControls.Player.Interact.Disable();
        playerControls.Player.Drop.Enable();
        playerControls.Player.Throw.Enable();

        playerControls.Player.Interact.performed += OnInteract;
        playerControls.Player.Drop.performed += OnDrop;
    }

    void OnDisable(){
        playerControls.Player.Interact.Disable();
        playerControls.Player.Drop.Disable();
    }

    void Update()
    {
        CheckForInteractables();

        /*if(pickedObj != null){
            MovePickedObj();
        }*/
    }

    /// <summary>
    /// Traces a ray from the center of the screen of a lenght then checks certain conditions.
    /// </summary>
    void CheckForInteractables(){
        Ray rayToInteract = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f, 0));

        // If we hit something in the layer.
        if (Physics.Raycast(rayToInteract, out RaycastHit hitInfo, pickUpRange, interactableLayer))
        {
            // If we didn't hit something before.
            if (interactableComponent == null)
            {
                // Report it as detected
                interactableComponent = hitInfo.transform.gameObject.GetComponent<IInteractable>();
                interactableComponent.Detected();
                mouse.GetComponent<MouseUI>().InteractionEffect();
            } 
            // If we are hitting a different object than before.
            else if (hitInfo.transform.gameObject.GetComponent<IInteractable>() != interactableComponent)
            {
                // Stop animation and start the new one
                interactableComponent.StoppedDetecting();
                interactableComponent = hitInfo.transform?.gameObject.GetComponent<IInteractable>();
                interactableComponent?.Detected();
            }
        }
        // If we didn't hit anything did we hit something before?
        else if (interactableComponent != null)
        {
            interactableComponent.StoppedDetecting();
            mouse.GetComponent<MouseUI>()?.DefaultEffect();
            interactableComponent = null;
        }

    }
 
    private void OnInteract(InputAction.CallbackContext context)
    {
        interactableComponent?.Interact();
    }

    private void OnDrop(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void ThrowObject(InputAction.CallbackContext context)
    {
        /*pickedObjRB.useGravity = true;
        pickedObjRB.linearDamping = initialLinearDamping;
        pickedObjRB.freezeRotation = false;
        pickedObj.transform.parent = null;
        Physics.IgnoreCollision(pickedObj.GetComponent<Collider>(), player.GetComponent<CapsuleCollider>(), false);
        pickedObjRB.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        pickedObj = null;
        pickedObjRB = null;*/
    }

    public void PickUpObject(GameObject pickableItem)
    {
        /*RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange)){
            PickUpObject(hit.transform.gameObject);
        }
        else
        {
            OnDrop();
        }*/
    }

    private void MovePickedObj(){
        /*if(Vector3.Distance(pickedObj.transform.position, leftHand.position) > .1f){
            Vector3 direction =  leftHand.position - pickedObj.transform.position;
            pickedObjRB.AddForce(direction * pickUpForce);
        }*/
    }

    //private void PickUpObject(GameObject obj){
        /*if(obj.GetComponent<Rigidbody>()){
            pickedObjRB = obj.GetComponent<Rigidbody>();
            pickedObjRB.useGravity = false;
            initialLinearDamping = pickedObjRB.drag;
            pickedObjRB.drag = drag;
            pickedObjRB.constraints = RigidbodyConstraints.FreezeRotation;
            pickedObj = obj;
            pickedObjRB.transform.parent = leftHand;
            Physics.IgnoreCollision(pickedObj.GetComponent<Collider>(), player.GetComponent<CapsuleCollider>(), true);
        }*/
    //}

    private void OnDrop(){
        /*pickedObjRB.useGravity = true;
        pickedObjRB.drag = initialLinearDamping;
        pickedObjRB.freezeRotation = false;
        Physics.IgnoreCollision(pickedObj.GetComponent<Collider>(), player.GetComponent<CapsuleCollider>(), false);
        pickedObj.transform.parent = null;
        pickedObj = null;
        pickedObjRB = null;*/
    }
}

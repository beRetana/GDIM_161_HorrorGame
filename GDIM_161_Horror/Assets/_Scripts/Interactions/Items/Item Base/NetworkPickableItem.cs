using Mirror;
using UnityEngine;

namespace Interactions
{
    /// <summary>
    /// This is a base class for items that can be picked up. It uses interactable items.
    /// </summary>
    [RequireComponent(typeof(InteractableItem))]
    public class NetworkPickableItem : NetworkBehaviour
    {
        [SerializeField] private PickableItemSO _pickableItemSO; // Contains information for soft parenting
        [SerializeField] protected bool _debugger;
        protected bool _isLeftHand;

        protected InteractableItem _interactableItem;
        protected Transform _targetHand;
        [SerializeField] protected Collider _itemCollider;
        [SyncVar] protected bool _isPossessed;
        [SyncVar] protected int _ownerPlayerID;

        public PickableItemSO PickableItemSO { get { return _pickableItemSO; } }
        public bool IsPossessed { get { return _isPossessed; }} // Held in Hand || Moving to Hand
        public int OwnerPlayerID { get { return _ownerPlayerID; }}

        protected virtual void Start()
        {
            _interactableItem = GetComponent<InteractableItem>();
            _interactableItem.SetInteractAction(PickItem);
        }

        public override string ToString()
        {
            return $"Item: {this.name}";
        }

        protected virtual void PickItem(int playerID) // <= (InteractableItem)this.Interact()
        {
            if (_isPossessed)
            {
                Debugger($"tried PICK UP on {this}, but is already possessed");
                return;
            }
            try
            {
                HandInventory playerInventory = PlayerManager.Instance.GetPlayer(playerID).GetComponent<HandInventory>();

                if (playerInventory.IsInventoryFull()) return;

                bool success = playerInventory.PickUpItem(this);
                if (!success) return;

                SetPossessed(true, playerID);
                AudioManager.instance.PlayOneShot(FMODEvents.instance.torchGrab, this.transform.position);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error picking up item {this.name}: {e.Message}");
                Debug.LogError($"Player with ID {playerID}");
            }
        }

        public virtual void UnPossessItem(Vector3 throwDir, int playerID)
        {
            Debugger($"THROWWWW: " + throwDir);
            transform.parent.transform.GetComponent<Rigidbody>().AddForce(throwDir, ForceMode.Impulse);
            Physics.IgnoreCollision(_itemCollider, PlayerManager.Instance.GetPlayer(playerID).GetComponent<Collider>(), false);
            _targetHand = null;
            SetPossessed(false, playerID);
        }

        public virtual void SetPossessed(bool isPossessed, int playerID = 0)
        {
            if (isServer) RpcSetPossessed(_targetHand, playerID);
            else CmdSetPossessed(isPossessed, playerID);
        }

        [Command(requiresAuthority = false)]
        public virtual void CmdSetPossessed(bool toPossess, int playerID)
        {
            RpcSetPossessed(toPossess, playerID);
        }

        [ClientRpc]
        public virtual void RpcSetPossessed(bool toPossess, int playerID)
        {
            Debugger($"Player {playerID} {(toPossess ? "posessing" : "forfeiting")} {this.name}");
            _isPossessed = toPossess;
            _ownerPlayerID = toPossess ? playerID : -1;
            _interactableItem.SetInteractive(!toPossess);
        }

        public virtual void UseItem(int playerID, InputData context) { }

        protected virtual void LateUpdate()
        {
            if (_targetHand != null && IsPossessed) SoftParenting();
        }

        protected virtual void SoftParenting()
        {
            if (_isLeftHand) SetRotationLocation(_pickableItemSO.LeftHandPosition, _pickableItemSO.LeftHandRotation);
            else SetRotationLocation(_pickableItemSO.RightHandPosition, _pickableItemSO.RightHandRotation);
        }

        public virtual void OrientItemInHand(Transform target, bool isLeftHand)
        {
            _isLeftHand = isLeftHand;
            _targetHand = target;
        }

        protected virtual void SetRotationLocation(Vector3 location, Vector3 rotation)
        {
            transform.parent.transform.position = _targetHand.position + (_targetHand.rotation * location);
            transform.parent.transform.rotation = _targetHand.rotation * Quaternion.Euler(rotation);
        }

        protected virtual void Debugger(string log)
        {
            if (_debugger) Debug.Log(log);
        }
    }
}
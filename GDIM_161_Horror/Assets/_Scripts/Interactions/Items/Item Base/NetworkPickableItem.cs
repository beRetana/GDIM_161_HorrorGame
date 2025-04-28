using Mirror;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Splines.Interpolators;

namespace Interactions
{
    /// <summary>
    /// This is a base class for items that can be picked up. It uses interactable items.
    /// </summary>
    [RequireComponent(typeof(InteractableItem))]
    public class NetworkPickableItem : NetworkBehaviour
    {
        [SerializeField] PickableItemSO _pickableItemSO; // Contains information for soft parenting

        protected InteractableItem _interactableItem;

        public PickableItemSO PickableItemSO { get { return _pickableItemSO; } }
        public bool IsPossessed {  get; private set; } // Held in Hand || Moving to Hand
        public int OwnerPlayerID { get; private set; }
        protected Transform locationTarget;
        protected Collider _itemCollider;

        protected virtual void Start()
        {
            _interactableItem = GetComponent<InteractableItem>();
            _interactableItem.SetInteractAction(PickItem);
            _itemCollider = transform.parent.transform.GetComponent<Collider>();
        }

        public override string ToString()
        {
            return $"Item: {this.name}";
        }

        protected virtual void PickItem(int playerID) // <= (InteractableItem)this.Interact()
        {
            if (IsPossessed)
            {
                Debug.Log($"tried PICK UP on {this}, but is already possessed");
                return;
            }

            bool success = PlayerManager.Instance.GetPlayer(playerID).GetComponent<HandInventory>().PickUpItem(this);
            if (!success) return;

            SetPossessed(true, playerID);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.torchGrab, this.transform.position);
        }

        public virtual void UnPossessItem(Vector3 throwDir, int playerID)
        {
            if (isServer) RpcThrowItem(throwDir);
            else CmdThrowItem(throwDir);
            SetPossessed(false, playerID);
            locationTarget = null;
        }

        [ClientRpc]
        private void RpcThrowItem(Vector3 throwDir)
        {
            Debug.Log($"RPC THROWWWW: " + throwDir);
            this.transform.parent.transform.GetComponent<Rigidbody>().AddForce(throwDir, ForceMode.Impulse);
        }

        [Command]
        private void CmdThrowItem(Vector3 throwDir)
        {
            Debug.Log($"CMD THROWWWW: "+throwDir);
            this.transform.parent.transform.GetComponent<Rigidbody>().AddForce(throwDir, ForceMode.Impulse);
        }

        public virtual void SetPossessed(bool toPossess, int playerID = 0)
        {
            Debug.Log($"Player {playerID} {(toPossess ? "posessing" : "forfeiting")} {this.name}");
            IsPossessed = toPossess;
            OwnerPlayerID = toPossess ? playerID : -1;
            _interactableItem.SetInteractive(!toPossess);
        }

        public virtual void UseItem(int playerID) { }

        protected virtual void Update()
        {
            if (IsPossessed) SoftParenting();
        }

        protected virtual void SoftParenting()
        {
            transform.parent.transform.position = locationTarget.position + _pickableItemSO.PickedPosition;
            transform.parent.transform.rotation = locationTarget.rotation * Quaternion.Euler(_pickableItemSO.PickedAngle);
        }
        public virtual void OrientItemInHand(Transform handLocation, int playerID) 
        {
            locationTarget = handLocation;
            Physics.IgnoreCollision(_itemCollider, PlayerManager.Instance.GetPlayer(playerID).GetComponent<Collider>(), true);
        }
    }
}

// enum with flags if u want it
/*[Flags]
public enum PickableItemStateEnum
{
    None = 0,               //000
    IsPossessed = 1 << 0,   //001
    IsInHand = 1 << 1       //010
}
public PickableItemStateEnum itemStateEnum = PickableItemStateEnum.None;

private void SetPossessed(bool isPossessed)
{
    if (isPossessed)
    {
        itemStateEnum |= PickableItemStateEnum.IsPossessed;
    }
    else
    {
        itemStateEnum &= ~PickableItemStateEnum.IsPossessed;
        itemStateEnum &= ~PickableItemStateEnum.IsInHand;
    }
}
private void SetInHand(bool isInHand)
{
    if (isInHand)
    {
        itemStateEnum |= PickableItemStateEnum.IsInHand;
        itemStateEnum &= ~PickableItemStateEnum.IsPossessed;
    }
    else
    {
        itemStateEnum &= ~PickableItemStateEnum.IsInHand;
    }
}*/
using UnityEngine;

public class PolyInteractable : InteractableItem
{
    [SerializeField] protected PolyInteractableOrder _order;

    public PolyInteractableOrder Order { get { return _order; } }
}

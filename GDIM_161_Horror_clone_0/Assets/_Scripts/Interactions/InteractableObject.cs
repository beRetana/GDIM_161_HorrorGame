using System;
using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{

    public virtual void Interact(int playerID)
    {
        Debug.Log($"Player {playerID} interactedWith Object {this.name}");
        return;
    }
    public void Detected(int playerID)
    {
        return;
    }
    public void StoppedDetecting(int playerID)
    {
        return;
    }
}

using UnityEngine;

public interface IInteractable
{
    public void Interact(int playerID);

    public void Detected(int playerID);

    public void StoppedDetecting(int playerID);
}

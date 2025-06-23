using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public void Interaction(int playerID, InputData context);

    public void Detected(int playerID);

    public void StoppedDetecting(int playerID);

    public NetworkIdentity GetNetworkID();
}

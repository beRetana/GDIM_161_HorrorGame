using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public void Interaction(int playerID, InputData context);

    public void StartedInteraction(int playerID, InputData context);

    public void CanceledInteraction(int playerID, InputData context);

    public void PerformedInteraction(int playerID, InputData context);

    public void Detected(int playerID);

    public void StoppedDetecting(int playerID);

    public void StopDetecting(int playerID);

    public void SetInteractive(bool intactive);

    public NetworkIdentity GetNetworkID();
}

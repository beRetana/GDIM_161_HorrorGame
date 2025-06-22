using Mirror;
using UnityEngine;

public interface IInteractable
{
    public void PerformedInteraction(int playerID);

    public void StartedInteraction(int playerID);

    public void CanceledInteraction(int playerID);

    public void Detected(int playerID);

    public void StoppedDetecting(int playerID);

    public NetworkIdentity GetNetworkID();
}

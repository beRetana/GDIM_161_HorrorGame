using Mirror;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class BonePiece : NetworkBehaviour
{
    [SyncVar(hook =nameof(ChangingState))] private bool m_Active;

    public void SetObjectActive(bool active)
    {
        if (isServer) m_Active = active;
        else CmdSetObjectActive(active);
    }

    private void ChangingState(bool newValue, bool oldState)
    {
        gameObject.SetActive(newValue);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetObjectActive(bool active)
    {
        m_Active = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Monster") return;

        if (isServer) DisablePiece();
        gameObject.SetActive(false);
    }

    [Server]
    private void DisablePiece()
    {
        NetworkServer.UnSpawn(gameObject);
    }
}

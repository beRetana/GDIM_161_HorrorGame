using Mirror;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class BonePiece : NetworkBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SetObjectActive(bool active)
    {
        if (isServer) RpcSetObjectActive(active);
        else CmdSetObjectActive(active);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetObjectActive(bool active)
    {
        RpcSetObjectActive(active);
    }

    [ClientRpc]
    public void RpcSetObjectActive(bool value)
    {
        gameObject.SetActive(value);
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

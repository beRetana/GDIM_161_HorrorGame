using Mirror;
using System.Collections;
using UnityEngine;

public class BonePiece : NetworkBehaviour
{
    public IEnumerator DisableTimer(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        if (!isServer) CmdDisablePiece();
        else DisablePiece();
    }

    [Command(requiresAuthority = false)]
    private void CmdDisablePiece()
    {
        DisablePiece();
    }

    [Server]
    private void DisablePiece()
    {
        NetworkServer.UnSpawn(gameObject);
        RpcDisablePiece();
    }

    [ClientRpc]
    private void RpcDisablePiece()
    {
        gameObject.SetActive(false);
    }
}

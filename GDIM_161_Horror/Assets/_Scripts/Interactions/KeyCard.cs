using Mirror;
using UnityEngine;

public class KeyCard : NetworkBehaviour
{
    public void SetKeyActive(bool active)
    {
        if (isServer) RpcSetKeyActive(active);
        else CmdSetKeyActive(active);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetKeyActive(bool active)
    {
        RpcSetKeyActive(active);
    }

    [ClientRpc]
    private void RpcSetKeyActive(bool active)
    {
        gameObject.SetActive(active);
    }
}

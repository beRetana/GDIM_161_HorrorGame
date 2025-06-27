using Mirror;
using UnityEngine;

public class LoadingBarUI : NetworkBehaviour
{
    [SerializeField] private InteractablePlayer player;

    private void Start()
    {
        if (player == null) Debug.LogWarning($"InteractablePlayer Not Set To An Instance.");
    }

    public void ResetAnimations()
    {
        if (isServer) RpcResetAnimations();
        else CmdResetAnimations();
    }

    [Command(requiresAuthority = false)]
    private void CmdResetAnimations()
    {
        RpcResetAnimations();
    }

    [ClientRpc]
    private void RpcResetAnimations()
    {
        player.ResetAnimations();
    }
}

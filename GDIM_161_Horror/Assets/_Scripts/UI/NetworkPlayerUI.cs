using Mirror;
using UnityEngine;

[RequireComponent(typeof(PlayerInteractableUI))]
public class NetworkPlayerUI : NetworkBehaviour
{
    private PlayerInteractableUI m_PlayerInteractUI;

    private void Start()
    {
        m_PlayerInteractUI = GetComponent<PlayerInteractableUI>();
    }

    public void StartHoldingUI()
    {
        if (isServer) RcpStartHoldingUI();
        else CmdStartHoldingUI();
    }

    [Command]
    private void CmdStartHoldingUI()
    {
        RcpStartHoldingUI();
    }

    [ClientRpc]
    private void RcpStartHoldingUI()
    {
        m_PlayerInteractUI.StartHoldingUI();
    }

    public void CancelHoldingUI()
    {
        if (isServer) RpcCancelHoldingUI();
        else CmdCancelHoldingUI();
    }

    [Command]
    private void CmdCancelHoldingUI()
    {
        RpcCancelHoldingUI();
    }

    [ClientRpc]
    private void RpcCancelHoldingUI()
    {
        m_PlayerInteractUI.CancelHoldingUI();
    }
}

using Mirror;
using OtherUtils;
using UnityEngine;

[RequireComponent(typeof(PlayerInteractionsHUD))]
public class NetworkPlayerUI : NetworkBehaviour, IDebugger
{
    private PlayerInteractionsHUD m_PlayerInteractUI;

    private bool m_Debugger;

    private void Start()
    {
        if (!isLocalPlayer) enabled = false;
        m_PlayerInteractUI = GetComponent<PlayerInteractionsHUD>();
    }

    public void DisplayInteractUI(string text)
    {
        Debugger($"Initial call to Display Interact UI");
        if (isServer) RpcDisplayInteractUI(text);
        else CmdDisplayInteractUI(text);
    }

    [Command]
    private void CmdDisplayInteractUI(string text)
    {
        Debugger($"COMMAND - Display Interact UI, text: {text}");
        RpcDisplayInteractUI(text);
    }

    [ClientRpc]
    private void RpcDisplayInteractUI(string text)
    {
        if (!isLocalPlayer) return;
        Debugger($"REPLICATE - Display Interact UI, text: {text}");
        m_PlayerInteractUI.DisplayInteractUI(text);
    }

    public void HideInteractUI()
    {
        Debugger($"Initial call to Hide Interact UI");
        if (isServer) RpcHideInteractUI();
        else CmdHideInteractUI();
    }

    [Command]
    private void CmdHideInteractUI()
    {
        Debugger($"COMMAND - Hide Interact UI");
        RpcHideInteractUI();
    }

    [ClientRpc]
    private void RpcHideInteractUI()
    {
        if (!isLocalPlayer) return;
        Debugger($"REPLICATE - Hide Interact UI");
        m_PlayerInteractUI.HideInteractUI();
    }

    public void StartHoldingUI()
    {
        Debugger($"Initial call to Start Holding UI");
        if (isServer) RcpStartHoldingUI();
        else CmdStartHoldingUI();
    }

    [Command]
    private void CmdStartHoldingUI()
    {
        Debugger($"COMMAND - Start Holding UI");
        RcpStartHoldingUI();
    }

    [ClientRpc]
    private void RcpStartHoldingUI()
    {
        if (!isLocalPlayer) return;
        Debugger($"REPLICATE - Start Holding UI");
        m_PlayerInteractUI.StartHoldingUI();
    }

    public void CancelHoldingUI()
    {
        Debugger($"Initial call to Cancel Holding UI");
        if (isServer) RpcCancelHoldingUI();
        else CmdCancelHoldingUI();
    }

    [Command]
    private void CmdCancelHoldingUI()
    {
        Debugger($"COMMAND - Cancel Holding UI");
        RpcCancelHoldingUI();
    }

    [ClientRpc]
    private void RpcCancelHoldingUI()
    {
        if (!isLocalPlayer) return;
        Debugger($"REPLICATE - Cancel Holding UI");
        m_PlayerInteractUI.CancelHoldingUI();
    }

    public void Debugger(object log)
    {
        if (m_Debugger) Debug.Log($"[{this.GetType().ToString()}]: {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_Debugger = active;
    }
}

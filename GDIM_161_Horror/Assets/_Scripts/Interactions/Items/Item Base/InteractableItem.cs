using UnityEngine;
using System;
using TMPro;
using Mirror;
using OtherUtils;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// This class allows items to be interacted with a player.
/// </summary>
public class InteractableItem : MonoBehaviour, IInteractable, IDebugger
{
    [SerializeField] protected NetworkIdentity _networkIdentity;
    [SerializeField] protected string m_DisplayText;
    
    protected bool m_IsInteractable;
    protected bool m_DebugEnabled;

    protected Action<int> OnInteractAction;

    protected virtual void Awake()
    {
        OnInteractAction = (int playerId) => Debug.Log($"Player: {playerId} Interacted");
        m_IsInteractable = true;
    }

    protected virtual void Start()
    {
        if (_networkIdentity == null) 
            _networkIdentity = transform.parent.GetComponent<NetworkIdentity>();
    }

    public virtual void SetInteractive(bool intactive)
    {
        m_IsInteractable = intactive;
    }

    public virtual void SetDisplayMessage(string text)
    {
        m_DisplayText = text;
    }

    public virtual void Interaction(int playerID, InputData context)
    {
        Debugger($"Player {playerID} is trying to interact" +
                 $"Active: {m_IsInteractable}, " +
                 $"Phase: {context.InputPhase}, " +
                 $"Type: {context.InputType}");

        if (!m_IsInteractable) return;

        switch (context.InputPhase)
        {
            case InputActionPhase.Started:
                StartedInteraction(playerID, context);
                break;
            case InputActionPhase.Canceled:
                CanceledInteraction(playerID, context);
                break;
            case InputActionPhase.Performed:
                PerformedInteraction(playerID, context);
                break;
        }
    }

    public virtual void StartedInteraction(int playerID, InputData context)
    {
        Debugger($"Starting Interaction");
    }

    public virtual void CanceledInteraction(int playerID, InputData context)
    {
        Debugger($"Canceling Interaction");
    }

    public virtual void PerformedInteraction(int playerID, InputData context)
    {
        Debugger($"Performing Interaction");
        OnInteractAction(playerID);
    }

    public virtual void SetInteractAction(Action<int> action)
    {
        OnInteractAction = action;
    }

    public virtual void Detected(int playerID)
    {
        if (!m_IsInteractable) return;
        PlayerManager.Instance.GetPlayer(playerID).
            GetComponent<NetworkPlayerUI>().DisplayInteractUI(m_DisplayText);
    }

    public virtual void StoppedDetecting(int playerID)
    {
        Debugger($"Player {playerID} stopped detecting");
        PlayerManager.Instance.GetPlayer(playerID).
            GetComponent<NetworkPlayerUI>().HideInteractUI();
    }

    public virtual void StopDetecting(int playerID)
    {
        Debugger($"Player {playerID} wants to stop detecting");
        PlayerManager.Instance.GetPlayer(playerID).
            GetComponent<NetworkPlayerUI>().HideInteractUI();
    }

    public NetworkIdentity GetNetworkID()
    {
        return _networkIdentity;
    }

    public void Debugger(object log)
    {
        if (m_DebugEnabled) Debug.Log($"[{this.GetType().ToString()}] {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_DebugEnabled = active;
    }
}

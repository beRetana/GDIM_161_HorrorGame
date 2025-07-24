using System;
using UnityEngine;
using OtherUtils;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InteractablePlayer : InteractableItem, IDebugger
{
    public event Action OnPlayerRecued;
    protected bool m_Loading;

    protected override void Start()
    {
        base.Start();
    }

    public override void StartedInteraction(int playerID, InputData context)
    {
        Debugger($"Started Rescuing");
        m_Loading = true;
        PlayerManager.Instance.GetPlayer(playerID).
            GetComponent<NetworkPlayerUI>().StartHoldingUI();
    }

    public override void CanceledInteraction(int playerID, InputData context)
    {
        if (context.InputType != InteractionType.Hold) return;
        Debugger($"Canceled Rescuing");
        m_Loading = false;
        PlayerManager.Instance.GetPlayer(playerID).
            GetComponent<NetworkPlayerUI>().CancelHoldingUI();
    }

    public override void PerformedInteraction(int playerID, InputData context)
    {
        if (context.InputType == InteractionType.Hold)
        {
            Debugger("Player Succesfully Rescued");
            OnPlayerRecued?.Invoke();
            PlayerManager.Instance.GetPlayer(playerID).
                GetComponent<PlayerDataTracker>().OnPlayerRezzed();
        }
        else
        {
            Debugger("Another Interaction was Succesful before Holding; Loading Cancelled");  
        }
        m_Loading = false;
        PlayerManager.Instance.GetPlayer(playerID).
                GetComponent<NetworkPlayerUI>().CancelHoldingUI();
    }

    public override void StoppedDetecting(int playerID)
    {
        base.StoppedDetecting(playerID);
        if (!m_Loading) return;
        m_Loading = false;
        PlayerManager.Instance.GetPlayer(playerID).
            GetComponent<NetworkPlayerUI>().CancelHoldingUI();
    }
}

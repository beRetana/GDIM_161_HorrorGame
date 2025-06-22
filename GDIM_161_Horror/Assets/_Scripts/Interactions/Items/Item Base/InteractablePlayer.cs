using System;
using UnityEngine;
using UnityEngine.InputSystem;
using OtherUtils;

public class InteractablePlayer : InteractableItem, IDebugger
{
    protected Action OnPlayerInteract;
    protected Coroutine m_Rescuing;
    protected PlayerControls m_PlayerControls;
    protected const string LOADING = "LOADING";

    protected override void Awake()
    {
        OnPlayerInteract = () => {  };
    }

    protected override void Start()
    {
        base.Start();
        m_PlayerControls = new();
    }

    public override void StartedInteraction(int playerID)
    {
        if (!_isInteractable) return;
        base.StartedInteraction(playerID);
        _uiAnimator.SetBool(FADE, false);
        _uiAnimator.SetBool(LOADING, true);
    }

    public override void CanceledInteraction(int playerID)
    {
        if (!_isInteractable) return;
        base.CanceledInteraction(playerID);
        _uiAnimator.SetBool(LOADING, false);
    }

    public override void PerformedInteraction(int playerID)
    {
        if (!_isInteractable) return;
        Debugger("Performed");
        Loaded();
    }

    public void SetPlayerInteraction(Action action)
    {
        Debugger("Interaction was set");
        OnPlayerInteract = action;
    }

    protected void Loaded()
    {
        Debugger("Player Succesfully Rescued");
        OnPlayerInteract?.Invoke();
    }
}

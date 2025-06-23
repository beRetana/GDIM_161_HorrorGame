using System;
using UnityEngine;
using OtherUtils;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InteractablePlayer : InteractableItem, IDebugger
{
    [SerializeField] protected GameObject m_TextDisplay;
    [SerializeField] protected GameObject m_ProgressDisplay;
    protected Action OnPlayerInteract;
    protected Coroutine m_Rescuing;
    protected PlayerControls m_PlayerControls;
    protected const string LOADING = "LOADING";
    protected const string RESET = "RESET";
    protected bool m_loading;

    protected override void Awake()
    {
        OnPlayerInteract = () => {  };
    }

    protected override void Start()
    {
        base.Start();
        m_PlayerControls = new();
        m_ProgressDisplay.SetActive(false);
    }

    protected virtual void StartedInteraction()
    {
        m_TextDisplay.SetActive(false);
        m_ProgressDisplay.SetActive(true);
        _uiAnimator.SetBool(LOADING, true);
        m_loading = true;
    }

    protected virtual void CanceledInteraction()
    {
        _uiAnimator.SetBool(LOADING, false);
    }

    public void ResetAnimations()
    {
        m_TextDisplay.SetActive(true);
        m_ProgressDisplay.SetActive(false);
        m_ProgressDisplay.GetComponent<Slider>().value = 0;
        _uiAnimator.SetTrigger(RESET);
        Loaded();
    }

    public override void Interaction(int playerID, InputAction.CallbackContext context)
    {
        if (!_isInteractable) return;

        switch (context.phase)
        {
            case InputActionPhase.Started:
                StartedInteraction(); break;
            case InputActionPhase.Canceled:
                if (context.interaction is not HoldInteraction) return;
                CanceledInteraction(); break;
        }
    }

    public override void Detected(int playerID)
    {
        if (m_loading) return;
        base.Detected(playerID);
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

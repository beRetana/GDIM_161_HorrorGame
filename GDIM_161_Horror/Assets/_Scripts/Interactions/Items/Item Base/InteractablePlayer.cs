using System;
using UnityEngine;
using OtherUtils;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InteractablePlayer : InteractableItem, IDebugger
{
    [SerializeField] protected Animator m_ProgressBarAnimator;
    [SerializeField] protected GameObject m_TextDisplay;
    [SerializeField] protected GameObject m_ProgressDisplay;
    protected Action OnPlayerInteract;
    protected Coroutine m_Rescuing;
    protected PlayerControls m_PlayerControls;
    protected const string LOADING = "START_LOADING";
    protected const string STOP = "STOP_LOADING";
    protected bool m_loading;

    protected override void Awake()
    {
        OnPlayerInteract = () => {  };
    }

    protected override void Start()
    {
        base.Start();
        m_ProgressDisplay.SetActive(false);
    }

    public override void StartedInteraction(int playerID, InputData context)
    {
        Debugger($"Started Rescuing");
        m_TextDisplay.SetActive(false);
        m_ProgressDisplay.SetActive(true);
        m_ProgressBarAnimator.SetTrigger(LOADING);
        m_loading = true;
    }

    public override void CanceledInteraction(int playerID, InputData context)
    {
        if (context.InputType != InteractionType.Hold) return;
        Debugger($"Canceled Rescuing");
        m_ProgressBarAnimator.SetTrigger(STOP);
        m_loading = false;
    }

    public override void PerformedInteraction(int playerID, InputData context)
    {
        switch (context.InputType)
        {
            case InteractionType.Hold:
                Debugger("Player Succesfully Rescued");
                OnPlayerInteract?.Invoke();
                OnPlayerInteract = null;
                break;
            case InteractionType.Tap:
                CanceledInteraction(playerID, context);
                break;
            case InteractionType.Other:
                CanceledInteraction(playerID, context);
                break;
        }
    }

    public void ResetAnimations()
    {
        m_TextDisplay.SetActive(true);
        m_ProgressDisplay.SetActive(false);
        m_ProgressDisplay.GetComponent<Slider>().value = 0;
        m_loading = false;
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
}

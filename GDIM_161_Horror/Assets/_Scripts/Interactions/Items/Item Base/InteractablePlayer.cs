using System;
using UnityEngine;
using OtherUtils;
using UnityEngine.UIElements;

public class InteractablePlayer : InteractableItem, IDebugger
{
    [SerializeField] protected GameObject m_TextDisplay;
    [SerializeField] protected GameObject m_ProgressDisplay;
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
        m_ProgressDisplay.SetActive(false);
    }

    public override void StartedInteraction(int playerID)
    {
        if (!_isInteractable) return;
        base.StartedInteraction(playerID);
        m_TextDisplay.SetActive(false);
        m_ProgressDisplay.SetActive(true);
        _uiAnimator.SetBool(LOADING, true);
    }

    public override void CanceledInteraction(int playerID)
    {
        if (!_isInteractable) return;
        base.CanceledInteraction(playerID);
        _uiAnimator.SetBool(LOADING, false);
    }

    public void ResetAnimations()
    {
        m_TextDisplay.SetActive(true);
        m_ProgressDisplay.SetActive(false);
        m_ProgressDisplay.GetComponent<Slider>().value = 0;
    }

    public override void PerformedInteraction(int playerID)
    {
        if (!_isInteractable) return;
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

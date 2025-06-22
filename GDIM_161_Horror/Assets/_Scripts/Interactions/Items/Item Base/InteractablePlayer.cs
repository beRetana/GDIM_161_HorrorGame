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

    private void OnDisable()
    {
        DisableInput();
    }

    protected void EnableInput()
    {
        Debugger("Enabled Input");
        m_PlayerControls.Player.Enable();
        m_PlayerControls.Player.Interact.canceled += RescueCancelled;
        m_PlayerControls.Player.Interact.performed += OnLoaded;
    }

    protected void DisableInput()
    {
        Debugger("Disabled Input");
        m_PlayerControls.Player.Interact.canceled -= RescueCancelled;
        m_PlayerControls.Player.Interact.performed -= OnLoaded;
        m_PlayerControls.Player.Disable();
    }

    public override void Interact(int playerID)
    {
        Debugger("Player interacted with me");
        _uiAnimator.SetBool(LOADING, true);
        EnableInput();
    }

    public void RescueCancelled(InputAction.CallbackContext context)
    {
        Debugger("Loading was Cacelled");
        _uiAnimator.SetBool(LOADING, false);
        DisableInput();
    }

    public void SetPlayerInteraction(Action action)
    {
        Debugger("Interaction was set");
        OnPlayerInteract = action;
    }

    protected void OnLoaded(InputAction.CallbackContext context)
    {
        Debugger("Player Succesfully Rescued");
        OnPlayerInteract?.Invoke();
        DisableInput();
    }
}

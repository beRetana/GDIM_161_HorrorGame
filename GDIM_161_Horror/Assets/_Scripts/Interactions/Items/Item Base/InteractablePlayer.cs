using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractablePlayer : InteractableItem
{
    protected Action OnPlayerInteract;
    protected Coroutine m_Rescuing;
    protected const string LOADING = "LOADING";
    protected bool m_IsRescuing;
    protected bool m_IsPlayerInteracting;

    protected override void Awake()
    {
        OnPlayerInteract = () => {  };
    }

    public override void Interact(int playerID)
    {
        m_IsPlayerInteracting = true;
        m_IsRescuing = true;
        _uiAnimator.SetBool(LOADING, true);
    }

    public void OnInteract(InputValue value)
    {
        if (!m_IsPlayerInteracting) return;

        if (!value.isPressed && m_IsRescuing)
        {
            m_IsRescuing = false;
            _uiAnimator.SetBool(LOADING, false);
            m_IsPlayerInteracting = false;
        }
    }

    public void SetPlayerInteraction(Action action)
    {
        OnPlayerInteract = action;
    }

    protected void OnLoaded()
    {
        m_IsPlayerInteracting = false;
        m_IsRescuing = false;
        OnPlayerInteract?.Invoke();
    }
}

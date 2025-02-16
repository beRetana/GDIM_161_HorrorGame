using System;
using UnityEngine;

public class Campfire : InteractableObject, IInteractable
{
    #region Variables
    [Header("Timer")]
    [SerializeField] private float timerInit;
    [SerializeField] private float tickSpeed = 1f;
    public float BurnoutTimer { get; private set; }


    public delegate void BurnedOut();
    public static event BurnedOut OnBurnedOut;

    #endregion

    void Awake()
    {
        BurnoutTimer = timerInit;
    }

    private void LateUpdate()
    {
        BurnoutTimer -= Time.deltaTime;
        if (BurnoutTimer < 0) OnBurnedOut?.Invoke();
    }

    public override void Interact(int playerID)
    {
        base.Interact(playerID);

    }
}

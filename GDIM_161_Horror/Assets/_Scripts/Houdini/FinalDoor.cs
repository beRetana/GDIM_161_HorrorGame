using Mirror;
using StarterAssets;
using System;
using UnityEngine;
using System.Collections.Generic;

public class FinalDoor : MoveDoors
{
    private HashSet<byte> m_PlayersCheckedIn;
    private FinalDoorInteractable m_Interactable;

    [SyncVar] private DoorState m_DoorState;

    protected int m_KeycardMax;

    public DoorState State => m_DoorState;
    public int KeycardCount => m_PlayersCheckedIn.Count;
    public int KeycardMax => m_KeycardMax;
    public static FinalDoor Instance;

    public enum DoorState
    {
        Locked,
        Alert,
        Unlocked
    }

    protected override void Start()
    {
        base.Start();

        m_Interactable = GetComponent<FinalDoorInteractable>();
        m_PlayersCheckedIn = new();
        m_DoorState = DoorState.Locked;
        m_KeycardMax = NewNetworkManager.NewSingleton.numPlayers;
    }

    [ClientRpc]
    private void SetRequiredNumber(int value)
    {
        m_KeycardMax = value;
    }

    public void OnStartedInteraction(int playerID)
    {
        switch (m_DoorState)
        {
            case DoorState.Locked:
                Debugger($"Player {playerID} Started Holding");
                PlayerManager.Instance.GetPlayer(playerID).
                    GetComponent<NetworkPlayerUI>().StartHoldingUI();
                break;
            case DoorState.Unlocked:
                
                Debugger($"Player {playerID} Started Tapping");
                PlayerManager.Instance.GetPlayer(playerID).
                    GetComponent<NetworkPlayerUI>().StartHoldingUI();
                break;
        }
    }

    public void OnCancelledInteraction(int playerID)
    {
        switch (m_DoorState)
        {
            case DoorState.Locked:
            case DoorState.Unlocked:
                Debugger($"Player {playerID} canceled their Interaction");
                PlayerManager.Instance.GetPlayer(playerID).
                    GetComponent<NetworkPlayerUI>().CancelHoldingUI();
                break;
        }
    }

    public void EnterAlertState(int playerID)
    {
        Debugger($"Player {playerID} successfully held; switching to {DoorState.Alert}");
        m_Interactable.SetDisplayMessage($"{m_KeycardMax - KeycardCount} More Keys Needed");
        m_DoorState = DoorState.Alert;
        CrazySequence();
        if (m_KeycardMax == 0) return;
        SetRequiredNumber(NewNetworkManager.NewSingleton.numPlayers);
    }

    private void ChangeDisplayTextUI(string text, NetworkPlayerUI playerUI)
    {
        m_Interactable.SetDisplayMessage(text);
        playerUI.DisplayInteractUI(text);
    }

    private void CrazySequence()
    {
        KeyCardInteractable[] keycards = FindObjectsByType<KeyCardInteractable>(FindObjectsSortMode.None);

        foreach (KeyCardInteractable keycard in keycards)
        {
            keycard.ChangeToAlertMode();
        }
        if (!isServer) return;
        MonsterData[] monsters = FindObjectsByType<MonsterData>(FindObjectsSortMode.None);
        Debugger($"Found {monsters.Length} monsters");
        foreach (MonsterData monster in monsters)
        {
            monster.SetAggressiveStats();
        }
    }

    public void OnPerformedInput(int playerID, InputData context)
    {
        if (isServer) RpcOnPerformedInput(playerID, context);
        else CmdOnPerformedInput(playerID, context);
    }

    [Command]
    private void CmdOnPerformedInput(int playerID, InputData context)
    {
        RpcOnPerformedInput(playerID, context);
    }

    [ClientRpc]
    private void RpcOnPerformedInput(int playerID, InputData context)
    {
        switch (m_DoorState)
        {
            case DoorState.Locked:
                PlayerManager.Instance.GetPlayer(playerID).
                        GetComponent<NetworkPlayerUI>().CancelHoldingUI();
                if (context.InputType != InteractionType.Hold) return;
                EnterAlertState(playerID);
                m_Interactable.SetDisplayMessage($"You Need A Keycard");
                break;
            case DoorState.Alert:
                if (context.InputType != InteractionType.Tap) return;
                Debugger($"Player {playerID} Trying to open Door");
                UnlockingDoor((byte)playerID);
                break;
            case DoorState.Unlocked:
                Debugger($"Player {playerID} successfully held; Opening Doors");
                PlayerManager.Instance.GetPlayer(playerID).
                    GetComponent<NetworkPlayerUI>().CancelHoldingUI();

                if (context.InputType != InteractionType.Hold) return;
                OpenDoors();
                GetComponent<Collider>().enabled = false;
                break;
        }
    }

    private void UnlockingDoor(byte playerID)
    {
        FirstPersonController player = PlayerManager.Instance.GetPlayer(playerID).GetComponent<FirstPersonController>();
        var playerUI = player.GetComponent<NetworkPlayerUI>();

        Debugger($"Player: {playerID} {(player.HasKeyCard ? "has keycard" : "does not have keycard")}");
        if (!player.HasKeyCard)
        {
            ChangeDisplayTextUI($"You Need A Keycard", playerUI);
            return;
        }

        if (!m_PlayersCheckedIn.Add(playerID))
        {
            ChangeDisplayTextUI($"One Keycard Per Person", playerUI);
            return;
        }

        if (m_PlayersCheckedIn.Count >= m_KeycardMax)
        {
            Debugger("Door Ready to Unlock!");
            m_DoorState = DoorState.Unlocked;
            ChangeDisplayTextUI($"HOLD To Open", playerUI);
            return;
        }

        ChangeDisplayTextUI($"{m_PlayersCheckedIn.Count}/{m_KeycardMax} Keycards Checked In", playerUI);
        Debugger($"Player: {playerID} has Checked in - {m_PlayersCheckedIn.Count}/{m_KeycardMax}!");
    }
}

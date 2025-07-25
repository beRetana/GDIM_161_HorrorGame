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
        if (!isServer) return;
        SetRequiredNumber(NewNetworkManager.NewSingleton.numPlayers);
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
        bool hasKeycard = PlayerManager.Instance.GetPlayer(playerID).
            GetComponent<FirstPersonController>().HasKeyCard;
        Debugger($"Player: {playerID} {(hasKeycard ? "has keycard" : "does not have keycard")}");
        if (!hasKeycard)
        {
            m_Interactable.SetDisplayMessage($"You Need A Keycard");
            return;
        }

        if (!m_PlayersCheckedIn.Add(playerID))
        {
            m_Interactable.SetDisplayMessage($"One Keycard Per Person");
            return;
        }

        Debugger($"Player: {playerID} has Checked in - {m_PlayersCheckedIn.Count}/{m_KeycardMax}!");
        if (m_PlayersCheckedIn.Count < m_KeycardMax) return;
        Debugger("Door Ready to Unlock!");
        m_DoorState = DoorState.Unlocked;
        m_Interactable.SetDisplayMessage($"HOLD To Open");
        PlayerManager.Instance.GetPlayer(playerID).GetComponent<NetworkPlayerUI>().DisplayInteractUI($"HOLD To Open");
    }
}

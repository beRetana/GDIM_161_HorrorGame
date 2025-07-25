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

        m_KeycardMax = NewNetworkManager.NewSingleton.numPlayers;
        m_Interactable = GetComponent<FinalDoorInteractable>();
        m_PlayersCheckedIn = new();
        m_DoorState = DoorState.Locked;
    }

    public bool IsPlayerCheckedIn(byte playerID)
    {
        return m_PlayersCheckedIn.Contains(playerID);
    }

    private void UpdateState(DoorState state)
    {
        if (isServer) RpcUpdateState(state);
        else CmdUpdateState(state);
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateState(DoorState state)
    {
        RpcUpdateState(state);
    }

    [ClientRpc]
    private void RpcUpdateState(DoorState state)
    {
        m_DoorState = state;
    }

    private void UpdateCheckedInList(byte playerID)
    {
        if (isServer) RpcUpdateCheckedInList(playerID);
        else CmdUpdateCheckedInList(playerID);
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateCheckedInList(byte playerID)
    {
        RpcUpdateCheckedInList(playerID);
    }

    [ClientRpc]
    private void RpcUpdateCheckedInList(byte playerID)
    {
        m_PlayersCheckedIn.Add(playerID);
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

    public bool TryUnlockDoor(byte playerID)
    {
        UpdateCheckedInList(playerID);
        bool result = KeycardCount >= m_KeycardMax;
        if (result)
        {
            UpdateState(DoorState.Unlocked);
        }
        return result;
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
                break;
            case DoorState.Alert:
                if (context.InputType != InteractionType.Tap) return;
                Debugger($"Player {playerID} Unlocking Door");
                UnlockingDoor((byte)playerID);
                break;
            case DoorState.Unlocked:
                Debugger($"Player {playerID} successfully held; Opening Doors");
                PlayerManager.Instance.GetPlayer(playerID).
                    GetComponent<NetworkPlayerUI>().CancelHoldingUI();

                if (context.InputType != InteractionType.Hold) return;
                OpenDoors();
                break;
        }
    }

    private void UnlockingDoor(byte playerID)
    {
        bool hasKeycard = PlayerManager.Instance.GetPlayer(playerID).
            GetComponent<FirstPersonController>().HasKeyCard;

        if (!hasKeycard)
        {
            m_Interactable.SetDisplayMessage($"You Need A Keycard");
            return;
        }

        if (IsPlayerCheckedIn(playerID))
        {
            m_Interactable.SetDisplayMessage($"One Keycard Per Person");
            return;
        }

        Debugger($"Player {playerID} successfully checked in; " +
                 $"Increasing count to {KeycardCount}");

        if (!TryUnlockDoor(playerID)) return;

        m_Interactable.SetDisplayMessage($"HOLD To Open");
    }
}

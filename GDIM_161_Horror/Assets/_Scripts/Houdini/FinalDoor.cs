using Mirror;
using StarterAssets;
using System;
using UnityEngine;

public class FinalDoor : MoveDoors
{
    [SerializeField] protected int m_KeycardMax = 4;

    private bool[] m_PlayersCheckedIn;

    [SyncVar] private DoorState m_DoorState;
    [SyncVar] private int m_KeycardCount;

    public DoorState State => m_DoorState;
    public int KeycardCount => m_KeycardCount;
    public int KeycardMax => m_KeycardMax;
    public static FinalDoor Instance;
    public event Action OnDoorEnteredAlert;

    public enum DoorState
    {
        Locked,
        Alert,
        Unlocked
    }

    protected override void Start()
    {
        base.Start();
        m_PlayersCheckedIn = new bool[m_KeycardMax];
        m_DoorState = DoorState.Locked;

        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public bool IsPlayerCheckedIn(int playerID)
    {
        return m_PlayersCheckedIn[playerID];
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

    private void UpdateCheckedInList(int playerID)
    {
        if (isServer) RpcUpdateCheckedInList(playerID);
        else CmdUpdateCheckedInList(playerID);
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateCheckedInList(int playerID)
    {
        RpcUpdateCheckedInList(playerID);
    }

    [ClientRpc]
    private void RpcUpdateCheckedInList(int playerID)
    {
        m_PlayersCheckedIn[playerID] = true;
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
        CrazySequence();
        UpdateState(DoorState.Alert);
    }

    private void CrazySequence()
    {
        KeyCard[] keycards = FindObjectsByType<KeyCard>(FindObjectsSortMode.None);

        foreach (KeyCard keycard in keycards)
        {
            keycard.ChangeToAlertMode();
        }

        MonsterData[] monsters = FindObjectsByType<MonsterData>(FindObjectsSortMode.None);
        Debugger($"Found {monsters.Length} monsters");
        foreach (MonsterData monster in monsters)
        {
            monster.SetAggressiveStats();
        }
    }

    public bool TryUnlockDoor(int playerID)
    {
        UpdateCheckedInList(playerID);
        ++m_KeycardCount;
        bool result = m_KeycardCount >= m_KeycardMax;
        if (result)
        {
            UpdateState(DoorState.Unlocked);
        }
        return result;
    }

    [ClientRpc]
    protected override void RpcOpenDoors()
    {
        GetComponent<Collider>().enabled = false;
        base.RpcOpenDoors();
    }
}

using Mirror;
using System;
using UnityEngine;

public class PlayerDataTracker : NetworkBehaviour
{
    private Vector3 m_SavedPosition;
    private int[] m_TimePerFloor = new int[5];
    private float m_StartTime;
    private ulong m_PlayerSteamID;
    private string m_PlayerName;

    [SyncVar] private float m_TotalTime;
    [SyncVar] private ushort m_KnockedDownCount;
    [SyncVar] private ushort m_RezzedUpCount;
    [SyncVar] private int m_TrialNumber;

    public Vector3 SavedPosition { get { return m_SavedPosition; } 
                                   set { m_SavedPosition = value; } }
    public ulong PlayerSteamID => m_PlayerSteamID;
    public string PlayerName => m_PlayerName;   
    public int[] TimesPerFloor => m_TimePerFloor;
    public float TotalTime => m_TotalTime;
    public int TrialNumber => m_TrialNumber;
    public ushort KnockedDownCount => m_KnockedDownCount;
    public ushort RezzedUpCount => m_RezzedUpCount;

    private void Start()
    {
        m_SavedPosition = Vector3.zero;
        m_StartTime = Time.time;
        PlayerObjectController playerController = GetComponent<PlayerObjectController>();
        m_PlayerSteamID = playerController.PlayerSteamID;
        m_PlayerName = playerController.PlayerName;
        if (!isServer) return;
        m_TrialNumber = UnityEngine.Random.Range(1000, 10000);
    }

    public void OnReachedNewFloor(byte floorNum)
    {
        float previousTime = (floorNum != 0) ? m_TimePerFloor[--floorNum] : m_StartTime;
        int timeStamp = (int)(Time.time - previousTime);

        if (isServer) RpcOnReachedNewFloor(timeStamp, floorNum);
        else CmdOnReachedNewFloor(timeStamp, floorNum);
    }

    [Command]
    private void CmdOnReachedNewFloor(int timeStamp, byte floorNum)
    {
        RpcOnReachedNewFloor(timeStamp, floorNum);
    }

    [ClientRpc]
    private void RpcOnReachedNewFloor(int timeStamp, byte floorNum)
    {
        m_TimePerFloor[floorNum] = timeStamp;
    }

    public void OnEndGame()
    {
        if (isServer) m_TotalTime = Time.time - m_StartTime;
        else CmdOnEndGame(Time.time - m_StartTime);
    }

    [Command]
    private void CmdOnEndGame(float timeStamp)
    {
        m_TotalTime = timeStamp;
    }

    public void OnPlayerKnocked()
    {
        if (!isServer) CmdOnPlayerKnocked();
        else ++m_KnockedDownCount;
    }

    [Command]
    private void CmdOnPlayerKnocked()
    {
        ++m_KnockedDownCount;
    }

    public void OnPlayerRezzed()
    {
        if (!isServer) CmdOnPlayerRezzed();
        else ++m_RezzedUpCount;
    }

    [Command]
    private void CmdOnPlayerRezzed()
    {
        ++m_RezzedUpCount;
    }
}

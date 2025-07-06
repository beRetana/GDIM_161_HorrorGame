using Mirror;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using StarterAssets;

public class PlayerDataTracker : NetworkBehaviour
{
    private PlayerObjectController m_PlayerController;
    private FirstPersonController m_FirstPersonController;

    private Vector3 m_SavedPosition;

    private ulong[] m_TimePerFloor = new ulong[5];
    private ulong m_StartTime;
    private ulong m_PlayerSteamID;
    private string m_PlayerName;

    [SyncVar] private ulong m_TotalTime;
    [SyncVar] private ushort m_KnockedDownCount;
    [SyncVar] private ushort m_RezzedUpCount;
    private static ushort m_TrialNumber;

    public Vector3 SavedPosition { get { return m_SavedPosition; } 
                                   set { m_SavedPosition = value; } }
    public ulong PlayerSteamID => m_PlayerSteamID;
    public string PlayerName => m_PlayerName;   
    public ulong[] TimesPerFloor => m_TimePerFloor;
    public ulong TotalTime => m_TotalTime;
    public ushort TrialNumber => m_TrialNumber;
    public ushort KnockedDownCount => m_KnockedDownCount;
    public ushort RezzedUpCount => m_RezzedUpCount;

    private void Start()
    {
        m_PlayerController = GetComponent<PlayerObjectController>();
        m_FirstPersonController = GetComponent<FirstPersonController>();
        m_FirstPersonController.OnPlayerUp += OnPlayerKnocked;
        SceneManager.sceneLoaded += OnLoadedGameScene;
        SceneManager.sceneLoaded += OnReturnToLobby;
    }

    private void OnDestroy()
    {
        m_FirstPersonController.OnPlayerUp -= OnPlayerKnocked;
        SceneManager.sceneLoaded -= OnReturnToLobby;
        SceneManager.sceneLoaded -= OnLoadedGameScene;
    }

    private void OnLoadedGameScene(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != NewNetworkManager.NewSingleton.GameplaySceneName) return;

        m_StartTime = (ulong) Time.time;
        m_PlayerSteamID = m_PlayerController.PlayerSteamID;
        m_PlayerName = m_PlayerController.PlayerName;
        m_SavedPosition = Vector3.zero;
        if (!isServer || !isLocalPlayer) return;
        m_TrialNumber = (ushort)UnityEngine.Random.Range(1000, 10000);
        RpcSetTrialNumber(m_TrialNumber);
    }

    [ClientRpc]
    private void RpcSetTrialNumber(ushort number)
    {
        m_TrialNumber = number;
    }

    private void OnReturnToLobby(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != NewNetworkManager.NewSingleton.GetLobbyScene()) return;

        m_StartTime = 0;
        m_RezzedUpCount = 0;
        m_TotalTime = 0;
        m_TrialNumber = 0;
        
        for (byte i = 0; i < m_TimePerFloor.Length; ++i)
        {
            m_TimePerFloor[i] = 0;
        }
    }

    public void OnReachedNewFloor(byte floorNum)
    {
        float previousTime = (floorNum != 0) ? m_TimePerFloor[--floorNum] : m_StartTime;
        ulong timeStamp = (ulong)(Time.time - previousTime);

        if (isServer) RpcOnReachedNewFloor(timeStamp, floorNum);
        else CmdOnReachedNewFloor(timeStamp, floorNum);
    }

    [Command]
    private void CmdOnReachedNewFloor(ulong timeStamp, byte floorNum)
    {
        RpcOnReachedNewFloor(timeStamp, floorNum);
    }

    [ClientRpc]
    private void RpcOnReachedNewFloor(ulong timeStamp, byte floorNum)
    {
        m_TimePerFloor[floorNum] = timeStamp;
    }

    public void EndGame()
    {
        if (isServer) m_TotalTime = (ulong)Time.time - m_StartTime;
        else CmdOnEndGame((ulong)Time.time - m_StartTime);
    }

    [Command(requiresAuthority = false)]
    private void CmdOnEndGame(ulong timeStamp)
    {
        m_TotalTime = timeStamp;
    }

    private void OnPlayerKnocked(bool isPlayerUp)
    {
        if (isPlayerUp) return;
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

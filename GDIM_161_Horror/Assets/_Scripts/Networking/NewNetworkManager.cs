using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using OtherUtils;
using System;

public class NewNetworkManager : NetworkManager, IDebugger
{
    [Header("Game Settings"), Space(5f)]
    [SerializeField] private PlayerObjectController _playerController;
    [SerializeField] private string m_LabSceneName;

    public event Action OnPlayersLoadedScene;

    private NetworkStartPosition[] m_SpawnPoints;
    private string m_GameplaySceneName = "BUILD_1";
    private int m_PlayersCount = 0;
    private int m_LoadedScenePlayerCount = 0;
    private bool m_PlayersReady;
    private bool m_Debugger;

    public string GameplaySceneName => m_GameplaySceneName;
    public string LabSceneName => m_LabSceneName;
    public int SpawnCount => m_PlayersCount;
    public bool PlayersReady => m_PlayersReady;
    public static NewNetworkManager NewSingleton => (NewNetworkManager.singleton as NewNetworkManager);
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();
    public NetworkStartPosition[] SpawnPoints { get {return m_SpawnPoints; } }

    public override void Start()
    {
        base.Start();
        SceneManager.sceneLoaded += UpdateLocationList;
    }
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        string sceneName = SceneManager.GetActiveScene().name;

        if (GetMainMenuScene() == sceneName) return;
        Debugger($"Active Scene: {sceneName}");
        
        m_PlayersReady = false;
        ++m_LoadedScenePlayerCount;
        Debugger($"Ready Players: {m_LoadedScenePlayerCount}");
        
        if (m_LoadedScenePlayerCount < numPlayers) return;
        Debugger($"Loading Locations and resetting values");
        
        UpdateLocationList();
        OnPlayersLoadedScene?.Invoke();
        m_LoadedScenePlayerCount = 0;
        m_PlayersReady = true;
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == GetSceneName(onlineScene))
        {
            PlayerObjectController GamePlayerInstance = Instantiate(_playerController,
                                    m_SpawnPoints[m_PlayersCount].transform.position,
                                    m_SpawnPoints[m_PlayersCount].transform.rotation);

            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerID = GamePlayers.Count;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);
            
            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
            LobbyController.Instance.UpdatePlayerList();
            ++m_PlayersCount;
        }
    }

    private void UpdateLocationList(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == GetMainMenuScene()) return;
        Debugger($"Updating the Location List");
        UpdateLocationList();
    }

    public void UpdateLocationList()
    {
        m_SpawnPoints = FindObjectsByType<NetworkStartPosition>(FindObjectsSortMode.InstanceID);
    }

    public void LoadMazeScene()
    {
        ChangeScene(m_GameplaySceneName);
    }

    public void LoadLabScene()
    {
        ChangeScene(m_LabSceneName);
    }

    public void LoadLobbyScene()
    {
        ChangeScene(GetSceneName(onlineScene));
    }

    private void ChangeScene(string sceneName)
    {
        ServerChangeScene(sceneName);
    }

    public void SetGameSceneName(string name)
    {
        m_GameplaySceneName = name;
    }

    public string GetLobbyScene()
    {
        return GetSceneName(onlineScene);
    }

    public string GetMainMenuScene()
    {
        return GetSceneName(offlineScene);
    }

    public bool IsGameplayScene(string sceneName)
    {
        return sceneName == m_GameplaySceneName || sceneName == m_LabSceneName;
    }

    public override void OnStopHost()
    {
        SceneManager.LoadScene(offlineScene);
    }

    public override void OnStopClient()
    {
        SceneManager.LoadScene(offlineScene);
    }

    /*
        Parse the name file-route of a scene to only get the name then return that.
    */
    private static string GetSceneName(string name)
    {
        int folder = name.LastIndexOf('/') + 1;
        return name.Substring(folder, name.IndexOf('.') - folder);
    }

    public void Debugger(object log)
    {
        if (m_Debugger) Debug.Log($"[{this.GetType().ToString()}]: {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_Debugger = active;
    }
}

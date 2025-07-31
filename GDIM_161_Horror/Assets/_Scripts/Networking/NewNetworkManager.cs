using Mirror;
using OtherUtils;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewNetworkManager : NetworkManager, IDebugger
{
    [Header("Game Settings"), Space(5f)]
    [SerializeField] private PlayerObjectController _playerController;
    [SerializeField] private string m_GameplaySceneName = "BUILD_1";
    [SerializeField] private string[] m_LabSceneNames;

    public event Action OnPlayersServerReady;
    public event Action OnPlayerConnected;
    public event Action OnPlayerDisconnected;

    private bool m_PlayersReady;
    private bool m_Debugger;

    public string GameplaySceneName => m_GameplaySceneName;
    public string[] LabSceneName => m_LabSceneNames;
    public int SpawnCount => numPlayers;
    public bool PlayersReady => m_PlayersReady;
    public static NewNetworkManager NewSingleton => (NewNetworkManager.singleton as NewNetworkManager);
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    public override void Start()
    {
        base.Start();
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnPlayersServerReady = () => Debugger($"All {numPlayers} players are ready");
        Debug.Log("[NewNetworkManager]: Script Started");

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);
        RefreshStartingLocations();
        Debugger("Server Changed Scenes");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debugger("Server Started");
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        if (conn.identity != null )
        {
            Transform location = GetStartPosition();
            conn.identity.GetComponent<NetworkTransformReliable>().
                RpcTeleport(location.position, location.rotation);
        }

        Debugger($"There is {numPlayers} and {startPositionIndex} are ready");
        
        m_PlayersReady = ArePlayersReady();
        
        if (m_PlayersReady)
        {
            OnPlayersServerReady?.Invoke();
        }

        Debugger("Client Is Server Ready");
        return;
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name != GetLobbySceneName()) return;

        Transform startPos = GetStartPosition();
        PlayerObjectController player = Instantiate(_playerController, startPos.position, startPos.rotation);
        player.ConnectionID = conn.connectionId;
        player.PlayerID = GamePlayers.Count;
        player.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

        NetworkServer.AddPlayerForConnection(conn, player.gameObject);
        LobbyController.Instance.UpdatePlayerList();
        OnPlayerConnected?.Invoke();
        Debugger("Player Added to Server");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        PlayerObjectController player;
        if (conn.identity?.TryGetComponent<PlayerObjectController>(out player) == null) return;
        GamePlayers.Remove(player);
        --startPositionIndex;
        OnPlayerDisconnected?.Invoke();
        Debugger($"Player: {player.PlayerID}" + $" disconnected from server.");

        base.OnServerDisconnect(conn);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GetMainMenuScene() == scene.name)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (IsGameplayScene(scene.name))
        {
            RefreshStartingLocations();
        }
    }

    public bool ArePlayersReady()
    {
        return numPlayers != 0 && startPositionIndex % numPlayers == 0;
    }

    public void RefreshStartingLocations()
    {
        startPositions.Clear();
        startPositionIndex = 0;

        NetworkStartPosition[] startingObjects = FindObjectsByType<NetworkStartPosition>(FindObjectsSortMode.None);
        
        foreach (NetworkStartPosition startObject in startingObjects)
        {
            RegisterStartPosition(startObject.transform);
        }
    }

    public void LoadMazeScene()
    {
        ChangeScene(m_GameplaySceneName);
    }

    public void LoadLabScene()
    {
        int lab = UnityEngine.Random.Range(0, m_LabSceneNames.Length);
        ChangeScene(m_LabSceneNames[lab]);
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

    public string GetLobbySceneName()
    {
        return GetSceneName(onlineScene);
    }

    public string GetMainMenuScene()
    {
        return GetSceneName(offlineScene);
    }

    public bool IsGameplayScene(string sceneName)
    {
        foreach (string lab in m_LabSceneNames)
        {
            if (lab == sceneName) return true;
        }
        return sceneName == m_GameplaySceneName;
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

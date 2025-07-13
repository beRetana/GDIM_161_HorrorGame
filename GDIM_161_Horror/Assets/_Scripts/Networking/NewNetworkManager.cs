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
    [Space(5f)]
    [SerializeField] private PlayerObjectController _playerController;
    [SerializeField] private Transform[] _spawnPoints;
    [Space(5f)]
    [SerializeField] private string m_GameplaySceneName = "BUILD_1";

    public event Action OnPlayersLoadedScene;

    private int m_PlayersCount = 0;
    private int m_LoadedScenePlayerCount = 0;
    private bool m_PlayersReady;
    private bool m_Debugger;

    public string GameplaySceneName => m_GameplaySceneName;
    public int SpawnCount => m_PlayersCount;
    public bool PlayersReady => m_PlayersReady;
    public static NewNetworkManager NewSingleton => (NewNetworkManager.singleton as NewNetworkManager);

    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SetPlayersPosition;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SetPlayersPosition;
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        if (!IsGameplayScene(SceneManager.GetActiveScene().name)) return;

        ++m_LoadedScenePlayerCount;
        m_PlayersReady = false;

        if (m_LoadedScenePlayerCount == m_PlayersCount)
        {
            OnPlayersLoadedScene?.Invoke();
            m_LoadedScenePlayerCount = 0;
            m_PlayersReady = true;
        }
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == GetSceneName(onlineScene))
        {
            PlayerObjectController GamePlayerInstance = Instantiate(_playerController, 
                                    _spawnPoints[m_PlayersCount].position, _spawnPoints[m_PlayersCount].rotation);
            ++m_PlayersCount;

            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    private void SetPlayersPosition(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == GetMainMenuScene()) return;

        PlayerBase[] players = FindObjectsByType<PlayerBase>(FindObjectsSortMode.None);
        NetworkStartPosition[] startingPositions = FindObjectsByType<NetworkStartPosition>(FindObjectsSortMode.None);
        m_PlayersCount = players.Length;

        Debugger($"Spawning {m_PlayersCount} Players");
        for (int i = 0; i < players.Length; ++i)
        {
            
            Debugger($"List size is: {_spawnPoints.Length}");
            Debugger($"Spawing player: {players[i].gameObject.name} " +
                     $"at location: {startingPositions[i].transform.position}");

            players[i].transform.position = startingPositions[i].transform.position;
            players[i].transform.rotation = startingPositions[i].transform.rotation;
        }
    }

    public void StartGame(string SceneName)
    {
        ChangeScene(m_GameplaySceneName);
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
        return sceneName == m_GameplaySceneName;
    }

    public override void OnStopHost()
    {
        SceneManager.LoadScene(offlineScene);
        SceneManager.sceneLoaded -= SetPlayersPosition;
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

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

    private int _spawnCount = 0;
    private int m_LoadedScenePlayerCount = 0;
    private bool m_PlayersReady;
    private bool m_Debugger;

    public string GameplaySceneName => m_GameplaySceneName;
    public bool PlayersReady => m_PlayersReady;
    public static NewNetworkManager NewSingleton => (NewNetworkManager.singleton as NewNetworkManager);

    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    public override void Start()
    {
        base.Start();
        SceneManager.sceneLoaded += SetPlayersPosition;
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        if (!IsGameplayScene(SceneManager.GetActiveScene().name)) return;

        ++m_LoadedScenePlayerCount;
        m_PlayersReady = false;

        if (m_LoadedScenePlayerCount == _spawnCount)
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
                                    _spawnPoints[_spawnCount].position, _spawnPoints[_spawnCount].rotation);
            ++_spawnCount;

            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    private void SetPlayersPosition(Scene scene, LoadSceneMode mode)
    {
        Debugger($"Spawning {_spawnCount} Players");
        PlayerBase[] players = FindObjectsByType<PlayerBase>(FindObjectsSortMode.None);
        for (int i = 0; i < players.Length; ++i)
        {
            
            Debugger($"List size is: {_spawnPoints.Length}");
            Debugger($"Spawing player: {players[i].gameObject.name} at location: {_spawnPoints[i].position}");
            players[i].transform.position = _spawnPoints[i].position;
            players[i].transform.rotation = _spawnPoints[i].rotation;
        }
    }

    public void StartGame(string SceneName)
    {
        ServerChangeScene(m_GameplaySceneName);
    }

    public void LoadLobbyScene()
    {
        ServerChangeScene(GetSceneName(onlineScene));
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

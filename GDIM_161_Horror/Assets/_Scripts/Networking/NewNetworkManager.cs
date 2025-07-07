using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
using OtherUtils;

public class NewNetworkManager : NetworkManager, IDebugger
{
    [Space(5f)]
    [SerializeField] private PlayerObjectController _playerController;
    [SerializeField] private Transform[] _spawnPoints;
    [Space(5f)]
    [SerializeField] private string m_GameplaySceneName = "BUILD_1";

    private int _spawnCount = 0;
    private bool m_Debugger;

    public string GameplaySceneName => m_GameplaySceneName;
    public static NewNetworkManager NewSingleton => (NewNetworkManager.singleton as NewNetworkManager);

    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    public override void Start()
    {
        base.Start();
        SceneManager.sceneLoaded += SetPlayersPosition;
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
        if (!IsGameplayScene(scene.name)) return;

        Debugger($"Spawning {_spawnCount} Players");
        for (int i = 0; i < _spawnCount; ++i)
        {
            PlayerBase player = PlayerManager.Instance.GetPlayer(i);
            Debugger($"Spawing player: {player.gameObject.name} at location: {_spawnPoints[i].position}");
            player.transform.position = _spawnPoints[i].position;
            player.transform.rotation = _spawnPoints[i].rotation;
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

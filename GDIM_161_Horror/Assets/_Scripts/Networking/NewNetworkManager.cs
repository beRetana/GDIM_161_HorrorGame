using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class NewNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController _playerController;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private bool m_Debugger;

    private int _spawnCount = 0;

    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == GetSceneName(onlineScene))
        {
            PlayerObjectController GamePlayerInstance = Instantiate(_playerController, 
                                    _spawnPoints[_spawnCount].position, _spawnPoints[_spawnCount].rotation);
            _spawnCount++;

            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);

            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
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
    private string GetSceneName(string name)
    {
        int folder = name.LastIndexOf('/') + 1;
        return name.Substring(folder, name.IndexOf('.') - folder);
    }

    private void Debugger(object log)
    {
        if (m_Debugger) Debug.Log(log);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;
public class NewNetworkManager : NetworkManager
{
    [SerializeField] private PlayerNetworkController _playerNetworkController;
    [SerializeField] private string _lobbyScene = "Lobby";

    private List<PlayerNetworkController> _playersInGame = new List<PlayerNetworkController>();
    public List<PlayerNetworkController> PlayersInGame { get { return _playersInGame; } }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name != _lobbyScene) return;

        PlayerNetworkController playerInstance = Instantiate(_playerNetworkController);

        playerInstance.ConnectionID = conn.connectionId;
        playerInstance.PlayerIdNumber = _playersInGame.Count + 1;
        playerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, _playersInGame.Count);
        playerInstance.PlayerName = SteamFriends.GetFriendPersonaName((CSteamID)playerInstance.PlayerSteamID);

        NetworkServer.AddPlayerForConnection(conn, playerInstance.gameObject);
        playerInstance.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
        Debug.Log("Player Instantiated " + _playersInGame.Count);
    }

    public void StartGame(string SceneName) { ServerChangeScene(SceneName); }
}

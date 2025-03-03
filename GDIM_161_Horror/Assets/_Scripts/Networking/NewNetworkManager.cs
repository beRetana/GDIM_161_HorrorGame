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

    public List<PlayerNetworkController> PlayersInGame { get; } = new List<PlayerNetworkController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name != _lobbyScene) return;

        PlayerNetworkController playerInstance = Instantiate(_playerNetworkController);

        playerInstance.ConnectionID = conn.connectionId;
        playerInstance.PlayerIdNumber = PlayersInGame.Count + 1;
        playerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, PlayersInGame.Count);

        NetworkServer.AddPlayerForConnection(conn, playerInstance.gameObject);
        playerInstance.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
        PlayersInGame.Add(playerInstance);

        LobbyController.Instance.UpdatePlayerList();
    }

    public void StartGame(string SceneName) { ServerChangeScene(SceneName); }
}

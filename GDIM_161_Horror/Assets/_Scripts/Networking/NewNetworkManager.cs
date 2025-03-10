using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class NewNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController _playerController;
    [SerializeField] private string _lobby_scene_name = "Lobby_Brandon";

    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == _lobby_scene_name)
        {
            PlayerObjectController GamePlayerInstance = Instantiate(_playerController);

            GamePlayerInstance.ConnectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
            GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);

            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public void StartGame(string SceneName)
    {
        ServerChangeScene(SceneName);
    }
}

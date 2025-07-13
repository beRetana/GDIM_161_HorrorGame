
using Mirror;
using Player;
using StarterAssets;
using Steamworks;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerObjectController : NetworkBehaviour
{
    public static PlayerObjectController LocalInstance { get; private set; }

    // Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerID;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;

    private NewNetworkManager manager;
    
    private NewNetworkManager Manager
    {
        get
        {
            if (manager != null) 
            {
                return manager;
            }
            return manager = NewNetworkManager.singleton as NewNetworkManager;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += SetPlayerLocation;
    }

    private void OnEnable()
    {
        if (NewNetworkManager.NewSingleton.PlayersReady)
        {
            SetPlayerLocation();
        }
        else
        {
            NewNetworkManager.NewSingleton.OnPlayersLoadedScene += SetPlayerLocation;
        }
    }

    private void OnDisable()
    {
        NewNetworkManager.NewSingleton.OnPlayersLoadedScene -= SetPlayerLocation;
    }

    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            this.Ready = newValue;
        }

        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    private void SetPlayerLocation(Scene scene, LoadSceneMode mode)
    {
        if (NewNetworkManager.NewSingleton.IsGameplayScene(scene.name)) return;
        SetPlayerLocation();
    }

    public void SetPlayerLocation()
    {
        Transform location = NewNetworkManager.NewSingleton.SpawnPoints[PlayerID].transform;
        Debug.Log($"Setting Player location to {location.position} {location.rotation}");

        if (!isServer) CmdSetPlayerLocation(location.position, location.rotation);
        else RpcPlayerLocation(location.position, location.rotation);
    }

    [Command]
    private void CmdSetPlayerLocation(Vector3 position, Quaternion rotation)
    {
        RpcPlayerLocation(position, rotation);
    }

    [ClientRpc]
    private void RpcPlayerLocation(Vector3 position, Quaternion rotation)
    {
        Debug.Log($"Setting Player location to {position} {rotation}");

        transform.position = position;
        transform.rotation = rotation;
    }

    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }

    public void ChangeReady()
    {
        if (isOwned)
        {
            CmdSetPlayerReady();
        }
    }

    public override void OnStartAuthority()
    {
        LocalInstance = this;
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
        CmdSetPlayerName(SteamFriends.GetPersonaName());
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        PlayerManager.Instance.AttemptAddPlayer(GetComponent<FirstPersonController>());
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        PlayerManager.Instance.RemovePlayer(GetComponent<HandInventory>().PlayerID);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        this.PlayerNameUpdate(this.PlayerName, playerName);
    }

    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
        {
            this.PlayerName = newValue;
        }

        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public void CanStartGame(string SceneName)
    {
        if (isOwned)
        {
            CmdCanStartGame(SceneName);
        }
    }

    [Command]
    public void CmdCanStartGame(string SceneName)
    {
        manager.StartGame(SceneName);
    }
}

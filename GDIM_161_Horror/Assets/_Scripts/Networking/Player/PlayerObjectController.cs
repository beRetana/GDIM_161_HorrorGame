
using Mirror;
using OtherUtils;
using Player;
using StarterAssets;
using Steamworks;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerObjectController : NetworkBehaviour, IDebugger
{
    public static PlayerObjectController LocalInstance { get; private set; }

    // Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerID;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;

    [SyncVar] private Vector3 m_LobbyPosition;
    [SyncVar] private Quaternion m_LobbyRotation;

    private bool m_Debugger;

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
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SetPlayerLocation;
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
        Debug.Log($"DISABLES");
        NewNetworkManager.NewSingleton.OnPlayersLoadedScene -= SetPlayerLocation;
        SceneManager.sceneLoaded -= SetPlayerLocation;
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

    public void SetLobbyLocation(Vector3 position, Quaternion rotation)
    {
        m_LobbyPosition = position;
        m_LobbyRotation = rotation;
    }

    private void SetPlayerLocation(Scene scene, LoadSceneMode mode)
    {
        if (NewNetworkManager.NewSingleton.IsGameplayScene(scene.name)) return;
        Debugger($"Loaded Lobby Scene: Starting Corutine");
        StartCoroutine(SetLobbyLocation());
    }

    private IEnumerator SetLobbyLocation()
    {
        Debugger($"Waiting for Client to be Ready");
        yield return new WaitUntil(() => NetworkServer.active && NetworkClient.ready);
        Debugger($"Client is Ready: Updating Position");
        NewNetworkManager.NewSingleton.UpdateLocationList();
        Transform location = NewNetworkManager.NewSingleton.SpawnPoints[PlayerID].transform;
        transform.position = location.position;
        transform.rotation = location.rotation;
    }

    public void SetPlayerLocation()
    {
        Transform location = NewNetworkManager.NewSingleton.SpawnPoints[PlayerID].transform;
        
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

    public void Debugger(object log)
    {
        if (m_Debugger) Debug.Log($"[{GetType().ToString()}]: {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_Debugger = active;
    }
}

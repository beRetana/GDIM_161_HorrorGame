
using Mirror;
using OtherUtils;
using Player;
using StarterAssets;
using Steamworks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerObjectController : NetworkBehaviour, IDebugger
{
    public static PlayerObjectController LocalInstance { get; private set; }

    // Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerID;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;

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

        return;
        SceneManager.sceneLoaded += SetPlayerLocation;
        NewNetworkManager.NewSingleton.OnPlayersLoadedScene += SetStartLocation;
    }

    private void OnDisable()
    {
        NewNetworkManager.NewSingleton.OnPlayersLoadedScene -= SetStartLocation;
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

    private void SetPlayerLocation(Scene scene, LoadSceneMode mode)
    {
        if (!isServer) return;

        Debugger($"Loaded Scene: Starting Coroutine");

        Func<bool> condition = () => NewNetworkManager.NewSingleton.PlayersReady && NetworkClient.ready;
        Action action = () => {
            NewNetworkManager.NewSingleton.RefreshStartingLocations();
            SetStartLocation();};

        StartCoroutine(WaitForCondition(condition, action));
    }

    public void SetStartLocation()
    {
        Func<bool> condition = () => NewNetworkManager.NewSingleton.SpawnPoints[PlayerID] != null;
        Action action = () =>
        {
            Transform location = NewNetworkManager.NewSingleton.SpawnPoints[PlayerID].transform;
            RpcSetPlayerLocation(location.position, location.rotation);
        };

        StartCoroutine(WaitForCondition(condition, action));
    }

    [ClientRpc]
    private void RpcSetPlayerLocation(Vector3 position, Quaternion rotation)
    {
        if (!isLocalPlayer) return;
        Debugger($"Setting new Location ({position}):({rotation})");
        GetComponent<NetworkTransformUnreliable>().CmdTeleport(position, rotation);
    }

    private IEnumerator WaitForCondition(Func<bool> condition, Action function)
    {
        while (!condition())
        {
            Debugger("Contidion is false");
            yield return null;
        }
        Debugger("Contidion is True");
        function();
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
        manager.LoadMazeScene();
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

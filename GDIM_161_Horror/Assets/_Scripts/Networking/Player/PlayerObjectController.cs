
using Mirror;
using OtherUtils;
using Player;
using StarterAssets;
using Steamworks;
using System;
using System.Collections;
using UnityEngine;

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
    }

    private void OnDisable()
    {
        
    }

    [ClientRpc]
    public void PlayerSetPosition(Vector3 Position, Quaternion Rotation)
    {
        if (!isLocalPlayer) return;

        transform.position = Position;
        transform.rotation = Rotation;
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

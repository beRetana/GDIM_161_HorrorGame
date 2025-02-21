using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerObjectController : NetworkBehaviour
{
    public static PlayerObjectController LocalInstance { get; private set; }

    // Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;

    private NewNetworkManager manager;
    public GameObject PlayerModel;

    private bool positionInvoked = false;

    private NewNetworkManager Manager
    {
        get
        {
            if (manager != null) return manager;
            return manager = NewNetworkManager.singleton as NewNetworkManager;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlayerModel.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            if (!PlayerModel.activeSelf)
            {
                Invoke(nameof(ActivatePlayer), 0.5f);
            }
        }
    }

    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            Ready = newValue;
        }

        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }

        if (SceneManager.GetActiveScene().name == "Game" && !positionInvoked)
        {
            positionInvoked = true;
            Invoke(nameof(ActivatePlayer), 0.5f);
        }
    }

    public void ActivatePlayer()
    {
        if (PlayerModel.activeSelf) return;

        SetPosition();
        PlayerModel.SetActive(true);
    }

    private void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-5, 5), 0.8f, Random.Range(7, 15));
    }

    [Command]
    private void CmdSetPlayerReady()
    {
        PlayerReadyUpdate(Ready, !Ready);
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
        CmdSetPlayerName(SteamFriends.GetPersonaName());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        PlayerNameUpdate(PlayerName, playerName);
    }

    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
        {
            PlayerName = newValue;
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

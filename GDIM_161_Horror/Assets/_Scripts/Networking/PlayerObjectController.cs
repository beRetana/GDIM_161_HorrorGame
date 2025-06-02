#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using UnityEngine;
using Mirror;
using Steamworks;
using Player;
using StarterAssets;
using UnityEngine.SceneManagement;

public class PlayerObjectController
#if MIRROR
    : NetworkBehaviour
#else
    : MonoBehaviour
#endif
{
    public static PlayerObjectController LocalInstance { get; private set; }

#if MIRROR
    // Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;
#else
    public int ConnectionID;
    public int PlayerIdNumber;
    public ulong PlayerSteamID;
    public string PlayerName;
    public bool Ready;
#endif

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
        Object.DontDestroyOnLoad(this is MonoBehaviour mb ? mb.gameObject : null);

#if UNITY_EDITOR
        // If not networked, always enable input/controller for local testing
        if (!NetworkClient.active && !NetworkServer.active)
        {
            EnableLocalPlayerComponents();
        }
#endif
    }

    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
#if MIRROR
        if (isServer)
        {
            this.Ready = newValue;
        }
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
#else
        this.Ready = newValue;
        LobbyController.Instance.UpdatePlayerList();
#endif
    }

#if MIRROR
    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }
#endif

    public void ChangeReady()
    {
#if MIRROR
        if (isOwned)
        {
            CmdSetPlayerReady();
        }
#else
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
#endif
    }

#if MIRROR
    public override void OnStartAuthority()
    {
        EnableLocalPlayerComponents();
        LocalInstance = this;
        CmdSetPlayerName(SteamFriends.GetPersonaName());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();

        if (LobbyController.Instance != null)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public override void OnStartClient()
    {
        // Only disable components for remote players
        if (!isLocalPlayer)
        {
            DisableLocalPlayerComponents();
        }

        Manager.GamePlayers.Add(this);
        PlayerManager.Instance.AttemptAddPlayer(GetComponent<FirstPersonController>());
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }
#else
    public void OnStartAuthority()
    {
        EnableLocalPlayerComponents();
        LocalInstance = this;
        this.PlayerName = SteamFriends.GetPersonaName();
        if (this is MonoBehaviour mb)
            mb.gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public void OnStartClient()
    {

#if MIRROR
        if (!hasAuthority)
        {
            DisableLocalPlayerComponents();
        }
#endif

        Manager.GamePlayers.Add(this);
        PlayerManager.Instance.AttemptAddPlayer((this as MonoBehaviour)?.GetComponent<FirstPersonController>());
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }

    public void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }
#endif

#if MIRROR
    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        this.PlayerNameUpdate(this.PlayerName, playerName);
    }
#endif

    public void PlayerNameUpdate(string oldValue, string newValue)
    {
#if MIRROR
        if (isServer)
        {
            this.PlayerName = newValue;
        }
        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
#else
        this.PlayerName = newValue;
        LobbyController.Instance.UpdatePlayerList();
#endif
    }

    public void CanStartGame(string SceneName)
    {
#if MIRROR
        if (isOwned)
        {
            CmdCanStartGame(SceneName);
        }
#else
        manager.StartGame(SceneName);
#endif
    }

#if MIRROR
    [Command]
    public void CmdCanStartGame(string SceneName)
    {
        manager.StartGame(SceneName);
    }
#endif // MIRROR

    private void EnableLocalPlayerComponents()
    {
#if ENABLE_INPUT_SYSTEM
        var input = GetComponent<PlayerInput>();
        if (input) input.enabled = true;
#endif
        var controller = GetComponent<FirstPersonController>();
        if (controller) controller.enabled = true;
    }

    private void DisableLocalPlayerComponents()
    {
        var input = GetComponent<PlayerInput>();
        if (input) input.enabled = false;
        var controller = GetComponent<FirstPersonController>();
        if (controller) controller.enabled = false;
    }
}

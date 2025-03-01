using Mirror;
using Steamworks;

public class PlayerNetworkController : NetworkBehaviour
{
    public static PlayerNetworkController LocalInstance { get; private set; }

    // Player Data
    [SyncVar] private int _connectionID;
    [SyncVar] private int _playerID;
    [SyncVar] private ulong _playerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] private string _playerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] private bool _ready;

    public int ConnectionID { get => _connectionID; set { _connectionID = value; } }
    public int PlayerIdNumber { get => _playerID; set { _playerID = value; } }
    public ulong PlayerSteamID { get => _playerSteamID; set { _playerSteamID = value; } }
    public string PlayerName { get => _playerName; set { _playerName = value; } }
    public bool Ready { get => _ready; private set { } }

    private NewNetworkManager manager;
    private const string _LOCAL_NAME_DEFAULT = "LocalGamePlayer";

    private NewNetworkManager Manager
    {
        get
        {
            if (manager != null) return manager;
            return manager = NewNetworkManager.singleton as NewNetworkManager;
        }
    }

    private void Start(){ DontDestroyOnLoad(this.gameObject); }

    // The signature must be in this format to work with the hook
    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer) this._ready = newValue;
        if (isClient) LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerReady() { this.PlayerReadyUpdate(this._ready, !this._ready); }

    public void ChangeReady() { if (isOwned) CmdSetPlayerReady(); }

    public override void OnStartAuthority()
    {
        LocalInstance = this;
        CmdSetPlayerName(SteamFriends.GetPersonaName());
        gameObject.name = _LOCAL_NAME_DEFAULT;
        LobbyController.Instance.FindLocalPlayer();
        if (LobbyController.Instance != null) LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStartClient()
    {
        Manager.PlayersInGame.Add(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    public override void OnStopClient()
    {
        Manager.PlayersInGame.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }

    [Command]
    private void CmdSetPlayerName(string playerName) { this.PlayerNameUpdate(_playerName, playerName); }

    // This must have the same signature as the hook
    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer) this._playerName = newValue;
        if (isClient) LobbyController.Instance.UpdatePlayerList();
    }

    public void CanStartGame(string SceneName) { if (isOwned) CmdCanStartGame(SceneName); }

    [Command]
    public void CmdCanStartGame(string SceneName) { manager.StartGame(SceneName); }
}

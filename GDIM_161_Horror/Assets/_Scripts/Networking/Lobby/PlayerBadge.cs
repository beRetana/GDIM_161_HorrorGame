using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;
using Mirror;
using Networking;

public class PlayerBadge: PlayerIconGetter
{
    [SerializeField] private const string _READY_DISPLAY_TEXT = "SIGNED";
    [SerializeField] private const string _NOT_READY_DISPLAY_TEXT = "YET TO SIGN";
    [SerializeField] private Color _READY_COLOR = Color.green;
    [SerializeField] private Color _NOT_READY_COLOR = Color.red;

    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _playerReadyText;

    private string _playerName;
    private int _connectionID = -1;
    [SyncVar] private bool _isActive;
    [SyncVar] private bool _isReady;

    public string PlayerName { get => _playerName; set { _playerName = value; } }
    public int ConnectionID { get => _connectionID; private set { } }
    public bool IsReady { get => _isReady;  set { _isReady = value; } }

    public void ChangeReadyStatus()
    {
        if (_isReady)
        {
            _playerReadyText.text = _READY_DISPLAY_TEXT;
            _playerReadyText.color = _READY_COLOR;
            return;
        }
        _playerReadyText.text = _NOT_READY_DISPLAY_TEXT;
        _playerReadyText.color = _NOT_READY_COLOR;
    }

    protected override void Start() 
    {
        base.Start();
        gameObject.SetActive(false);
    }

    public void SetStatus(bool active) 
    {
        _isActive = active;
        gameObject.SetActive(active);

        Debug.Log("SETTING ACTIVE STATUS LOCAL");
        CmdSetStatus(active);
    }

    public void UpdatePlayerValues()
    {
        Debug.Log("Updating Player Values IN SCRIPT");
        _playerNameText.text = _playerName;
        ChangeReadyStatus();
        if (!_avatarReceived) GetPlayerIcon();
        gameObject.SetActive(true);
        SetStatus(true);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetStatus(bool active)
    {
        Debug.Log("Setting status Command");
        this._isActive = active;
        RpcSetStatus(this._isActive);
    }

    [ClientRpc]
    private void RpcSetStatus(bool active)
    {
        Debug.Log("Setting status Propagation");
        this.gameObject.SetActive(active);
    }

    public void SetPlayerNetworkController(PlayerNetworkController player)
    {
        _playerName = player.PlayerName;
        _connectionID = player.ConnectionID;
        _playerSteamID = player.PlayerSteamID;
        _isReady = player.Ready;
    }

    public void ClearBadge()
    {
        _playerName = string.Empty;
        _connectionID = -1;
        _isReady = false;
        _playerSteamID = 0;
        _avatarReceived = false;
        _playerNameText.text = string.Empty;
        _playerReadyText.text = string.Empty;
        gameObject.SetActive(false);
    }
}

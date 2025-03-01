using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;
using Mirror;


public class PlayerInfoLobby: NetworkBehaviour
{
    [SerializeField] private const string _READY_DISPLAY_TEXT = "SIGNED";
    [SerializeField] private const string _NOT_READY_DISPLAY_TEXT = "YET TO SIGN";
    [SerializeField] private Color _READY_COLOR = Color.green;
    [SerializeField] private Color _NOT_READY_COLOR = Color.red;

    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private RawImage _playerIcon;
    [SerializeField] private TextMeshProUGUI _playerReadyText;

    private string _playerName;
    private int _connectionID;
    private ulong _playerSteamID;
    private bool _avatarReceived;
    [SyncVar] private bool _isReady;

    public string PlayerName { get => _playerName; set { _playerName = value; } }
    public int ConnectionID { get => _connectionID; private set { } }
    public ulong PlayerSteamID { get => _playerSteamID; private set { } }

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

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

    private void Start() 
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    public void GetPlayerNetworkController(PlayerNetworkController player)
    {
        _playerName = player.PlayerName;
        _connectionID = player.ConnectionID;
        _playerSteamID = player.PlayerSteamID;
        _isReady = player.Ready;
    }

    public void SetPlayerValues()
    {
        _playerNameText.text = _playerName;
        ChangeReadyStatus();
        if(!_avatarReceived) GetPlayerIcon();
    }

    void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)_playerSteamID);
        if (ImageID == -1) return;
        _playerIcon.texture = GetSteamImageAsTexture(ImageID);
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        if (!SteamUtils.GetImageSize(iImage, out uint width, out uint height)) return null;

        byte[] image = new byte[width * height * 4];

        if (!SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4))) return null;

        Texture2D texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
        texture.LoadRawTextureData(image);
        texture.Apply();

        _avatarReceived = true;
        return texture;
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID != PlayerSteamID) return;
        _playerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
    }
}

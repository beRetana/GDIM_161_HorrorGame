using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class PlayerIconGetter : NetworkBehaviour
    {
        [SerializeField] private RawImage _playerIcon;

        protected bool _avatarReceived;
        protected ulong _playerSteamID;

        public ulong PlayerSteamID { get => _playerSteamID; }

        protected Callback<AvatarImageLoaded_t> ImageLoaded;

        protected virtual void Start()
        {
            ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
        }

        protected virtual void GetPlayerIcon()
        {
            int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)_playerSteamID);
            if (ImageID == -1) return;
            _playerIcon.texture = GetSteamImageAsTexture(ImageID);
        }

        protected Texture2D GetSteamImageAsTexture(int iImage)
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

        protected void OnImageLoaded(AvatarImageLoaded_t callback)
        {
            if (callback.m_steamID.m_SteamID != PlayerSteamID) return;
            _playerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
    }
}

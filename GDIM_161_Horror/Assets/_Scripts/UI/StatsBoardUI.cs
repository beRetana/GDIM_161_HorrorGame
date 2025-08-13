using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsBoardUI : MonoBehaviour
{
    [SerializeField]
    private string[] m_FloorNames = new string[] {"Overall", "Forest",
                                                  "Maze 1", "Maze 2", 
                                                  "Maze 3", "Lab"};

    [SerializeField] private Image m_ProfileImage;
    [SerializeField] private TextMeshProUGUI m_UserNameText;
    [SerializeField] private TextMeshProUGUI[] m_TimeStampsText;
    [SerializeField] private TextMeshProUGUI m_KnockedDownCountText;
    [SerializeField] private TextMeshProUGUI m_RezzedUpCountText;

    public void SetPlayerStats(PlayerDataTracker playerData)
    {
        m_UserNameText.text = $"{playerData.PlayerName}";
        m_KnockedDownCountText.text = $"{playerData.KnockedDownCount}";
        m_RezzedUpCountText.text = $"{playerData.RezzedUpCount}";
        SetPlayerIcon(playerData.PlayerSteamID);
        SetTimeStamps(playerData.TimesPerFloor, playerData.TotalTime);
    }

    private void SetTimeStamps(ulong[] timeStamps, ulong totalTime)
    {
        m_TimeStampsText[0].text = $"{m_FloorNames[0]} {FormatTime(totalTime)}";

        for (int i = 1; i < m_TimeStampsText.Length; ++i)
        {
            m_TimeStampsText[i].text = $"{m_FloorNames[i]} " +
                $"{FormatTime((i == 0) ? totalTime : timeStamps[i - 1])}";
        }
    }

    private static string FormatTime(ulong time)
    {
        return $"{(ulong)(time / 3600)}:{DoubleDigit((ulong)(time/60) % 60)}:{DoubleDigit((ulong)(time % 60))}";
    }

    private static string DoubleDigit(ulong time)
    {
        return (time < 10) ? $"0{time}" : $"{time}";
    }

    private void SetPlayerIcon(ulong playerSteamID)
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)playerSteamID);
        if (ImageID == -1) return;
        Texture2D texture = FlipTexture2D(GetSteamImageAsTexture(ImageID));
        Sprite image = Sprite.Create(texture,
                                     new Rect(0, 0, texture.width, texture.height),
                                     new Vector2(0.5f, 0.5f));
        m_ProfileImage.sprite = image;
    }

    private static Texture2D FlipTexture2D(Texture2D old)
    {
        int width = old.width;
        int height = old.height;

        Texture2D newTexture = new(width, height, old.format, false);

        for (int y = 0; y < height; ++y)
        {
            newTexture.SetPixels(0, y, width, 1,
                                 old.GetPixels(0, height - y - 1, width, 1));
        }
        newTexture.Apply();
        return newTexture;
    }

    private static Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        return texture;
    }
}

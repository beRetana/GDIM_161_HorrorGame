using Mirror;
using Networking;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayerBadge : PlayerIconGetter
{
    public void SetPlayerController(PlayerNetworkController player)
    {
        _playerSteamID = player.PlayerSteamID;
        if (!_avatarReceived) GetPlayerIcon();
    }
}

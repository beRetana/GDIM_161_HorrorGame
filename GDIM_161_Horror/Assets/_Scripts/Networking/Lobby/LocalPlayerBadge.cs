using Mirror;
using Networking;

public class LocalPlayerBadge : PlayerIconGetter
{
    public void SetPlayerController(PlayerNetworkController player)
    {
        _playerSteamID = player.PlayerSteamID;
        if (!_avatarReceived) GetPlayerIcon();
    }
}

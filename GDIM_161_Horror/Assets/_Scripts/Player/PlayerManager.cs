using MessengerSystem;
using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    private Dictionary<int, string> playerMessengerKeys = new Dictionary<int, string>();
    private int totalPlayers = 0;
    private void Awake()
    {
        PlayerBase.OnPlayerSpawn += AddPlayer;
        DataMessenger.SetGameObject(MessengerKeys.GameObjectKey.PlayerManager, gameObject);
    }
    public void AddPlayer(PlayerBase player)
    {
        ++totalPlayers;
        MessengerKeys.GameObjectKey? keyToAdd = null;

        switch(totalPlayers)
        {
            case 1:
                keyToAdd = MessengerKeys.GameObjectKey.Player1;
                break;
            case 2:
                keyToAdd = MessengerKeys.GameObjectKey.Player2;
                break;
            case 3:
                keyToAdd = MessengerKeys.GameObjectKey.Player3;
                break;
            case 4:
                keyToAdd = MessengerKeys.GameObjectKey.Player4;
                Debug.Log("Full Lobby");
                break;
             
            default:
                Debug.LogError($"Invalid Player Amount: {player.ID()}");
                break;
        }
        Debug.Log($"Player {player.ID()} added to PlayerManager");
        playerMessengerKeys[player.ID()] = DataMessenger.SetGameObject((MessengerKeys.GameObjectKey)keyToAdd, player.gameObject);
    }

    public void LockPlayerInput(int playerId)
    {
        GetPlayer(playerId).LockPlayer();
    }

    public void UnlockPlayerInput(int playerId)
    {
        GetPlayer(playerId).UnlockPlayer();
    }

    public PlayerBase GetPlayer(int ID)
    {
        return DataMessenger.GetGameObject(playerMessengerKeys[ID])?.GetComponent<PlayerBase>();
    }
}

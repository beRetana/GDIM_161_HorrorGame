using MessengerSystem;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Steamworks;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance {  get; private set; }
    private PlayerHolder _playerHolder;

    private NewNetworkManager _networkmanager;

    public NewNetworkManager NetworkManager
    {
        get
        {
            if (_networkmanager != null)
            {
                return _networkmanager;
            }
            return _networkmanager = NewNetworkManager.singleton as NewNetworkManager;
        }
    }

    private void Awake()
    {
        DeclareSingletonInsatnce();
        _playerHolder = new();
    }

    private void DeclareSingletonInsatnce()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int AttemptAddPlayer(PlayerBase player) // returns -1 if error
    {
        int newID = _playerHolder.AddPlayer(player);
        Debug.Log($"Player {player.name} added to {_playerHolder}");

        if (newID != -1)    Debug.Log($"Player {newID} added to {_playerHolder}. Finishing PlayerBase set up.");
        else                Debug.Log($"ERROR: Player not added to {_playerHolder}");

        return newID; // returns -1 if error
    }

    public void LockPlayerInput(int playerID)
    {
        _playerHolder[playerID].LockPlayer();
    }
    public void UnlockPlayerInput(int playerID)
    {
        _playerHolder[playerID].UnlockPlayer();
    }
    public PlayerBase GetPlayer(int playerID) // ID: 0, 1, 2, 3
    {
        if (NetworkServer.active) return GetPlayerFromNetworkManager(playerID);
        return _playerHolder[playerID];
    }

    private PlayerBase GetPlayerFromNetworkManager(int playerID)
    {
        foreach (PlayerObjectController player in NetworkManager.GamePlayers)
        {
            if (player.PlayerIdNumber != playerID) continue;

            return player.GetComponent<PlayerBase>();
        }
        throw new System.Exception($"NETWORK ERROR: Player {playerID} does not exist");
    }
}

public class PlayerHolder
{
    static readonly int _MAX_PLAYER_COUNT = 4;
    private int totalPlayers = 0;
    private PlayerBase[] playerList = new PlayerBase[4];

    public int AddPlayer(PlayerBase player)
    {
        if (totalPlayers >= _MAX_PLAYER_COUNT)
        {
            Debug.LogError("ERROR: Max player count reached");
            return -1;
        }
        if (PlayerExistsInList(player)) // player < totalPlayers
        {
            Debug.LogError("ERROR: Player already exists");
            return -1;
        }

        playerList[totalPlayers] = player;
        Debug.Log($"List size: {totalPlayers}");
        return totalPlayers++;
    }

    public PlayerBase this[int index] // index: get and set
    {
        get{
            if (!IndexPlayerExists(index)) throw new System.Exception($"ERROR: Player {index} does not exist");
            if (IndexInRange(index)) return playerList[index];
            else throw new System.Exception("ERROR: Invalid player slot index");
        }
        private set{
            if (IndexInRange(index)) playerList[index] = value;
            else throw new System.Exception("ERROR: Invalid item slot index");
        }
    } 
    private bool IndexInRange(int index) { return (index >= 0 && index < _MAX_PLAYER_COUNT); }
    private bool IndexPlayerExists(int index) 
    { 
        Debug.Log($"Checking if player {index} exists in {playerList} total players of {totalPlayers}");
        return (index >= 0 && index < totalPlayers); 
    }

    private bool PlayerExistsInList(PlayerBase player)
    {
        foreach (PlayerBase existingPlayer in playerList)
        {
            if (existingPlayer == player)
                return true;
        }
        return false;
    }
}
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance {  get; private set; }
    private PlayerHolder _playerHolder;

    [SerializeField] private bool isSingleDev;

    private void Awake()
    {
        Debug.Log($"Caller {gameObject.name}");
        if (!isSingleDev && !NetworkServer.active) return;
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
        Debug.Log(_playerHolder);
        int newID = _playerHolder.AddPlayer(player);

        if (newID != -1)    Debug.Log($"Player{newID} added to {_playerHolder}. Finishing PlayerBase set up.");
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
        return _playerHolder[playerID];
    }
}

public class PlayerHolder
{
    readonly int _MAX_PLAYER_COUNT = 4;
    [SyncVar] private int totalPlayers = 0;
    [SyncVar] private PlayerBase[] playerList = new PlayerBase[4];

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
        return totalPlayers++;
    }
    public PlayerBase this[int index] // index: get and set
    {
        get{
            if (!IndexPlayerExists(index)) throw new System.Exception($"ERROR: Player{index} does not exist");
            if (IndexInRange(index)) return playerList[index];
            else throw new System.Exception("ERROR: Invalid player slot index");
        }
        private set{
            if (IndexInRange(index)) playerList[index] = value;
            else throw new System.Exception("ERROR: Invalid item slot index");
        }
    } 
    private bool IndexInRange(int index) { return (index >= 0 && index < _MAX_PLAYER_COUNT); }
    private bool IndexPlayerExists(int index) { return (index >= 0 && index < totalPlayers); }

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
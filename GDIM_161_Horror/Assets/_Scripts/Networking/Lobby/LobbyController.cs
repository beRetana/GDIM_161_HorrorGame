using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    //UI Elements
    [SerializeField] private Button _startGameButton;
    [SerializeField] private RawImage _localPlayerAvatar;

    //Player Data
    [SerializeField] private List<PlayerBadge> _playerBadges = new List<PlayerBadge>();

    private PlayerNetworkController _localPlayerController;

    public PlayerNetworkController LocalPlayerController { get { return _localPlayerController; } private set { } }

    private bool _hostBadgeCreated;
    private int _playerCount;

    //Manager
    private NewNetworkManager manager;

    private NewNetworkManager Manager
    {
        get
        {
            if(manager != null) return manager;
            return manager = NewNetworkManager.singleton as NewNetworkManager;
        }
    }

    private void Awake() { if (Instance == null) Instance = this; }

    public void StartGame(string SceneName) { _localPlayerController.CanStartGame(SceneName); }

    public void ReadyPlayer() { _localPlayerController.ChangeReady(); }

    public void FindLocalPlayer() 
    { 
        _localPlayerController = GameObject.Find("LocalGamePlayer")?.GetComponent<PlayerNetworkController>(); 
        _localPlayerController.gameObject.SetActive(false);
    }


    public void UpdateButton()
    {
        // Change for local display
    }

    public void CheckIfAllReady()
    {
        bool AllReady = false;
        foreach(PlayerNetworkController player in Manager.PlayersInGame)
        {
            if(player.Ready)
            {
                AllReady = true;
            } 
            else
            {
                AllReady = false;
                break;
            }
        }

        if(AllReady)
        {
            if(_localPlayerController.PlayerIdNumber == 1)
            {
                _startGameButton.interactable = true;
            }
            else
            {
                _startGameButton.interactable = false;
            }
        }
        else
        {
            _startGameButton.interactable = false;
        }
    }

    public void UpdatePlayerList()
    {
        if (!_hostBadgeCreated) CreateHostPlayerItem(); //Host
        if(_playerBadges.Count < Manager.PlayersInGame.Count) CreateClientPlayerItem();
        if(_playerBadges.Count > Manager.PlayersInGame.Count) RemovePlayerItem();
        if(_playerBadges.Count == Manager.PlayersInGame.Count) UpdatePlayerItem();
    }

    public void CreateHostPlayerItem()
    {
        Debug.Log("Creating Host Player Item: " + Manager.PlayersInGame.Count);
        foreach (PlayerNetworkController player in Manager.PlayersInGame)
        {
            CreateBadge(player);
        }
        _hostBadgeCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach(PlayerNetworkController player in Manager.PlayersInGame)
        {
            if(!_playerBadges.Any(b => b.ConnectionID == player.ConnectionID))
            {
                CreateBadge(player);
            }
        }
    }

    private void CreateBadge(PlayerNetworkController player)
    {
        _playerBadges[_playerCount].SetStatus(true);
        _playerBadges[_playerCount].SetPlayerNetworkController(player);
        _playerCount++;
    }

    public void UpdatePlayerItem()
    {
        /*
        foreach(PlayerNetworkController player in Manager.GamePlayers)
        {
            foreach(PlayerListItem PlayerListItemScript in _playerListItems)
            {
                if(PlayerListItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerListItemScript.PlayerName = player._playerName;
                    PlayerListItemScript._isReady = player._ready;
                    PlayerListItemScript.SetPlayerValues();
                    if(player == _localPlayerController)
                    {
                        UpdateButton();
                    }
                }
            }
        }
        CheckIfAllReady();
        */
    }

    public void RemovePlayerItem()
    {
        /*
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem playerlistItem in _playerListItems)
        {
            if(!Manager.GamePlayers.Any(b=> b.ConnectionID == playerlistItem._connectionID))
            {
                playerListItemToRemove.Add(playerlistItem);
            }
        }

        if(playerListItemToRemove.Count > 0)
        {
            foreach(PlayerListItem playerlistItemToRemove in playerListItemToRemove)
            {
                if (playerlistItemToRemove == null) { continue; }
                GameObject ObjectToRemove = playerlistItemToRemove?.gameObject;
                _playerListItems?.Remove(playerlistItemToRemove);
                Destroy(ObjectToRemove);
                ObjectToRemove = null;
            }
        }
        */
    }
}

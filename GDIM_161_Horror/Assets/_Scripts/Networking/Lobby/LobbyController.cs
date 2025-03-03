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
    [SerializeField] private LocalPlayerBadge _localPlayerBadge;

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
        //_localPlayerController.gameObject.SetActive(false);
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
        Debug.Log("Updating Player List Count: " + Manager.PlayersInGame.Count);
        Debug.Log("Updating Player badge Count: " + _playerCount);
        if (!_hostBadgeCreated) CreateHostPlayerItem(); //Host
        if(_playerCount < Manager.PlayersInGame.Count) CreateClientPlayerItem();
        if(_playerCount > Manager.PlayersInGame.Count) RemovePlayerItem();
        if(_playerCount == Manager.PlayersInGame.Count) UpdatePlayerItem();
    }

    public void CreateHostPlayerItem()
    {
        foreach (PlayerNetworkController player in Manager.PlayersInGame)
        {
            if (_localPlayerController != null)
            {
                CreateBadge(player);
                _hostBadgeCreated = true;
            }
        }
    }

    public void CreateClientPlayerItem()
    {
        foreach(PlayerNetworkController player in Manager.PlayersInGame)
        {
            if(!_playerBadges.Any(b => b.ConnectionID == player.ConnectionID))  CreateBadge(player);
        }
    }

    private void CreateBadge(PlayerNetworkController player)
    {
        if (player.ConnectionID == _localPlayerController.ConnectionID) _localPlayerBadge.SetPlayerController(player);
        _playerBadges[_playerCount].SetStatus(true);
        _playerBadges[_playerCount].SetPlayerNetworkController(player);
        _playerBadges[_playerCount].UpdatePlayerValues();
        _playerCount++;
        Debug.Log($"Player Count {_playerCount}");
        Debug.Log($"Created Badge for {player.PlayerName}, {player.ConnectionID}");
    }

    public void UpdatePlayerItem()
    { 
        foreach(PlayerNetworkController player in Manager.PlayersInGame)
        {
            foreach(PlayerBadge playerBadge in _playerBadges)
            {
                if(playerBadge.ConnectionID == player.ConnectionID)
                {
                    playerBadge.PlayerName = player.PlayerName;
                    playerBadge.IsReady = player.Ready;
                    playerBadge.UpdatePlayerValues();
                    if(player == _localPlayerController)
                    {
                        UpdateButton();
                    }
                }
            }
        }
        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        foreach (PlayerBadge playerBadge in _playerBadges)
        {
            if(!Manager.PlayersInGame.Any(b=> b.ConnectionID == playerBadge.ConnectionID))
            {
                playerBadge.ClearBadge();
            }
        }
    }
}

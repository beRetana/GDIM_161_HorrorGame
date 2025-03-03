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
    [SerializeField] private TextMeshProUGUI _lobbyName;
    [SerializeField] private TextMeshProUGUI _localPlayerReadyText;
    [SerializeField] private Button _startGameButton;

    //Player Data
    [SerializeField] private GameObject _playerContent; // change this for a info displayer for local player
    [SerializeField] private GameObject _playerInfoDisplay; // change this for a info displayer for other players

    private List<PlayerNetworkController> _playerListItems = new List<PlayerNetworkController>();
    private PlayerNetworkController _localPlayerController;

    public PlayerNetworkController LocalPlayerController { get { return _localPlayerController; } private set { } }

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
        if(_localPlayerController.Ready)
        {
            _localPlayerReadyText.text = "Unready";
            return;
        }
        _localPlayerReadyText.text = "Ready";
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
        /*
        if(!PlayerItemCreated) {CreateHostPlayerItem(); } //Host
        if(_playerListItems.Count < Manager.GamePlayers.Count) {CreateClientPlayerItem();}
        if(_playerListItems.Count > Manager.GamePlayers.Count) {RemovePlayerItem();}
        if(_playerListItems.Count == Manager.GamePlayers.Count) {UpdatePlayerItem();}
        */
    }

    public void CreateHostPlayerItem()
    {
        /*
        foreach(PlayerNetworkController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(_playerInfoDisplay) as GameObject;

            _playerListItems.Add(_localPlayerController);
        }
        PlayerItemCreated = true;
        */
    }

    public void CreateClientPlayerItem()
    {
        /*
        foreach(PlayerNetworkController player in Manager.GamePlayers)
        {
            if(!_playerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject NewPlayerItem = Instantiate (_playerInfoDisplay) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerInfoLobby>().GetPlayerNetworkController();

                NewPlayerItemScript._playerName = player._playerName;
                NewPlayerItemScript._connectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript._isReady = player._ready;
                NewPlayerItemScript.SetPlayerValues();
            
                NewPlayerItem.transform.SetParent(_playerContent.transform);
                NewPlayerItem.transform.localScale = Vector3.one;

                _playerListItems.Add(NewPlayerItemScript); 
            }
        }
        */
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

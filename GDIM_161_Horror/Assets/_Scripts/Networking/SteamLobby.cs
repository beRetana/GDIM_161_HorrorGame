using Mirror;
using Steamworks;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// DO NOT FUCKING TOUCH THIS SCRIPT UNLESS YOU KNOW WHAT YOU'RE DOING
public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;

    //callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    //Variables
    private ulong _currentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private NewNetworkManager _manager;

    [SerializeField] private const string MAIN_SCENE = "BUILD_MainMenu";
    [SerializeField] private bool _debugger;

    public ulong CurrentLobbyID { get => _currentLobbyID; set => _currentLobbyID = value; }

    private void Start()
    {
        if (!SteamAPI.IsSteamRunning())
        {
            Debug.LogError("Steam is not running!");
            return;
        }

        if (Instance == null) { Instance = this; }

        _manager = GetComponent<NewNetworkManager>();
        if (_manager == null)
        {
            Debug.LogError("NewNetworkManager component not found!");
            return;
        }

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        SceneManager.sceneLoaded += DestroyOnMainMenu;
    }

    private void DestroyOnMainMenu(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == MAIN_SCENE)
        {
            LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
            LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }
    }

    public void HostLobby() => SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _manager.maxConnections);

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }
        Debugger("STEAMLOBBY: LOBBY CREATED SUCCESSFULLY");
        
        _manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debugger("STEAMLOBBY: REQUEST TO JOIN LOBBY");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //Everyone
        _currentLobbyID = callback.m_ulSteamIDLobby;

        //Clients

        if (NetworkServer.active) { return; }

        _manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        _manager.StartClient();
    }

    public void LeaveServer()
    {
        Debugger("STEAMLOBBY: LEAVING LOBBY");
        
        SteamMatchmaking.LeaveLobby(new CSteamID(_currentLobbyID));
        _currentLobbyID = 0;

        LobbyCreated?.Unregister();
        JoinRequest?.Unregister();
        LobbyEntered?.Unregister();
    }

    private void Debugger(object log)
    {
        if (_debugger) Debug.Log(log);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;
using System.Linq;
using Dissonance.Integrations.MirrorIgnorance;

public class PlayerObjectController : NetworkBehaviour
{
    public static PlayerObjectController LocalInstance { get; private set; }

    // Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;

    private NewNetworkManager manager;
    
    public GameObject PlayerObject;
    public GameObject DissonanceSetup;

    private NewNetworkManager Manager
    {
        get
        {
            if (manager != null) 
            {
                return manager;
            }
            return manager = NewNetworkManager.singleton as NewNetworkManager;
        }
    }


   private void Start()
   {
    DontDestroyOnLoad(this.gameObject);
    //new testing
    SceneManager.sceneLoaded+= OnSceneLoaded;

     Invoke(nameof(AddMirrorIgnorancePlayer), 1f); // Calls the method after 1 second
    //
   }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

     void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    if (scene.name == "Game") // Ensure the scene name matches exactly
    {
        // Instantiate DissonanceSetup immediately
        if (DissonanceSetup != null)
        {
            Instantiate(DissonanceSetup, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Prefab is not assigned!");
        }

        // Delay adding MirrorIgnorancePlayer by 1 second
        StartCoroutine(AddMirrorIgnorancePlayer());
    }
}

// Coroutine to delay adding the component
private IEnumerator AddMirrorIgnorancePlayer()
{
    yield return new WaitForSeconds(1f);

    if (!PlayerObject.GetComponent<MirrorIgnorancePlayer>())
    {
        var playerScript = PlayerObject.AddComponent<MirrorIgnorancePlayer>();

        // Manually initialize it
        playerScript.OnStartLocalPlayer();
    }
}


    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            this.Ready = newValue;
        }

        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }

        
    }

   

    [Command]
    private void CmdSetPlayerReady()
    {
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }

    public void ChangeReady()
    {
        if (isOwned)
        {
            CmdSetPlayerReady();
        }
    }

    public override void OnStartAuthority()
    {
        LocalInstance = this;
        CmdSetPlayerName(SteamFriends.GetPersonaName());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
        
        if (LobbyController.Instance != null)
            {
                LobbyController.Instance.UpdatePlayerList();
            }

    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
      

    }

    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
        
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        this.PlayerNameUpdate(this.PlayerName, playerName);
    }

    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
        {
            this.PlayerName = newValue;
        }

        if (isClient)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public void CanStartGame(string SceneName)
    {
        if (isOwned)
        {
            CmdCanStartGame(SceneName);
        }
    }

    [Command]
    public void CmdCanStartGame(string SceneName)
    {
        manager.StartGame(SceneName);
    }
}

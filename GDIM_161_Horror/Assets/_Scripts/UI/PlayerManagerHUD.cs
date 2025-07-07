using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManagerHUD : NetworkBehaviour
{
    [SerializeField] private GameObject m_InteractionGameObject;
    [SerializeField] private GameObject m_InGameMenuGameObject;
    [SerializeField] private GameObject m_EndGameGameObject;

    private Transform m_PlayerManagerHUD;
    private GameplayMenuHUD m_GameplayMenuHUD;
    private EndGameHUD m_EndGameHUD;
    private SettingMenuManager m_SettingsMenuHUD;
    private PlayerInteractionsHUD m_InteractionsHUD;

    private void Start()
    {
        m_PlayerManagerHUD = m_InGameMenuGameObject.transform.parent;

        m_GameplayMenuHUD = m_InGameMenuGameObject.GetComponent<GameplayMenuHUD>();
        m_InteractionsHUD = GetComponent<PlayerInteractionsHUD>();
        m_EndGameHUD = m_EndGameGameObject.GetComponentInChildren<EndGameHUD>();
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (NewNetworkManager.NewSingleton.IsGameplayScene(scene.name))
        {
            OnGameSceneSetUp();
        }
        else if (scene.name == NewNetworkManager.NewSingleton.GetLobbyScene())
        {
            OnLobbySceneSetUp();
        }
    }

    private void OnGameSceneSetUp()
    {
        SetGameObjectsUI(true);
        SetUpUI(true);
    }

    private void OnLobbySceneSetUp()
    {
        SetGameObjectsUI(false);
        SetUpUI(false);
    }

    private void SetUpUI(bool enable)
    {
        if (enable)
        {
            m_GameplayMenuHUD.EnableMenuUI();
        }
        else
        {
            m_GameplayMenuHUD.DisableMenuUI();
        }
    }

    public void SetEndGame(bool won)
    {
        RpcSetEndGame(won);
    }

    [ClientRpc]
    private void RpcSetEndGame(bool won)
    {
        if (!isLocalPlayer) return;
        SetGameObjectsUI(false);
        m_EndGameHUD.SetEndGameUI(won);
    }

    public void ResetGameUI()
    {
        SetGameObjectsUI(true);
        m_GameplayMenuHUD.ToggleVolumeMenu();
    }

    private void SetGameObjectsUI(bool active)
    {
        m_InGameMenuGameObject?.SetActive(active);
        m_InteractionGameObject?.SetActive(active);
        m_EndGameGameObject?.SetActive(!active);
    }
}

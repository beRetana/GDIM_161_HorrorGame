using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManagerHUD : MonoBehaviour
{
    [SerializeField] private GameObject m_InteractionGameObject;
    [SerializeField] private GameObject m_InGameMenuGameObject;
    [SerializeField] private GameObject m_EndGameGameObject;

    private GameplayMenuHUD m_MenuHUD;
    private EndGameHUD m_EndGameHUD;
    private SettingMenuManager m_SettingsMenuHUD;
    private PlayerInteractionsHUD m_InteractionsHUD;

    private void Start()
    {
        SetGameUIObjects(true);
        m_SettingsMenuHUD = m_InGameMenuGameObject.GetComponentInChildren<SettingMenuManager>();
        m_InteractionsHUD = GetComponent<PlayerInteractionsHUD>();
        m_EndGameHUD = m_EndGameGameObject.GetComponentInChildren<EndGameHUD>();
        m_MenuHUD = m_InGameMenuGameObject.GetComponent<GameplayMenuHUD>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (NewNetworkManager.NewSingleton.IsGameplayScene(scene.name))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (scene.name == NewNetworkManager.NewSingleton.GetLobbyScene())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SetEndGame(bool won)
    {
        SetGameUIObjects(false);
        m_EndGameHUD.SetEndGameUI(won);
    }

    public void ResetGameUI()
    {
        SetGameUIObjects(true);
        m_MenuHUD.ToggleVolumeMenu();
    }

    private void SetGameUIObjects(bool active)
    {
        m_InGameMenuGameObject.SetActive(active);
        m_InteractionGameObject.SetActive(active);
        m_EndGameGameObject.SetActive(!active);
    }
}

using UnityEngine;
using Steamworks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private bool m_Debugger;
    [SerializeField] private Button m_HostButton;
    [SerializeField] private Button m_JoinButton;

    void Start()
    {
        if (!SteamAPI.IsSteamRunning())
        {
            Debug.LogError("Steam is not initialized.");
            return;
        }
    }

    private void OnEnable()
    {
        m_HostButton.onClick.AddListener(OnClickedHost);
        m_JoinButton.onClick.AddListener(OnClickedJoin);
    }

    private void OpenOverlay()
    {
        if (SteamAPI.IsSteamRunning())
        {
            SteamFriends.ActivateGameOverlay("Friends"); // Opens Steam Overlay
        }
        else
        {
            Debug.LogError("Steam is not initialized.");
        }
    }

    private void OnClickedHost()
    {
        Debugger("[MainMenuUI]: OnCliked Host, starting host");
        m_JoinButton.gameObject.SetActive(false);
        SteamLobby.Instance.HostLobby();
    }

    private void OnClickedJoin()
    {
        OpenOverlay();
    }

    private void OnDisable()
    {
        m_HostButton.onClick.AddListener(OnClickedHost);
        m_JoinButton.onClick.AddListener(OnClickedJoin);
    }

    private void Debugger(object log)
    {
        if (m_Debugger) Debug.Log(log);
    }
}

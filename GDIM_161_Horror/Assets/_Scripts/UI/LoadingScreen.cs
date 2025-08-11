using Mirror;
using OtherUtils;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : NetworkBehaviour, IDebugger
{
    [SerializeField] private GameObject m_LoadingScreen;
    [SerializeField] private TextMeshProUGUI m_LoadingText;
    [SerializeField] private byte m_MaxLoadingTime = 15;

    private PlayerBase m_Player;
    private bool m_IsLoading = false;
    private bool m_Debugger;

    private void Start()
    {
        m_LoadingText.text = "Loading";
        m_LoadingScreen.SetActive(false);
        m_Player = GetComponent<PlayerBase>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == NewNetworkManager.NewSingleton.GetLobbySceneName()) return;
        if (!isLocalPlayer) return;

        Debugger("Starting Loading Animation");

        m_LoadingScreen.SetActive(true);
        m_IsLoading = true;
        StartCoroutine(LoadingAnim());
    }

    [ClientRpc]
    public void StartLoadingScreenCounter()
    {
        if (!isLocalPlayer) return;

        StartCoroutine(LoadScreenTimer());
    }

    private IEnumerator LoadingAnim()
    {
        m_Player.LockPlayer();

        int i = 0;

        while (m_IsLoading)
        {
            Debugger($"Loading Animation: {i}");

            if (i == 0)
            {
                m_LoadingText.text = "Loading";
            }
            else
            {
                m_LoadingText.text += ".";
            }

            yield return new WaitForSeconds(1f);

            i = (++i) % 4;
        }

        m_Player.UnlockPlayer();
        m_LoadingText.text = "Loading";
    }

    private IEnumerator LoadScreenTimer()
    {
        yield return new WaitForSeconds(m_MaxLoadingTime);
        m_LoadingScreen.SetActive(false);
        m_IsLoading = false;
    }

    public void Debugger(object log)
    {
        if (m_Debugger) Debug.Log($"[{GetType().ToString()}]: {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_Debugger = active;
    }
}
using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingScreen : NetworkBehaviour
{
    [SerializeField] private GameObject m_LoadingScreen;
    [SerializeField] private TextMeshProUGUI m_LoadingText;
    [SerializeField] private byte m_MaxLoadingTime = 15;

    private void Start()
    {
        m_LoadingText.text = "Loading";
        SetLoadingScreenActive(false);
    }

    [ClientRpc]
    public void StartLoadingScreenCounter()
    {
        if (!isLocalPlayer) return;

        StartCoroutine(LoadingAnim());
    }

    public void SetLoadingScreenActive(bool active)
    {
        m_LoadingScreen.SetActive(active);
    }

    private IEnumerator LoadingAnim()
    {
        for (int i = 0; i < m_MaxLoadingTime; i++)
        {
            if (i % 4 == 0)
            {
                m_LoadingText.text = "Loading";
            }
            else
            {
                m_LoadingText.text += ".";
            }

            yield return new WaitForSeconds(1f);
        }

        SetLoadingScreenActive(false);
    }
}
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class ReturnToLobby : NetworkBehaviour
{
    [SerializeField] private Button m_ReturnToLobby;
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private string m_LobbyName;

    private void Start()
    {
        if (isClientOnly) m_ReturnToLobby.gameObject.SetActive(false);
        m_ReturnToLobby.onClick.AddListener(LoadLobby);
    }

    public void SetText(string message)
    {
        m_Title.text = message;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Y)) LoadLobby();
    }

    [Server]
    private void LoadLobby()
    {
        // Do all clean up here
        NewNetworkManager.singleton.ServerChangeScene(m_LobbyName);
    }
}

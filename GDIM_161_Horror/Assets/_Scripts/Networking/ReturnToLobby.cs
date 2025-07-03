using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class ReturnToLobby : NetworkBehaviour
{
    [SerializeField] private Button m_ReturnToLobby;

    private string m_LobbyName;

    private void Start()
    {
        m_LobbyName = (NewNetworkManager.singleton as NewNetworkManager).GetOnlineScene();
        if (!isServer) m_ReturnToLobby.gameObject.SetActive(false);
        m_ReturnToLobby.onClick.AddListener(LoadLobby);
    }

    [Server]
    private void LoadLobby()
    {
        // Do all clean up here
        NewNetworkManager.singleton.ServerChangeScene(m_LobbyName);
    }
}

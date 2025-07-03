using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class ReturnToLobby : NetworkBehaviour
{
    [SerializeField] private Button m_ReturnToLobby;

    private void Start()
    {
        if (!isServer) m_ReturnToLobby.gameObject.SetActive(false);
        m_ReturnToLobby.onClick.AddListener(LoadLobby);
    }

    [Server]
    private void LoadLobby()
    {
        CleanUpScene();
        NewNetworkManager.NewSingleton.LoadLobbyScene();
    }

    [ClientRpc]
    private void CleanUpScene()
    {

    }

    private void ResetPlayers()
    {

    }

    private void ResetUI()
    {

    }
}

using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using StarterAssets;

public class ReturnToLobby : NetworkBehaviour
{
    [SerializeField] private Button m_ReturnToLobby;

    private PlayerManagerHUD m_PlayerManagerHUD;
    private HandInventory m_HandInventory;
    private FirstPersonController m_FirstPersonController;

    private void Start()
    {
        if (!isServer) m_ReturnToLobby.gameObject.SetActive(false);
        m_PlayerManagerHUD = transform.root.GetComponent<PlayerManagerHUD>();
        m_HandInventory = transform.root.GetComponent<HandInventory>();
        m_FirstPersonController = transform.root.GetComponent<FirstPersonController>();
    }

    private void OnEnable()
    {
        m_ReturnToLobby.onClick.AddListener(LoadLobby);
    }

    private void OnDisable()
    {
        m_ReturnToLobby.onClick.RemoveListener(LoadLobby);
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
        ResetPlayers();
    }

    private void ResetPlayers()
    {
        m_PlayerManagerHUD.ResetGameUI();
        m_HandInventory.DropAllItems();
        m_FirstPersonController.UnlockPlayer();
    }
}

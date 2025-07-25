using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ReturnToLobby : NetworkBehaviour
{
    [SerializeField] private Button m_ReturnToLobby;

    private PlayerManagerHUD m_PlayerManagerHUD;

    private void Start()
    {
        m_PlayerManagerHUD = transform.root.GetComponent<PlayerManagerHUD>();
        if (!isServer) m_ReturnToLobby.gameObject.SetActive(false);
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
    }

    [ClientRpc]
    private void CleanUpScene()
    {
        GameplayMenuHUD.ActOnAllPlayers((PlayerManagerHUD playerHUD) =>
        {
            if (playerHUD.isLocalPlayer) playerHUD.ResetGameUI();
        });
        if (isServer && isLocalPlayer) NewNetworkManager.NewSingleton.LoadLobbyScene();
    }
}

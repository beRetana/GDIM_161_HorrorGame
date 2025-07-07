using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using StarterAssets;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private GameObject m_PlayerFollowCamera;
    [SerializeField] private GameObject m_PlayerCamera;
    [SerializeField] private GameObject m_CameraHolder;

    private PlayerInput m_PlayerInput;
    private CharacterController m_CharacterController;

    private HandInventory m_HandInventory;

    override public void OnStartAuthority()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (!isLocalPlayer) return;
        ToggleObjects(NewNetworkManager.NewSingleton.IsGameplayScene(scene.name));
    }

    private void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_PlayerInput = GetComponent<PlayerInput>();

        m_HandInventory = GetComponent<HandInventory>();

        ToggleObjects(false);
    }

    private void ToggleObjects(bool active)
    {
        m_PlayerFollowCamera.SetActive(active);
        m_PlayerCamera.SetActive(active);
        m_CameraHolder.SetActive(active);

        m_CharacterController.enabled = active;
        m_PlayerInput.enabled = active;
        
        m_HandInventory.enabled = active;
        m_HandInventory.EnablePickingUp = active;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

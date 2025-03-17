using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using StarterAssets;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private GameObject _cameraBrain;
    [SerializeField] private GameObject _playerCamera;

    private FirstPersonController _playerController;
    private StarterAssetsInputs _playerStarterInput;
    private HandInventory _handInventory;
    private BasicRigidBodyPush _basicRigidBodyPush;
    private PlayerArticulations _playerArticulations;
    private PlayerInput _playerInput;

    override public void OnStartAuthority()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        _playerController = GetComponent<FirstPersonController>();
        _playerStarterInput = GetComponent<StarterAssetsInputs>();
        _handInventory = GetComponent<HandInventory>();
        _basicRigidBodyPush = GetComponent<BasicRigidBodyPush>();
        _playerArticulations = GetComponent<PlayerArticulations>();
        _playerInput = GetComponent<PlayerInput>();

        ToggleObjects(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (isLocalPlayer) ToggleObjects(true);
    }

    void ToggleObjects(bool active)
    {
        _playerController.enabled = active;
        _playerStarterInput.enabled = active;
        _playerInput.enabled = active;
        _handInventory.enabled = active;
        _basicRigidBodyPush.enabled = active;
        _playerArticulations.enabled = active;
        _cameraBrain.SetActive(active);
        _playerCamera.SetActive(active);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

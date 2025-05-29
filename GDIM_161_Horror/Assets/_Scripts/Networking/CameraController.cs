using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using StarterAssets;
using UnityEngine.InputSystem;

#if MIRROR
public class CameraController : NetworkBehaviour
#else
public class CameraController : MonoBehaviour
#endif
{
    [SerializeField] private GameObject _cameraBrain;
    [SerializeField] private GameObject _playerCamera;

    private FirstPersonController _playerController;
    private StarterAssetsInputs _playerStarterInput;
    private BasicRigidBodyPush _basicRigidBodyPush;
    private PlayerArticulations _playerArticulations;
    private HandInventory _handInventory;
    private PlayerInput _playerInput;

#if MIRROR
    override public void OnStartAuthority()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
#else
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
#endif

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
#if MIRROR
        if (isLocalPlayer) ToggleObjects(true);
#else
        ToggleObjects(true);
#endif
    }

    private void Start()
    {
        _playerController = GetComponent<FirstPersonController>();
        _playerStarterInput = GetComponent<StarterAssetsInputs>();
        _basicRigidBodyPush = GetComponent<BasicRigidBodyPush>();
        _playerArticulations = GetComponent<PlayerArticulations>();
        _handInventory = GetComponent<HandInventory>();
        _playerInput = GetComponent<PlayerInput>();
#if MIRROR
        if (NetworkClient.active || NetworkServer.active)
            ToggleObjects(false);
#else
        ToggleObjects(true);
#endif
    }

    void ToggleObjects(bool active)
    {
        Debug.Log("Toggling CameraController objects: " + active);
        _playerController.enabled = active;
        _playerStarterInput.enabled = active;
        _playerInput.enabled = active;
        _basicRigidBodyPush.enabled = active;
        _playerArticulations.enabled = active;
        //_handInventory.enabled = active;
        _cameraBrain.SetActive(active);
        _playerCamera.SetActive(active);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

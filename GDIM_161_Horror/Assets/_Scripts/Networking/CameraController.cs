using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using StarterAssets;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private GameObject _cameraBrainTransform;
    [SerializeField] private FirstPersonController _playerController;
    [SerializeField] private StarterAssetsInputs _playerStarterInput;
    [SerializeField] private HandInventory _handInventory;
    [SerializeField] private BasicRigidBodyPush _basicRigidBodyPush;
    [SerializeField] private PlayerArticulations _playerArticulations;
    [SerializeField] private PlayerInput _playerInput;

    override public void OnStartAuthority()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ToggleObjects(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (isLocalPlayer) ToggleObjects(true);
    }

    void ToggleObjects(bool active)
    {
        _cameraBrainTransform.SetActive(active);
        gameObject.SetActive(active);
        _playerController.enabled = active;
        _playerStarterInput.enabled = active;
        _playerInput.enabled = active;
        _handInventory.enabled = active;
        _basicRigidBodyPush.enabled = active;
        gameObject.SetActive(active);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

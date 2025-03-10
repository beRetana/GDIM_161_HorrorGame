using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using StarterAssets;
using UnityEngine.InputSystem;
using Dissonance.Integrations.MirrorIgnorance;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private GameObject _cameraBrainTransform;
    [SerializeField] private FirstPersonController _playerController;
    [SerializeField] private StarterAssetsInputs _playerStarterInput;
    [SerializeField] private HandInventory _handInventory;
    [SerializeField] private BasicRigidBodyPush _basicRigidBodyPush;
    [SerializeField] private PlayerArticulations _playerArticulations;
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private MirrorIgnorancePlayer _mirrorIgnorancePlayer;

    public GameObject PlayerObject;

    override public void OnStartAuthority()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ToggleObjects(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        

            if (scene.name == "Game") // Ensure the scene name matches exactly
                    { 
                    Invoke(nameof(AddMirrorIgnorancePlayerDirectly), 0.1f);   
                    }
                    
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
       // _mirrorIgnorancePlayer.enabled = active;
    }

    

            private void AddMirrorIgnorancePlayerDirectly()
                {
                    if (PlayerObject == null)
                    {
                        Debug.LogError("PlayerObject is null. Make sure it is set before adding the script.");
                        return;
                    }

                    if (!PlayerObject.GetComponent<MirrorIgnorancePlayer>())
                    {
                        var PlayerScript = PlayerObject.AddComponent<MirrorIgnorancePlayer>();
                        Debug.Log("MirrorIgnorancePlayer added successfully.");

                        PlayerScript.OnStartLocalPlayer();
                    }
                }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

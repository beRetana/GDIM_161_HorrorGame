using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;
using Unity.Cinemachine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera _cameraBrainTransform;

    private void Start()
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
        _cameraBrainTransform.gameObject.SetActive(active);
        gameObject.SetActive(active);
    }
}

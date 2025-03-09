using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform _cameraBrainTransform;
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ToggleObjects(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        ToggleObjects(true);
    }

    void ToggleObjects(bool active)
    {
        _cameraBrainTransform.gameObject.SetActive(active);
        gameObject.SetActive(active);
    }
}

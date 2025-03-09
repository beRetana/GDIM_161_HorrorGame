using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;
using Unity.Cinemachine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private GameObject _cameraBrainTransform;

    override public void OnStartAuthority()
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
        _cameraBrainTransform.SetActive(active);
        gameObject.SetActive(active);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

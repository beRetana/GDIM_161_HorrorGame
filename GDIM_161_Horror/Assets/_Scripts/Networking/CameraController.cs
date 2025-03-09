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
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        _cameraBrainTransform.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }
}

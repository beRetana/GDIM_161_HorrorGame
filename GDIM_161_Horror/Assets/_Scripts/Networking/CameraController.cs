using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class CameraController : NetworkBehaviour
{
  
    [SerializeField] private GameObject Camera;
    void Start()
    {
        if (isLocalPlayer)
        {
            // Only enable the local player's camera
           Camera.SetActive(true);
        }
        else
        {
            // Disable camera for other players
           Camera.SetActive(false);
        }
    }
}

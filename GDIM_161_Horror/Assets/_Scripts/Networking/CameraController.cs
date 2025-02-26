using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class CameraController : NetworkBehaviour
{
    public Camera playerCamera;  // The camera attached to the player.

    void Start()
    {
        if (isLocalPlayer)
        {
            // Only enable the local player's camera
            playerCamera.enabled = true;
        }
        else
        {
            // Disable camera for other players
            playerCamera.enabled = false;
        }
    }
}

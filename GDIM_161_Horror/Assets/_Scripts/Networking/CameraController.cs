using UnityEngine;
using Mirror;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true); 
    }
}

using UnityEngine;
using Mirror;
using Steamworks;

public class NetworkComponentManager : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private AudioListener audioListener;
    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true); 
    }

    private void Start()
    {
        if (!isLocalPlayer) audioListener.enabled = false;
    }
}

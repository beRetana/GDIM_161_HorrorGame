using UnityEngine;
using Interactions;
using Mirror;
using System.Collections;
using Unity.VisualScripting;

public class SingleDoor : NetworkBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform m_StartLocation;
    [SerializeField] private Transform m_EndingLocation;
    [SerializeField] private float _openAnimDuration = 3f;

    [Header("Debugging")]
    [SerializeField] private bool _enableDebugging;

    private InteractableItem _interactableItem;
    private NetworkIdentity _networkIdentity;
    private Quaternion _endingRotation;
    [SyncVar] private bool _isOpen;

    private void Start()
    {
        _interactableItem = GetComponent<InteractableItem>();
        _networkIdentity = transform.parent.GetComponent<NetworkIdentity>();
        _interactableItem.SetInteractAction(OpenDoor);
    }

    private void OpenDoor(int playerID)
    {
        if (_isOpen) return;
        Debugger($"Player {playerID} has opened Door: {gameObject.name}");
        if (isServer) RpcSetState(_networkIdentity);
        else CmdSetState(_networkIdentity);
    }

    [ClientRpc]
    private void RpcSetState(NetworkIdentity doorID)
    {
        if (this._networkIdentity != doorID) return;
        this._isOpen = true;
        _interactableItem.SetInteractive(false);
        StartCoroutine(OpenDoorAnim(m_StartLocation.position, m_EndingLocation.position, _openAnimDuration));
        AudioManager.instance.PlayOneShot(FMODEvents.instance.SingleDoorOpen, this.transform.position);
    }

    [Command]
    private void CmdSetState(NetworkIdentity doorID)
    {
        RpcSetState(doorID);
    }

    private IEnumerator OpenDoorAnim(Vector3 startingPosition, Vector3 endingPosition, float duration)
    {
        float timeElapsed = 0f;

        for (; timeElapsed <= duration; timeElapsed += Time.deltaTime)
        {
            m_StartLocation.transform.position = Vector3.Lerp(startingPosition, endingPosition, Mathf.Clamp01(timeElapsed / duration));
            
            yield return null;
        }
        m_StartLocation.transform.position = endingPosition;
        _interactableItem.SetInteractive(false);
    }

    private void Debugger(object log)
    {
        if (_enableDebugging) Debug.Log(log);
    }
}

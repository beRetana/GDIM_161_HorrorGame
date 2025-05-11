using UnityEngine;
using Interactions;
using Mirror;
using System.Collections;
using UnityEngine.Splines.Interpolators;

public class SingleDoor : NetworkBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform pivot;
    [SerializeField] private float _openRotation = 90f;
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
        _endingRotation = Quaternion.Euler(new Vector3(0, _openRotation, 0));
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
        StartCoroutine(OpenDoorAnim(pivot.rotation, pivot.rotation * _endingRotation, _openAnimDuration));
    }

    [Command]
    private void CmdSetState(NetworkIdentity doorID)
    {
        RpcSetState(doorID);
    }

    private IEnumerator OpenDoorAnim(Quaternion rotation, Quaternion endingRotation, float duration)
    {
        Quaternion startingRotation = rotation;
        float timeElapsed = 0f;

        for (; timeElapsed <= duration; timeElapsed += Time.deltaTime)
        {
            pivot.rotation = Quaternion.Slerp(startingRotation, endingRotation, Mathf.Clamp01(timeElapsed / duration));
            
            yield return null;
        }
    }

    private void Debugger(object log)
    {
        if (_enableDebugging) Debug.Log(log);
    }
}

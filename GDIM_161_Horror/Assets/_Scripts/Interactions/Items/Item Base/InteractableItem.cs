using UnityEngine;
using System;
using TMPro;
using Mirror;

/// <summary>
/// This class allows items to be interacted with a player.
/// </summary>
public class InteractableItem : MonoBehaviour, IInteractable
{
    [SerializeField] protected Animator _uiAnimator;
    [SerializeField] protected LookAtCamera _lookAtCamera;
    [SerializeField] protected NetworkIdentity _networkIdentity;
    [SerializeField] protected string _textName;
    protected string FADE = "FADE";

    protected TextMeshProUGUI _textMesh;

    protected Action<int> OnInteractAction;

    protected bool _isInteractable;

    protected virtual void Awake()
    {
        OnInteractAction = (int playerId) => Debug.Log($"Player: {playerId} Interacted");
        _isInteractable = true;
    }

    protected virtual void Start()
    {
        _textMesh = transform.GetComponentInChildren<TextMeshProUGUI>();
        if (_networkIdentity == null) _networkIdentity = transform.parent.GetComponent<NetworkIdentity>();
        SetDisplayMessage(_textName);
    }

    public virtual void SetDisplayMessage(string message)
    {
        if (_textMesh == null) return;
        _textMesh.text = message;
    }

    public virtual void SetInteractive(bool intactive)
    {
        _isInteractable = intactive;
    }

    public virtual void Interact(int playerID)
    {
        OnInteractAction(playerID);
    }

    public virtual void SetInteractAction(Action<int> action)
    {
        OnInteractAction = action;
    }

    public virtual void Detected(int playerID)
    {
        if (!_isInteractable) return;
        _lookAtCamera.SetCamera(Camera.main);
        _uiAnimator.SetBool(FADE, true);
    }

    public virtual void StoppedDetecting(int playerID)
    {
        try { _uiAnimator?.SetBool(FADE, false); }
        finally{}
    }

    public NetworkIdentity GetNetworkID()
    {
        return _networkIdentity;
    }
}

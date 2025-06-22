using UnityEngine;
using System;
using TMPro;
using Mirror;
using OtherUtils;

/// <summary>
/// This class allows items to be interacted with a player.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(LookAtCamera))]
public class InteractableItem : MonoBehaviour, IInteractable, IDebugger
{
    [SerializeField] protected NetworkIdentity _networkIdentity;
    [SerializeField] protected string _textName;
    
    protected Animator _uiAnimator;
    protected TextMeshProUGUI _textMesh;
    protected LookAtCamera _lookAtCamera;
    protected string FADE = "FADE";
    protected bool _isInteractable;
    protected bool m_DebugEnabled;

    protected Action<int> OnInteractAction;

    protected virtual void Awake()
    {
        OnInteractAction = (int playerId) => Debug.Log($"Player: {playerId} Interacted");
        _isInteractable = true;
    }

    protected virtual void Start()
    {
        _textMesh = transform.GetComponentInChildren<TextMeshProUGUI>();
        _uiAnimator = GetComponent<Animator>();
        _lookAtCamera = GetComponent<LookAtCamera>();
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
    public virtual void StartedInteraction(int playerID)
    {
        Debugger($"Started Interacting with player {playerID}");
    }

    public virtual void CanceledInteraction(int playerID)
    {
        Debugger($"Cancelled Interaction By Player {playerID}");
    }

    public virtual void PerformedInteraction(int playerID)
    {
        if (!_isInteractable) return;
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

    public void Debugger(object log)
    {
        if (m_DebugEnabled) Debug.Log($"[{this.GetType().ToString()}] {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_DebugEnabled = active;
    }
}

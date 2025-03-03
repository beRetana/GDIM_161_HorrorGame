using UnityEngine;
using System;
using TMPro;

/// <summary>
/// This class allows items to be interacted with a player.
/// </summary>
public class InteractableItem : MonoBehaviour, IInteractable
{
    [SerializeField] protected Animator _textAnimation;
    [SerializeField] protected LookAtCamera _lookAtCamera;
    [SerializeField] protected string _textName;
    [SerializeField] protected string _fadeIn;

    protected TextMeshProUGUI _textMesh;

    protected Action<int> OnInteractAction;

    protected bool _isInteractable;

    void Awake()
    {
        OnInteractAction = (int playerId) => Debug.Log($"Player: {playerId} Interacted");
        _isInteractable = true;
    }

    public void Start()
    {
        _textMesh = transform.GetComponentInChildren<TextMeshProUGUI>();
        SetDisplayMessage(_textName);
    }

    public virtual void SetDisplayMessage(string message)
    {
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
        _lookAtCamera.SetCamera(PlayerManager.Instance.GetPlayer(playerID).GetComponentInChildren<Camera>());
        _textAnimation.SetBool(_fadeIn, true);
    }

    public virtual void StoppedDetecting(int playerID)
    {
        _textAnimation.SetBool(_fadeIn, false);
    }
}

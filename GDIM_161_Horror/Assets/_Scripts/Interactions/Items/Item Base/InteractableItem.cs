using UnityEngine;
using System;
using TMPro;

/// <summary>
/// This class allows items to be interacted with a player.
/// </summary>
public class InteractableItem : MonoBehaviour, IInteractable
{
    [SerializeField] protected Animator _textAnimation;
    [SerializeField] protected string _textName;
    [SerializeField] protected string _fadeIn;

    protected TextMeshProUGUI _textMesh;

    protected Action<int> OnInteractAction;

    void Awake()
    {
        OnInteractAction = (int playerId) => Debug.Log($"Player: {playerId} Interacted");
    }

    void Start()
    {
        _textMesh = transform.GetComponentInChildren<TextMeshProUGUI>();
        _textMesh.text = _textName;
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
        //Debug.Log($"Detected by player {playerID}, {_playerManager.GetPlayer(playerID).gameObject.ToString()}");
        _textAnimation.SetBool(_fadeIn, true);
    }

    public virtual void StoppedDetecting(int playerID)
    {
        _textAnimation.SetBool(_fadeIn, false);
    }
}

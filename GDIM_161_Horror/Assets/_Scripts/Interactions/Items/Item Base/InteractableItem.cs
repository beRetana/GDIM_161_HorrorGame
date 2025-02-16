using UnityEngine;
using System;
using MessengerSystem;
using TMPro;

/// <summary>
/// This class allows items to be picked up by the player.
/// </summary>
public class InteractableItem : MonoBehaviour, IInteractable
{
    [SerializeField] protected Animator _textAnimation;
    [SerializeField] protected string _textName;
    [SerializeField] protected string _fadeIn;

    protected TextMeshProUGUI _textMesh;
    protected PlayerManager _playerManager;

    protected Action<int> OnInteractAction;

    void Start()
    {
        _playerManager = DataMessenger.GetGameObject(MessengerKeys.GameObjectKey.PlayerManager).GetComponent<PlayerManager>();
        //Debug.Log($"{this.gameObject.name} found playerManager: {_playerManager}");
        _textMesh = transform.GetComponentInChildren<TextMeshProUGUI>();
        _textMesh.text = _textName;

        OnInteractAction = (playerID) => { Debug.Log($"Player {playerID} interacted With Object {this.name}"); };
    }

    public virtual void Interact(int playerID)
    {
        OnInteractAction(playerID);
    }

    protected virtual void SetInteractAction(Action<int> action)
    {
        OnInteractAction = action;
    }

    public virtual void Detected(int playerID)
    {
        Debug.Log($"Detected by player {playerID}, {_playerManager.GetPlayer(playerID).gameObject.ToString()}");
        _textAnimation.SetBool(_fadeIn, true);
    }

    public virtual void StoppedDetecting(int playerID)
    {
        _textAnimation.SetBool(_fadeIn, false);
    }
}

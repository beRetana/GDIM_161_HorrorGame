using UnityEngine;
using MessengerSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// This class allows items to be picked up by the player.
/// </summary>
public class PickableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator textAnimation;
    [SerializeField] private string _fadeIn;

    private PlayerManager _playerManager;

    void Start()
    {
        _playerManager = DataMessenger.GetGameObject(MessengerKeys.GameObjectKey.PlayerManager).GetComponent<PlayerManager>();
        //Debug.Log($"{this.gameObject.name} found playerManager: {_playerManager}");
    }

    public void Interact(int playerID)
    {
        _playerManager.GetPlayer(playerID).gameObject.GetComponent<HandInventory>().PickUpObject(gameObject);
    }

    public void Detected(int playerID)
    {
        Debug.Log($"Detected by player {playerID}, {_playerManager.GetPlayer(playerID).gameObject.ToString()}");
        textAnimation.SetBool(_fadeIn, true);
    }

    public void StoppedDetecting(int playerID)
    {
        textAnimation.SetBool(_fadeIn, false);
    }
}

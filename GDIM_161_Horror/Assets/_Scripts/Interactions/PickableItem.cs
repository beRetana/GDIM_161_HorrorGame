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

    private GameObject _player;

    void Start()
    {
        _player = DataMessenger.GetGameObject(MessengerKeys.GameObjectKey.Player);
    }

    public void Interact()
    {
        _player.GetComponent<HandInventory>().PickUpObject(gameObject);
    }

    public void Detected()
    {
        textAnimation.SetBool(_fadeIn, true);
    }

    public void StoppedDetecting()
    {
        textAnimation.SetBool(_fadeIn, false);
    }
}

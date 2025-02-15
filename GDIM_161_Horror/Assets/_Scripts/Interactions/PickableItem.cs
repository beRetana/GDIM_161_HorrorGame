using UnityEngine;
using MessengerSystem;
using TMPro;

/// <summary>
/// This class allows items to be picked up by the player.
/// </summary>
public class PickableItem : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator _textAnimation;
    [SerializeField] private string _textName;
    [SerializeField] private string _fadeIn;

    private TextMeshProUGUI _textMesh;
    private PlayerManager _playerManager;

    void Start()
    {
        _playerManager = DataMessenger.GetGameObject(MessengerKeys.GameObjectKey.PlayerManager).GetComponent<PlayerManager>();
        //Debug.Log($"{this.gameObject.name} found playerManager: {_playerManager}");
        _textMesh = transform.GetComponentInChildren<TextMeshProUGUI>();
        _textMesh.text = _textName;
    }

    public void Interact(int playerID)
    {
        Debug.Log($"Pickable Item {gameObject.name} recieved an Interact order from Player {playerID}");
        _playerManager.GetPlayer(playerID).gameObject.GetComponent<HandInventory>().PickUpItem(transform.parent.gameObject);
    }

    public void Detected(int playerID)
    {
        Debug.Log($"Detected by player {playerID}, {_playerManager.GetPlayer(playerID).gameObject.ToString()}");
        _textAnimation.SetBool(_fadeIn, true);
    }

    public void StoppedDetecting(int playerID)
    {
        _textAnimation.SetBool(_fadeIn, false);
    }
}

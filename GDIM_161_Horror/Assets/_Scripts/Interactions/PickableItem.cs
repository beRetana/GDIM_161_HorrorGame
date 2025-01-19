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
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _textName;
    [SerializeField] private float _timer = 2f;

    private GameObject _player;
    private float _time;
    private bool _increasing;
    private bool _decreasing;

    private float _MAX_ALPHA = 255;
    private float _MIN_TIME = 0;

    void Start()
    {
        _player = DataMessenger.GetGameObject(MessengerKeys.GameObjectKey.Player);
        _time = _timer;
    }

    void FixedUpdate()
    {
        if(_time >= 0 && _increasing)
        {
            UpdateVisuals(Mathf.Max(_time/_timer, _MIN_TIME));
            _time -= Time.deltaTime;
        }
        else if (_time <= _timer && _decreasing)
        {
            UpdateVisuals(Mathf.Min(_time/_timer, _timer/_timer));
            _time += Time.deltaTime;
        }
        else
        {
            _increasing = false;
            _decreasing = false;
        }
    }

    void UpdateVisuals(float alphaPercentage)
    {
        Color tempColor = _background.color;
        tempColor.a = alphaPercentage * _MAX_ALPHA;
        _background.color = tempColor;

        tempColor = _textName.color;
        tempColor.a = alphaPercentage * _MAX_ALPHA;
        _textName.color = tempColor;
    }

    public void Interact()
    {
        _player.GetComponent<HandInventory>().PickUpObject(gameObject);
    }

    public void Detected()
    {
        _time = _MIN_TIME;
        _decreasing = true;
    }

    public void StoppedDetecting()
    {
        _time = _timer;
        _increasing = true;
    }
}

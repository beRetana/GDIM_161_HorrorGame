using UnityEngine;
using StarterAssets;
using Mirror;
// this code is specific to the ladder
public class Ladder : MonoBehaviour
{
    bool _playerInRange = false;
    private FirstPersonController _player;
    
    void Start()
    {
        _player = GetComponent<FirstPersonController>();  
        _player = FindObjectOfType<FirstPersonController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _playerInRange)
        {
            Debug.Log("Button working");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player.IsClimbing = true;
            _player.Headbop = true;
            _playerInRange = true;
            Debug.Log("Entered Trigger: " + other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player.IsClimbing = false;
            _playerInRange = false;
            Debug.Log("Exited Trigger: " + other.gameObject.name);
        }
    }
}
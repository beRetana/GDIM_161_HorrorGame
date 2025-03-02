using UnityEngine;
using StarterAssets;
using System.Collections;
public class Ladderupletgo : MonoBehaviour
{
    [SerializeField] private Transform snapPosition;
    private FirstPersonController _player;
    private bool _playerInRange = false;
    private float moveDuration = 0.2f; // Adjust for speed

    void Start()
    {
        _player = FindObjectOfType<FirstPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Helloworld");
            _player.IsClimbing = true;
            _playerInRange = true;
            _player.Headbop = true;
            StartCoroutine(SmoothMoveToPosition());
        }
    }

    private IEnumerator SmoothMoveToPosition()
    {
        Vector3 startPosition = _player.transform.position;
        Quaternion startRotation = _player.transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            _player.Headbop = false;
            _player.IsClimbing = false;
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            _player.transform.position = Vector3.Lerp(startPosition, snapPosition.position, t);
            _player.transform.rotation = Quaternion.Slerp(startRotation, snapPosition.rotation, t);
            yield return null;
        }
    }
}

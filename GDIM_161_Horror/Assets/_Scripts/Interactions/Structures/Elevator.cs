using StarterAssets;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterController>(out CharacterController character))
        {
            other.GetComponent<FirstPersonController>().GravityOn = false;
            other.GetComponent<PlayerAnimator>().SetAnimLadder(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<CharacterController>(out CharacterController character))
        {
            other.GetComponent<CharacterController>().Move(Vector3.up * _moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<CharacterController>(out CharacterController character))
        {
            other.GetComponent<FirstPersonController>().GravityOn = true;
            other.GetComponent<PlayerAnimator>().SetAnimLadder(false);
        }
    }
}

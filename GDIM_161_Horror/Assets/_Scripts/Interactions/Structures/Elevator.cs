using StarterAssets;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private bool _debugger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterController>(out CharacterController character))
        {
            other.GetComponent<FirstPersonController>().GravityOn = false;
            other.GetComponent<PlayerAnimator>().SetAnimLadder(true);
            Debugger("Player Going Up the Ladder");
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
            Debugger("Player Off the Ladder");
        }
    }

    private void Debugger(object log)
    {
        if (_debugger) Debug.Log(log);
    }
}

using Mirror;
using StarterAssets;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private Transform m_Filler;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private bool _debugger;

    public Transform Filler => m_Filler;

    public void SetLadderActive(bool active)
    {
        gameObject.SetActive(active);
        m_Filler.gameObject.SetActive(!active);
    }

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

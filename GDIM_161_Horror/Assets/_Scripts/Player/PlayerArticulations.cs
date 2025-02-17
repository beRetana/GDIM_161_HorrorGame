using UnityEngine;

public class PlayerArticulations : MonoBehaviour
{
    [SerializeField] private Rigidbody _playerRidgidbody;
    public Rigidbody PlayerHandRigidbody { get { return _playerRidgidbody; } }
}

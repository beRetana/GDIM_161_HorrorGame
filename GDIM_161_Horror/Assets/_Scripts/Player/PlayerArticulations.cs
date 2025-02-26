using Mirror;
using UnityEngine;

public class PlayerArticulations : NetworkBehaviour
{
    [SerializeField] private Rigidbody _playerRidgidbody;
    public Rigidbody PlayerHandRigidbody { get { return _playerRidgidbody; } }
}

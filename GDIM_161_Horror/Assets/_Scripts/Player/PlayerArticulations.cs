using UnityEngine;

public class PlayerArticulations : MonoBehaviour
{
    [SerializeField] private Transform _playerArticulation;

    public Transform PlayerArticulation { get { return _playerArticulation; } }
}

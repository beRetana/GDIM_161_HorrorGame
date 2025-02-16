using System;
using UnityEngine;

namespace Interactions
{
    public class Door : MonoBehaviour
    {
        [Header("Door Settings/Components")]
        [SerializeField] private Transform _rightStartPosition;
        [SerializeField] private Transform _leftStartPosition;
        [SerializeField] private GameObject _rightDoor;
        [SerializeField] private GameObject _leftDoor;
        [SerializeField] private float _openDistance = 1.5f;

        private enum DoorState
        {
            Locked,
            Unlocking,
            Unlocked
        }

        private DoorState _doorState;

        private void Start()
        {
            _doorState = DoorState.Locked;
        }
    }
}

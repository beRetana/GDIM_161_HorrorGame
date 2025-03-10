using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class CheckpointManager : MonoBehaviour
    {
        private List<Checkpoint> _checkpoints;
        public static CheckpointManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            _checkpoints = new List<Checkpoint>();
        }

        public void AddCheckpoint(Checkpoint checkpoint)
        {
            _checkpoints.Add(checkpoint);
        }

        // Finializar el sistema de checkpoints para que se desactiven todos los checkpoints y se vuelvana a activar por ciclo.

        public void ResetCheckpoints()
        {
            foreach (Checkpoint checkpoint in _checkpoints)
            {
                checkpoint.RandomSpawn();
            }
        }
    }
}

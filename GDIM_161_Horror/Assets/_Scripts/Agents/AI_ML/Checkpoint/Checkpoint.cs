using UnityEngine;
using AI_FSM;

namespace AI
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private CheckpointType _checkpointType;

        private void Start()
        {
            gameObject.SetActive(false);
            RandomSpawn();
        }

        public enum CheckpointType
        {
            Main = 100,
            Common = 50,
            Basic = 25
        }

        public void RandomSpawn()
        {
            if (Random.value > (float)_checkpointType / 100f) return;
            gameObject.SetActive(true);
            CheckpointManager.Instance.AddCheckpoint(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<SoundSensor>(out _)) return;

            gameObject.SetActive(false);
        }
    }
}

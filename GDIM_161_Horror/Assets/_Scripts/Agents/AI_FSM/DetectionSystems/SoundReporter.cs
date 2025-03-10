using UnityEngine;

namespace AI_FSM{
    public class SoundReporter : MonoBehaviour{

        [Header("AI Sound Sensor Basic Settings")]
        [SerializeField] private bool _enableSoundDetection = true;

        [Tooltip("The Max distance at which a sound can be heard in meters.")]
        [SerializeField, Range(1f,100f)] private float _detectionRange = 100f;

        [Tooltip("The layer mask that will be used to detect sound obstructions (exclude player and enemy).")]
        [SerializeField] private LayerMask _soundObstructions;

        private float _MIN_DB = 20f;

        public void Enable()
        {
            _enableSoundDetection = true;
        }

        public void Disable()
        {
            _enableSoundDetection = false;
        }

        public void ReportSound(Vector3 position, float intensity)
        {
            if (!_enableSoundDetection) return;

            SoundSensor enemy = GetClosestEnemy();

            float distance = CalculateEnemyDistance(enemy);

            if (distance < 0.001f) return;
            float db = CalculateSoundStrength(intensity, distance);

            if (db < _MIN_DB) return;

            enemy.ReceiveReportSound(position);
        }

        private float CalculateEnemyDistance(SoundSensor enemy)
        {
            float directDistance = Vector3.Distance(transform.position, enemy.transform.position);
            if (directDistance > _detectionRange) return -1f;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, (enemy.transform.position - transform.position).normalized,
                                                   directDistance, _soundObstructions);
            float obstructionDistance = 0f;

            for (int i = 0; i < hits.Length - 1; i += 2)
            {
                obstructionDistance += hits[i+1].distance - hits[i].distance;
            }

            return directDistance + obstructionDistance;
        }

        private float CalculateSoundStrength(float intensity, float distance)
        {
            return (10f * (Mathf.Log10(intensity) - 12f)) - (6 * Mathf.Log(distance, 2f));
        }

        private SoundSensor GetClosestEnemy()
        {
            return (SoundSensor)FindFirstObjectByType(typeof(SoundSensor));
        }

        /// <summary>
        /// Default method for trigering sound reports but dev might want to modify how 
        /// and when these events should get called in which case they might want to manually call ReportSound.
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision){

            if (!_enableSoundDetection) return;
            ReportSound(collision.transform.position, collision.impulse.magnitude);
        }
    }
}

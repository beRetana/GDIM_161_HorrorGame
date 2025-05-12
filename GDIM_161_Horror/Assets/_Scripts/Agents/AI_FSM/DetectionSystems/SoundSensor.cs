using System;
using UnityEngine;

namespace AI_FSM
{
    public class SoundSensor : MonoBehaviour
    {
        [SerializeField] private bool m_debugger;
        public Action<Vector3> OnSoundReport;

        private Vector3 m_RecentPosition;

        public Vector3 RecentPosition {  get { return m_RecentPosition; } }

        public void ReceiveReportSound(Vector3 soundPosition)
        {
            OnSoundReport?.Invoke(soundPosition);
            Debugger($"Sensor {this.gameObject.name} received a sound at position: {soundPosition}");
        }

        private void Debugger(object log)
        {
            if (m_debugger) Debug.Log(log);
        }
    }
}

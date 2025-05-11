using System;
using UnityEngine;

namespace AI_FSM
{
    public class SoundSensor : MonoBehaviour
    {
        public Action<Vector3> OnSoundReport;

        private Vector3 m_RecentPosition;

        public Vector3 RecentPosition {  get { return m_RecentPosition; } }

        public void ReceiveReportSound(Vector3 soundPosition)
        {
            OnSoundReport?.Invoke(soundPosition);
        }
    }
}

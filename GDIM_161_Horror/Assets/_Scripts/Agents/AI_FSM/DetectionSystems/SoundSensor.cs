using System;
using UnityEngine;

namespace AI_FSM
{
    public class SoundSensor : MonoBehaviour
    {
        public Action<Vector3> OnSoundReport;

        public void ReceiveReportSound(Vector3 soundPosition)
        {
            OnSoundReport?.Invoke(soundPosition);
        }
    }
}

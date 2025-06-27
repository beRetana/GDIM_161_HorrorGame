using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(FirstPersonController))]
public class PlayerHeadBobbing : MonoBehaviour
{
    [SerializeField] private CinemachineBasicMultiChannelPerlin m_PerlinNoise;
    [SerializeField, Range(0f, 10f)] private float m_MaxFrequency = 5f;
    [SerializeField, Range(0f, 5f)] private float m_MinFrequency = 1.0f;
    [SerializeField, Range(0f, 10f)] private float m_MaxAmplitude = 5f;
    [SerializeField, Range(0f, 5f)] private float m_MinAmplitude = 1.0f;
    [SerializeField] private bool m_enableHeadBobbing;
    [SerializeField] private bool m_debugger;

    /// <summary>
    /// Sets the noise's amplitude and frequency based on the values given.
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="amplitude"></param>
    public void SetNoise(float frequency, float amplitude)
    {
        if (!m_enableHeadBobbing) return;

        if ((frequency > m_MaxFrequency) || (amplitude > m_MaxAmplitude))
        {
            Debugger("Frequency or Amplitude are above the MAX.");
            return;
        }

        if ((frequency < 0) || (amplitude < 0))
        {
            Debugger("Frequency or Amplitude are below 0.");
            return;
        }

        m_PerlinNoise.AmplitudeGain = Mathf.Max(amplitude, m_MinAmplitude);
        m_PerlinNoise.FrequencyGain = Mathf.Max(frequency, m_MinFrequency);
    }

    /// <summary>
    /// Sets the noise amplitude and frequency base on a ratio from 0-1.
    /// </summary>
    /// <param name="ratio"></param>
    public void SetNoise(float ratio)
    {
        if ((ratio < 0) && (ratio > 1))
        {
            Debugger("Noise Ratio is outside of bounds (0,1)");
            return;
        }
        SetNoise(ratio * m_MaxFrequency, ratio * m_MaxAmplitude);
    }

    public void SetHeadBobbing(bool active)
    {
        m_enableHeadBobbing = active;
    }

    private void Debugger(object log)
    {
        if (m_debugger) Debug.Log($"[{this.name.ToUpper()}] {log}");
    }
}

using Google.Protobuf.WellKnownTypes;
using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpeedTracker : MonoBehaviour
{
    [SerializeField] private float[] m_FloorHeights;
    [SerializeField, Range(0,10f)] private float m_Multiplier;
    [SerializeField, Range(0,10f)] private float m_BufferDistance = 3f;
    [SerializeField, Tooltip("The number of checks per second")] private float m_Frequency = 4;

    private FirstPersonController m_FirstPersonController;

    private float m_Timer;
    private float m_InverseMultiplier;
    private float m_IntervalLength;
    private int m_FloorIndex;
    private bool m_IsPlaying;

    protected virtual void Start()
    {
        m_FirstPersonController = GetComponent<FirstPersonController>();
        if (!m_FirstPersonController.isLocalPlayer) return;
        m_InverseMultiplier = 1 / m_Multiplier;
        m_IntervalLength = 1 / m_Frequency;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        m_IsPlaying = NewNetworkManager.NewSingleton.IsGameplayScene(scene.name);
    }

    protected virtual void Update()
    {
        if (!m_IsPlaying) return;

        if (m_Timer > m_IntervalLength)
        {
            m_Timer = 0;
            CheckPlayerDistance();
        }

        m_Timer += Time.deltaTime;
    }

    protected virtual void CheckPlayerDistance()
    {
        float difference = transform.position.y - m_FloorHeights[m_FloorIndex];

        if (MathF.Abs(difference) <= m_BufferDistance) return;

        if (difference < 0)
        {
            --m_FloorIndex;
            m_FirstPersonController.SpeedMultiplier(m_InverseMultiplier);
        }
        else
        {
            ++m_FloorIndex;
            m_FirstPersonController.SpeedMultiplier(m_Multiplier);
        }
    }

    protected virtual void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

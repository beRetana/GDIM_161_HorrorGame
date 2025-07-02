using System;
using UnityEngine;

public class PlayerDataTracker : MonoBehaviour
{
    private Vector3 m_SavedPosition;
    private float[] m_TimePerFloor = new float[5];
    private float m_StartTime;
    private float m_TotalTime;
    private ushort m_KnockedDownCount; 
    private ushort m_RezzedUpCount;

    public Vector3 SavedPosition { get { return m_SavedPosition; } set { m_SavedPosition = value; } }
    public float[] TimesPerFloor => m_TimePerFloor;
    public float TotalTime => m_TotalTime;
    public ushort KnockedDownCount => m_KnockedDownCount;
    public ushort RezzedUpCount => m_RezzedUpCount;

    private void Start()
    {
        m_SavedPosition = Vector3.zero;
        m_StartTime = Time.time;
    }

    public void OnReachedNewFloor(byte floor)
    {
        float previousTime = (floor != 0) ? m_TimePerFloor[floor-1] : m_StartTime;
        m_TimePerFloor[floor] = Time.time - previousTime;
    }

    public void OnEndGame()
    {
        m_TotalTime = Time.time - m_StartTime;
    }

    public void OnPlayerKnocked()
    {
        ++m_KnockedDownCount;
    }

    public void OnPlayerRezzed()
    {
        ++m_RezzedUpCount;
    }
}

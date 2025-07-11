using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System;

public class SectionManager : NetworkBehaviour
{
    [SerializeField] private ObjectActivator[] m_Activators;
    [SerializeField, Tooltip("Sort: 0 = Lowest, 3 = Highest")]
    private float[] m_FloorHeights;
    [SerializeField] private float m_ChecksPerSecond;

    private Transform m_PlayerTransform;
    private float m_Frequency;
    private float m_Timer;

    private void Start()
    {
        GetPlayer();
        if (!isServer) return;
        m_Frequency = 1f / m_ChecksPerSecond;
    }

    private void GetPlayer()
    {
        PlayerObjectController[] players = FindObjectsByType<PlayerObjectController>(FindObjectsSortMode.None);
        
        foreach(PlayerObjectController player in players)
        {
            if (!player.isLocalPlayer) continue;
            m_PlayerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (!isServer) return;
        if (m_Timer >= m_Frequency)
        {
            m_Timer = 0f;
            UpdateActivators();
        }
        m_Timer += Time.deltaTime;
    }

    [ClientRpc]
    private void UpdateActivators()
    {
        byte newFloor = 0;
        for (byte i = 0; i < m_FloorHeights.Length; ++i)
        {
            if (m_FloorHeights[i] > m_PlayerTransform.position.y)
            {
                break;
            }
            newFloor = i;
        }

        foreach (ObjectActivator activator in m_Activators)
        {
            activator.UpdateObjectsState(newFloor, m_PlayerTransform.position);
        }
    }


}

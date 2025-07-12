using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System;

public class SectionManager : NetworkBehaviour
{
    [SerializeField] private ObjectActivator[] m_Activators;
    [SerializeField] private float m_ChecksPerSecond;

    private Transform[] m_PlayerTransforms;
    private float m_Frequency;
    private float m_Timer;

    private void Start()
    {
        if (!isServer) return;
        GetPlayers();
        m_Frequency = 1f / m_ChecksPerSecond;
    }

    [Server]
    private void GetPlayers()
    {
        PlayerObjectController[] players = FindObjectsByType<PlayerObjectController>(FindObjectsSortMode.None);
        m_PlayerTransforms = new Transform[players.Length];
        for (int i = 0; i < players.Length; ++i)
        {
            m_PlayerTransforms[i] = players[i].transform;
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

    [Server]
    private void UpdateActivators()
    {
        Vector3[] positions = new Vector3[m_PlayerTransforms.Length];

        for (int i = 0; i < positions.Length; ++i)
        {
            positions[i] = m_PlayerTransforms[i].position;
        }

        foreach (ObjectActivator activator in m_Activators)
        {
            activator.UpdateObjectsState(positions);
        }
    }
}

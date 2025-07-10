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

    public Transform[] PlayerTransforms => m_PlayerTransforms;

    private void Start()
    {
        if (!isServer) return;
        m_Frequency = 1f / m_ChecksPerSecond;
        GetPlayers();
    }

    private void GetPlayers()
    {
        PlayerBase[] players = FindObjectsByType<PlayerBase>(FindObjectsSortMode.None);
        m_PlayerTransforms = new Transform[players.Length];
        for (int i = 0; i < players.Length; ++i)
        {
            m_PlayerTransforms[i] = players[i].transform;
        }
    }

    private void Update()
    {
        if (m_Timer >= m_Frequency)
        {
            m_Timer = 0f;
        }
        m_Timer -= Time.deltaTime;
    }

    private void UpdateActivators()
    {
        foreach (ObjectActivator activator in m_Activators)
        {
            //activator.
        }
    }


}

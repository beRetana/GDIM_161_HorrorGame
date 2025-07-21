using Mirror;
using OtherUtils;
using System;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour, IDebugger
{
    [SerializeField] private FloorSpawner[] m_FloorSpawners;
    [SerializeField] private Transform m_Item;
    [SerializeField] private float m_SpawnProbability;
    [SerializeField] private byte m_MinItems;

    private float m_Chance;
    private bool m_Debugger;

    [Serializable]
    private struct FloorSpawner
    {
        public Transform[] Locations;
    }

    private void Start()
    {
        if (!isServer) return;
        PopulateGame();
    }

    [Server]
    private void PopulateGame()
    {
        foreach (FloorSpawner floor in m_FloorSpawners)
        {
            Debugger($"Spawning Items in Floor");
            for (int index = 0; index < floor.Locations.Length; ++index)
            {
                if (index < m_MinItems)
                {
                    Debugger($"Spawning {index} minimum items.");
                    SpawnItems(floor.Locations[index]);
                    continue;
                }

                m_Chance = UnityEngine.Random.Range(0f, 1f);
                Debugger($"Chance was {m_Chance}");
                if (m_Chance > m_SpawnProbability) continue;
                Debugger($"Spawning Chance item {index}");
                SpawnItems(floor.Locations[index]);
            }
        }
    }

    [Server]
    private void SpawnItems(Transform location)
    {
        Transform item = Instantiate(m_Item, location.position, location.rotation);
        NetworkServer.Spawn(item.gameObject);
    }

    public void Debugger(object log)
    {
        if (m_Debugger) Debug.Log($"[{GetType().ToString()}]: {log}");
    }

    public void SetDebugActive(bool value)
    {
        m_Debugger = value;
    }
}

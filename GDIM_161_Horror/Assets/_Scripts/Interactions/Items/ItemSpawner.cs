using Mirror;
using System;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private FloorSpawner[] m_FloorSpawners;
    [SerializeField] private Transform m_Item;
    [SerializeField] private float m_SpawnProbability;
    [SerializeField] private byte m_MinItems;

    private float m_Chance;

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
            for (int index = 0; index < floor.Locations.Length; ++index)
            {
                if (index < m_MinItems)
                {
                    SpawnItems(floor.Locations[index]);
                }

                m_Chance = UnityEngine.Random.Range(0f, 1f);

                if (m_Chance > m_SpawnProbability) return;

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
}

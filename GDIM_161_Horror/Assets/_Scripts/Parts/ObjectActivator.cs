using Mirror;
using OtherUtils;
using System;
using System.Collections;
using UnityEngine;

public class ObjectActivator : NetworkBehaviour, IDebugger
{
    [SerializeField] private Transform m_ObjectModel;
    [SerializeField] private Transform[] m_SpawnLocations;
    [SerializeField] private float m_ProximityRange;

    private const float HEIGHT_BUFFER = 2f;

    private float m_SqrProxRange;
    private bool m_Debugger;

    private void Start()
    {
        m_SqrProxRange = m_ProximityRange * m_ProximityRange;
        if (!isServer) return;
        StartCoroutine(PopulateServerPool());
    }

    [Server]
    private IEnumerator PopulateServerPool()
    {
        Debugger($"Server: Populating Server Pools");
        for(int i = 0; i < m_SpawnLocations.Length; ++i)
        {
            GameObject newObject = Instantiate(m_ObjectModel, m_SpawnLocations[i].position,
                                       m_SpawnLocations[i].rotation).gameObject;
            NetworkServer.Spawn(newObject);
            ServerSetActiveObject(newObject, false);
            m_SpawnLocations[i] = newObject.transform;
            yield return null; // Spawn one per frame to avoid FPS drop
        }
        Debugger($"Server: Finished Populating Server Pools");
    }

    [Server]
    public void UpdateObjectsState(Vector3[] playerLocations)
    {
        Debugger($"Server: Updating Player location: {playerLocations}");
        foreach (Transform pooledObject in m_SpawnLocations)
        {
            if (pooledObject == null) continue;

            bool isObjectInRange = false;

            foreach (Vector3 position in playerLocations)
            {
                if (HEIGHT_BUFFER < Mathf.Abs(pooledObject.position.y - position.y)) continue;
                
                Vector3 distance = pooledObject.position - position;
                if (m_SqrProxRange < (distance.x * distance.x) + (distance.z * distance.z)) continue;

                isObjectInRange = true;
                break;
            }

            bool isObjectActive = pooledObject.gameObject.activeSelf;

            if (isObjectInRange && !isObjectActive)
            {
                ServerSetActiveObject(pooledObject.gameObject, active: true);
            }
            else if (!isObjectInRange && isObjectActive)
            {
                ServerSetActiveObject(pooledObject.gameObject, active: false);
            }
        }
    }

    [Server]
    private void ServerSetActiveObject(GameObject objectToSet, bool active)
    {
        Debugger($"Server: Setting {objectToSet.name} to active:{active}");
        NetworkIdentity networkIdentity;
        if (!objectToSet.TryGetComponent<NetworkIdentity>(out networkIdentity)) return;
        ClientSetActiveObject(networkIdentity.netId, active);
    }

    [ClientRpc]
    private void ClientSetActiveObject(uint networkID, bool active)
    {
        Debugger($"Client: Setting object of ID {networkID} to active:{active}");
        NetworkClient.spawned[networkID].gameObject.SetActive(active);
    }

    public void Debugger(object log)
    {
        if (m_Debugger) Debug.Log($"[{GetType().ToString()}]: {log}");
    }

    public void SetDebugActive(bool active)
    {
        m_Debugger = active;
    }
}

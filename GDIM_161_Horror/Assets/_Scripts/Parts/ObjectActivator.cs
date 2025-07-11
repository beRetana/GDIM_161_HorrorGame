using Mirror;
using OtherUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActivator : NetworkBehaviour, IDebugger
{
    [SerializeField] private Transform m_ObjectModel;
    [SerializeField] private FloorPool[] m_LocationPools;
    [SerializeField] private float m_ProximityRange;

    private float m_SqrProxRange;
    private byte m_CurrentFloor;
    private bool m_Debugger;

    private void Start()
    {
        m_SqrProxRange = m_ProximityRange * m_ProximityRange;
        if (!isServer) return;
        StartCoroutine(PopulateServerPool());
    }

    [Serializable]
    private enum Floor
    {
        Forest,
        First,
        Second,
        Third
    }

    [Serializable]
    private struct FloorPool
    {
        public Floor Floor;
        public Transform[] Locations;
    }

    [Server]
    private IEnumerator PopulateServerPool()
    {
        Debugger($"Server: Populating Server Pools");
        foreach(FloorPool floorPool in m_LocationPools)
        {
            Transform[] pool = floorPool.Locations;
            for (int i = 0; i < pool.Length; ++i)
            {
                GameObject newObject = Instantiate(m_ObjectModel, pool[i].position,
                                       pool[i].rotation).gameObject;
                NetworkServer.Spawn(newObject);
                ServerSetActiveObject(newObject, false);
                pool[i] = newObject.transform;
                yield return null; // Spawn one per frame to avoid FPS drop
            }
        }
        Debugger($"Server: Finished Populating Server Pools");
    }

    public void UpdateObjectsState(byte newFloor, Vector2 playerLocation)
    {
        Debugger($"Client: Updating to floor: {newFloor} and location: {playerLocation}");
        if (newFloor != m_CurrentFloor)
        {
            DeactivateAllObjects(m_CurrentFloor);
            ActivateNearObjects(newFloor, playerLocation);
            m_CurrentFloor = newFloor;
        }
        else
        {
            ActivateNearObjects(m_CurrentFloor, playerLocation);
        }
    }

    [Command(requiresAuthority =false)]
    private void DeactivateAllObjects(byte floor)
    {
        Debugger($"Server: Deactivating all Objects in floor #{floor}");
        foreach (Transform objectPooled in m_LocationPools[floor].Locations)
        {
            ServerSetActiveObject(objectPooled.gameObject, false);
        }
    }

    [Command(requiresAuthority =false)]
    private void ActivateNearObjects(byte floor, Vector2 playerLocation)
    {
        Debugger($"Server: Activating Near Objects");
        foreach(Transform pooledObject in m_LocationPools[floor].Locations)
        {
            float playerToObjectDistance = ((Vector2)pooledObject.position - playerLocation).sqrMagnitude;
            bool isObjectInRange = m_SqrProxRange >= playerToObjectDistance;
            bool isObjectActive = pooledObject.gameObject.activeSelf;
            if (isObjectInRange && !isObjectActive)
            {
                ServerSetActiveObject(pooledObject.gameObject, active:true);
            }
            else if (!isObjectInRange && isObjectActive)
            {
                ServerSetActiveObject(pooledObject.gameObject, active:false);
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

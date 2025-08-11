using Mirror;
using Mono.CSharp;
using OtherUtils;
using System;
using System.Collections;
using UnityEngine;

public class ObjectActivator : Activator
{
    [SerializeField] private Transform m_ObjectModel;
    [SerializeField] private Transform[] m_SpawnLocations;
    [SerializeField] private float m_ProximityRange;

    private const float HEIGHT_BUFFER = 2f;
    private float m_SqrProxRange;
    private bool m_Loaded;

    private void OnEnable()
    {
        m_SqrProxRange = m_ProximityRange * m_ProximityRange;
        NewNetworkManager.NewSingleton.OnPlayersServerReady += StartPopulatingScene;
    }

    private void OnDisable()
    {
        NewNetworkManager.NewSingleton.OnPlayersServerReady -= StartPopulatingScene;
    }

    private void StartPopulatingScene()
    {
        if (m_Loaded) return;
        m_Loaded = true;
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
            m_SpawnLocations[i] = newObject.transform;
            yield return null; // Spawn one per frame to avoid FPS drop
        }
        Debugger($"Server: Finished Populating Server Pools");
    }

    [Server]
    public override void UpdateObjectsState(Vector3[] playerLocations)
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
        //Debugger($"Server: Setting {objectToSet.name} to active:{active}");
        NetworkIdentity networkIdentity;
        if (!objectToSet.TryGetComponent<NetworkIdentity>(out networkIdentity)) return;
        ClientSetActiveObject(networkIdentity.netId, active);
    }

    /*
        Try statement because depending on the client/host specs
        some of the objects might not be fully spawned by the time 
        the function gets called. This will cause the NetworkClient to
        throw an error if it doesn't find the object in the dictionary in
        the beginning frames after loading the scene.
    */
    [ClientRpc]
    private void ClientSetActiveObject(uint networkID, bool active)
    {
        //Debugger($"Client: Setting object of ID {networkID} to active:{active}");
        try
        {
            NetworkClient.spawned[networkID].gameObject.SetActive(active);
        }
        catch { Debugger($"Object of netID: {networkID} was not found"); }
    }
}

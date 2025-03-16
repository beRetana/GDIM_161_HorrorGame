using UnityEngine;
using Mirror;
using System.Collections;

namespace OtherUtils
{
    public class NetworkSpawner : NetworkBehaviour
    {
        [SerializeField] private Transform _prefab;
        [SerializeField] private float _delay;

        [Server]
        void Start() 
        { 
            if (isServer) StartCoroutine(SpawnPrefab()); 
        }

        private IEnumerator SpawnPrefab()
        {
            yield return new WaitForSecondsRealtime(_delay);
            Transform prefab = Instantiate(_prefab, transform.position, transform.rotation);
            NetworkServer.Spawn(prefab.gameObject);
        }
    }
}

using UnityEngine;
using Mirror;
using System.Collections;

namespace OtherUtils
{
    public class NetworkSpawner : NetworkBehaviour
    {
        [SerializeField] private Transform _prefab;
        [SerializeField] private float _delay;

        void Start() 
        { 
            if (isServer) StartCoroutine(SpawnPrefab()); 
        }

        [Server]
        private IEnumerator SpawnPrefab()
        {
            yield return new WaitForSecondsRealtime(_delay);
            Transform prefab = Instantiate(_prefab, transform.position, transform.rotation);
            NetworkServer.Spawn(prefab.gameObject);
            yield return null;
            gameObject.SetActive(false);
        }
    }
}

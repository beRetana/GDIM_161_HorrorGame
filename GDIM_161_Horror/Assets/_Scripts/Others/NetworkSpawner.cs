using UnityEngine;
using Mirror;
using System.Collections;

namespace OtherUtils
{
    public class NetworkSpawner : NetworkBehaviour
    {
        [SerializeField] private Transform _prefab;

        protected void Start() 
        {
            if (!isServer) return;

            Transform prefab = Instantiate(_prefab, transform.position, transform.rotation);
            NetworkServer.Spawn(prefab.gameObject);
            gameObject.SetActive(false);
        }
    }
}

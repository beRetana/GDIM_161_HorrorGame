using Mirror;
using System.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class BonePiece : NetworkBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void StartLifeTimer(float lifeTime)
    {
        StartCoroutine(DisableTimer(lifeTime));
    }

    private IEnumerator DisableTimer(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Debug.Log("Time To DeSpawn");
        if (isServer) DisablePiece();
        gameObject.SetActive(false);
    }

    [Server]
    private void DisablePiece()
    {
        NetworkServer.UnSpawn(gameObject);
    }
}

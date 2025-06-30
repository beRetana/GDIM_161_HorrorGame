using Mirror;
using System.Collections;
using UnityEngine;

public class BonePiece : NetworkBehaviour
{
    [ClientRpc]
    private void RpcSetOff(bool newValue)
    {
        gameObject.SetActive(newValue);
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
    }

    [Server]
    private void DisablePiece()
    {
        RpcSetOff(false);
        NetworkServer.UnSpawn(gameObject);
    }
}

using Mirror;
using System.Collections;
using UnityEngine;

public class BonePiece : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetOff))] private bool m_State;

    private void SetOff(bool newValue, bool oldValue)
    {
        m_State = newValue;
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
        m_State = false;
        NetworkServer.UnSpawn(gameObject);
    }
}

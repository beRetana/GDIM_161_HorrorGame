using Mirror;
using System.Collections;
using UnityEngine;

public class Pavlov : NetworkBehaviour
{
    [SerializeField] private Animator m_Animator;
    [SerializeField] private GameObject m_Food;
    [SerializeField, Range(0f, 100f)] private float m_RangeToOpen = 10f;
    [SerializeField, Range(0f, 600f)] private float m_DelayToClose = 30f;
    [SerializeField] private bool m_debugger;

    private Transform m_Enemy;
    private Coroutine m_Coroutine;
    private const string OPEN_PAVLOV = "OPEN_PAVLOV";
    private const string CLOSE_PAVLOV = "CLOSE_PAVLOV";
    private float m_RangeSquared;
    [SyncVar] private bool m_IsPavlovOpen;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            enabled = false;
            return;
        }
        m_RangeSquared = m_RangeToOpen * m_RangeToOpen;
    }

    public void ConnectEnemyToPavlov(NetworkIdentity enemy)
    {
        if (isServer) RpcConnectingEnemy(enemy);
        else CmdConnectingEnemy(enemy);
    }

    [Command]
    private void CmdConnectingEnemy(NetworkIdentity enemy)
    {
        RpcConnectingEnemy(enemy);
    }

    [ClientRpc]
    private void RpcConnectingEnemy(NetworkIdentity enemy)
    {
        if (m_IsPavlovOpen && m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
            m_Animator.SetTrigger(CLOSE_PAVLOV);
            m_IsPavlovOpen = false;
        }
        m_Enemy = enemy.transform;
    }

    private void Update()
    {
        if (m_Enemy != null && DistanceCheck() && !m_IsPavlovOpen)
        {
            if (isServer) RpcOpenPavlov();
            else CmdOpenPavlov();
        }
    }

    private bool DistanceCheck()
    {
        return m_RangeSquared >= (m_Enemy.position - transform.position).sqrMagnitude;
    }

    [Command]
    private void CmdOpenPavlov()
    {
        RpcOpenPavlov();
    }

    [ClientRpc]
    private void RpcOpenPavlov()
    {
        m_IsPavlovOpen = true;
        m_Animator.SetTrigger(OPEN_PAVLOV);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.doorsOpening, this.transform.position);
    }

    public void TakeFood()
    {
        if (isServer) RpcTakeFood();
        else CmdTakeFood(); ;
    }

    [Command]
    private void CmdTakeFood()
    {
        RpcTakeFood();
    }

    [ClientRpc]
    private void RpcTakeFood()
    {
        m_Coroutine = StartCoroutine(WaitToClose());
    }

    IEnumerator WaitToClose()
    {
        m_Food.SetActive(false);
        m_Enemy = null;
        yield return new WaitForSeconds(m_DelayToClose);
        m_Animator.SetTrigger(CLOSE_PAVLOV);
        m_IsPavlovOpen = false;
        AudioManager.instance.PlayOneShot(FMODEvents.instance.doorsClosing, this.transform.position);
    }

    // Getting called by animation.
    private void ResetFood()
    {
        m_Food.SetActive(true);
    }

    private void Debugger(object log)
    {
        if (m_debugger) Debug.Log(log);
    }
}

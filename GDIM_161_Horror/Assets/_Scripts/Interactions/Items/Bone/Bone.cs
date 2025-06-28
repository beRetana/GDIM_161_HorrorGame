using Grpc.Core;
using Interactions;
using Mirror;
using OtherUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bone : NetworkPickableItem
{
    [Space(5), Header("Instance Properties")]
    [SerializeField] protected Transform m_Template;
    [SerializeField] protected Transform m_SpawnPoint;
    [SerializeField] protected int m_MaxUses;
    [SerializeField] protected float m_LifeTime;

    protected Stack<Transform> m_BonePieces;

    protected int m_CurrentUses;

    protected override void Start()
    {
        base.Start();
        m_BonePieces = new Stack<Transform>(m_MaxUses);
        if (!isServer) return;
        SpawnPooledObject();
    }

    public override void UseItem(int playerID)
    {
        base.UseItem(playerID);
        if (m_CurrentUses >= m_MaxUses) KillObject();
        else DropBone();
    }

    protected void DropBone()
    {
        if (!isServer) CmdDropDone();
        else RpcDropBone();
    }

    [Command(requiresAuthority = false)]
    protected void CmdDropDone()
    {
        RpcDropBone();
    }

    [ClientRpc]
    protected void RpcDropBone()
    {
        Transform bone = m_BonePieces.Pop();
        bone.gameObject.SetActive(true);
        bone.parent = null;
        StartCoroutine(CollectBone(bone));
        m_CurrentUses++;
    }

    private void KillObject()
    {
        if (!isServer) CmdKillObject();
        else NetworkServer.Destroy(gameObject);
    }

    [Command(requiresAuthority = false)]
    private void CmdKillObject()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    private void SpawnPooledObject()
    {
        for (int i = 0; i < m_MaxUses; i++)
        {
            Transform piece = Instantiate(m_Template);
            if (isServer) NetworkServer.Spawn(piece.gameObject);
            piece.gameObject.SetActive(false);
            m_BonePieces.Push(piece);
        }
    }

    protected IEnumerator CollectBone(Transform bone)
    {
        yield return new WaitForSeconds(m_LifeTime);
        if (isServer) NetworkServer.UnSpawn(bone.gameObject);
    }
}

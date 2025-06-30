using Grpc.Core;
using Interactions;
using Mirror;
using OtherUtils;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class Bone : NetworkPickableItem
{
    [Space(5), Header("Instance Properties")]
    [SerializeField] protected Transform m_BonePiece;
    [SerializeField] protected Transform m_SpawnPoint;
    [SerializeField] protected float m_LifeTime;

    [SerializeField] protected int m_MaxUses;

    protected Stack<BonePiece> m_BonePieces;
    [SyncVar] protected int m_CurrentUses;

    protected override void Start()
    {
        base.Start();
        m_BonePieces = new Stack<BonePiece>();
        SpawnPooledObject();
    }

    private void SpawnPooledObject()
    {
        for (int i = 0; i < m_MaxUses; ++i)
        {
            BonePiece piece = Instantiate(m_BonePiece).GetComponent<BonePiece>();
            if (isServer) NetworkServer.Spawn(piece.transform.root.gameObject);
            piece.gameObject.SetActive(false);
            m_BonePieces.Push(piece);
        }
    }

    public override void UseItem(int playerID)
    {
        base.UseItem(playerID);
        Debugger("Pressed Use!");
        if (m_CurrentUses >= m_MaxUses)
        {
            //if (!isServer) CmdDisableBone(playerID);
            //else DisableBone(playerID);
            DisableBone(playerID);
        }
        else
        {
            Debugger("Dropping");
            //if (!isServer) CmdDropPiece(playerID);
            //else
            {
                ++m_CurrentUses;
                RpcDropPiece(playerID);
            }
        } 
    }

    [Command(requiresAuthority = false)]
    private void CmdDisableBone(int playerID)
    {
        DisableBone(playerID);
    }

    [Server]
    private void DisableBone(int playerID)
    {
        RpcDisableBone(playerID);
        NetworkServer.UnSpawn(transform.root.gameObject);
    }

    [ClientRpc]
    private void RpcDisableBone(int playerID)
    {
        PlayerManager.Instance.GetPlayer(playerID).GetComponent<HandInventory>().DropAction();
        gameObject.SetActive(false);
    }

    [Command(requiresAuthority = false)]
    protected void CmdDropPiece(int playerID)
    {
        Debugger("CMD: Dropping");
        ++m_CurrentUses;
        RpcDropPiece(playerID);
    }

    [ClientRpc]
    protected void RpcDropPiece(int playerID)
    {
        Debugger("RPC: Dropping");
        BonePiece bone = m_BonePieces.Pop();
        if (bone == null) return;
        bone.transform.position = m_SpawnPoint.position;
        bone.gameObject.SetActive(true);
        StartCoroutine(bone.DisableTimer(m_LifeTime));
    }
}

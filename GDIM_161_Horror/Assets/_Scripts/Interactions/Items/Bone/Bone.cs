using Grpc.Core;
using Interactions;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Bone : NetworkPickableItem
{
    [Space(5), Header("Instance Properties")]
    [SerializeField] protected Transform m_BonePiece;
    [SerializeField] protected Transform m_SpawnPoint;
    [SerializeField] protected LayerMask m_CrushableLayers;
    [SerializeField] protected LayerMask m_ObstructableLayers;
    [SerializeField] protected string m_CrushingText = "Hold To Crush Bone";
    [SerializeField] protected float m_CrushingDistance = 1.2f;
    [SerializeField] protected int m_MaxUses;

    protected Stack<BonePiece> m_BonePieces;
    protected Transform m_PlayerCameraTransform;
    protected PlayerInteractableUI m_InteractableUI;
    [SyncVar] protected int m_CurrentUses;
    protected bool m_IsOnDominantHand;
    protected BoneState m_State;

    public enum BoneState
    {
        Uncrushed,
        CanCrush,
        Crushing,
        Crushed
    }

    protected override void Start()
    {
        base.Start();
        m_BonePieces = new Stack<BonePiece>();
        SpawnPooledObject();
        m_State = BoneState.Uncrushed;
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

    public override void SetPossessed(bool toPossess, int playerID = 0)
    {
        base.SetPossessed(toPossess, playerID);

        if (toPossess)
        {
            HandInventory inventory = PlayerManager.Instance.GetPlayer(playerID).
                GetComponent<HandInventory>();

            m_PlayerCameraTransform = inventory.GetComponent<PlayerBase>().CameraTransform;
            m_InteractableUI = inventory.GetComponent<PlayerInteractableUI>();
            inventory.OnSwapingHands += OnSwappedHands;
            OnSwappedHands(inventory.PeekAtDominant());
        }
        else
        {
            PlayerManager.Instance.GetPlayer(playerID).
                GetComponent<HandInventory>().OnSwapingHands -= OnSwappedHands;
            m_PlayerCameraTransform = null;
            m_InteractableUI = null;
        }   
    }

    private void OnSwappedHands(NetworkPickableItem item)
    {
        if (m_State == BoneState.Crushed) return;

        uint itemID = item.transform.root.GetComponent<NetworkIdentity>().netId;
        if (_interactableItem.GetNetworkID().netId == itemID)
        {
            m_IsOnDominantHand = true;
        }
        else if (m_IsOnDominantHand)
        {
            m_IsOnDominantHand = false;
        }
    }

    private void Update()
    {
        CheckForWalls();
    }

    private void CheckForWalls()
    {
        if (m_State == BoneState.Crushed || !m_IsOnDominantHand) return;

        bool hasWallInFront = Physics.Raycast(m_PlayerCameraTransform.position, m_PlayerCameraTransform.forward, 4f, m_CrushableLayers);
        bool isObstructed = Physics.Raycast(m_PlayerCameraTransform.position, m_PlayerCameraTransform.forward, 4f, m_ObstructableLayers);
        bool canCrushBone = hasWallInFront && !isObstructed;

        Debugger($"Bone State is: {m_State} and can crush bune: {canCrushBone}");

        switch (m_State)
        {
            case BoneState.Uncrushed:
                if (!canCrushBone) return;
                m_InteractableUI.DisplayInteractUI(m_CrushingText);
                m_State = BoneState.CanCrush;
                break;
            case BoneState.CanCrush:
            case BoneState.Crushing:
                if (canCrushBone) return;
                m_InteractableUI.CancelHoldingUI();
                m_InteractableUI.HideInteractUI();
                m_State = BoneState.Uncrushed;
                break;
        }
    }

    public override void UseItem(int playerID, InputData context)
    {
        base.UseItem(playerID, context);
        Debugger($"Player {playerID} sent input of Type: {context.InputType}" +
                 $" and Phase: {context.InputPhase}");
        switch (m_State)
        {
            case BoneState.Uncrushed:
                Debugger("Player used Item in Uncrushed state: Nothing to do");
                break;
            case BoneState.CanCrush:
                if (InputActionPhase.Started != context.InputPhase) return;
                m_InteractableUI.StartHoldingUI();
                m_State = BoneState.Crushing;
                Debugger($"Bone's previous state: {BoneState.CanCrush}," +
                         $"new state: {m_State}");
                break;
            case BoneState.Crushing:
                CrushingInput(context);
                break;
            case BoneState.Crushed:
                if (context.InputPhase != InputActionPhase.Performed) return;
                if (context.InputType != InteractionType.Tap) return;
                TrialDropping(playerID);
                break;
        }
    }

    private void CrushingInput(InputData context)
    {
        bool isPerformed = context.InputPhase == InputActionPhase.Performed;
        bool isHoldAction = context.InputType == InteractionType.Hold;

        if (isPerformed && isHoldAction)
        {
            // change model?
            m_State = BoneState.Crushed;
        }
        else
        {
            m_State = BoneState.CanCrush;
        }
        m_InteractableUI.HideInteractUI();
        m_InteractableUI.CancelHoldingUI();
    }

    private void TrialDropping(int playerID)
    {
        if (m_CurrentUses >= m_MaxUses - 1)
        {
            Debugger("Last use: Will auto-destroy");
            if (!isServer) return;
            RpcDropPiece(playerID);
            DisableBone(playerID);
        }
        else
        {
            Debugger("Dropping");
            if (!isServer) return;
            ++m_CurrentUses;
            RpcDropPiece(playerID);
        }
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

    [ClientRpc]
    protected void RpcDropPiece(int playerID)
    {
        Debugger("RPC: Dropping");
        BonePiece bone = m_BonePieces.Pop();
        if (bone == null) return;
        bone.transform.position = m_SpawnPoint.position;
        bone.gameObject.SetActive(true);
        bone.StartLifeTimer(5f);
    }
}

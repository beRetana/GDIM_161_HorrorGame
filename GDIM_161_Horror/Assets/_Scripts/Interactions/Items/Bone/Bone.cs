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
    protected PlayerInteractionsHUD m_PlayerHUD;
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
        m_State = BoneState.Uncrushed;
        if (!isServer) return;
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

    public override void SetPossessed(bool toPossess, int playerID = 0)
    {
        base.SetPossessed(toPossess, playerID);

        if (toPossess)
        {
            HandInventory inventory = PlayerManager.Instance.GetPlayer(playerID).
                GetComponent<HandInventory>();

            m_PlayerCameraTransform = inventory.GetComponent<PlayerBase>().CameraTransform;
            m_PlayerHUD = inventory.GetComponent<PlayerInteractionsHUD>();
            inventory.OnSwapingHands += OnSwappedHands;
            OnSwappedHands(inventory.PeekAtDominant());
        }
        else
        {
            PlayerManager.Instance.GetPlayer(playerID).
                GetComponent<HandInventory>().OnSwapingHands -= OnSwappedHands;
            m_PlayerCameraTransform = null;
            m_PlayerHUD = null;
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
            ChangeBoneState(BoneState.Uncrushed);
            m_PlayerHUD.CancelHoldingUI();
            m_PlayerHUD.HideInteractUI();
        }
    }

    private void Update()
    {
        if (!_isPossessed) return;
        CheckForWalls();
    }

    private void CheckForWalls()
    {
        if (!m_IsOnDominantHand || m_State == BoneState.Crushed) return;

        bool hasWallInFront = Physics.Raycast(m_PlayerCameraTransform.position, m_PlayerCameraTransform.forward, 4f, m_CrushableLayers);
        bool isObstructed = Physics.Raycast(m_PlayerCameraTransform.position, m_PlayerCameraTransform.forward, 4f, m_ObstructableLayers);
        bool canCrushBone = hasWallInFront && !isObstructed;

        //Debugger($"Bone State is: {m_State} and can crush bune: {canCrushBone}");

        switch (m_State)
        {
            case BoneState.Uncrushed:
                if (!canCrushBone) return;
                m_PlayerHUD.SetIconLeftClick();
                m_PlayerHUD.DisplayInteractUI(m_CrushingText);
                ChangeBoneState(BoneState.CanCrush);
                break;
            case BoneState.CanCrush:
            case BoneState.Crushing:
                if (canCrushBone) return;
                m_PlayerHUD.CancelHoldingUI();
                m_PlayerHUD.HideInteractUI();
                m_PlayerHUD.SetIconKeyboardE();
                ChangeBoneState(BoneState.Uncrushed);
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
                if (InteractionType.Tap == context.InputType) return;
                m_PlayerHUD.StartHoldingUI();
                ChangeBoneState(BoneState.Crushing);
                break;
            case BoneState.Crushing:
                if (InteractionType.Tap == context.InputType) return;
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
        switch (context.InputPhase)
        {
            case InputActionPhase.Canceled:
                m_PlayerHUD.CancelHoldingUI();
                ChangeBoneState(BoneState.CanCrush);
                break;
            case InputActionPhase.Performed:
                m_PlayerHUD.CancelHoldingUI();
                m_PlayerHUD.HideInteractUI();
                m_PlayerHUD.SetIconKeyboardE();
                ChangeBoneState(BoneState.Crushed);
                break;
        }
    }

    private void TrialDropping(int playerID)
    {
        if (m_CurrentUses >= m_MaxUses - 1)
        {
            if (!isServer) return;
            Debugger("Last use: Will auto-destroy");
            DropPiece(playerID);
            DisableBone(playerID);
        }
        else
        {
            Debugger("Dropping");

            if (!isServer) CmdDropPiece(playerID);
            else DropPiece(playerID);
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
        HandInventory inventory = PlayerManager.Instance.GetPlayer(playerID).GetComponent<HandInventory>();
        inventory.DropAction();
        inventory.OnSwapingHands -= OnSwappedHands;
        gameObject.SetActive(false);
    }

    [Command]
    protected void CmdDropPiece(int playerID)
    {
        Debugger("CMD: Dropping");
        DropPiece(playerID);
    }

    [Server]
    protected void DropPiece(int playerID)
    {
        ++m_CurrentUses;
        BonePiece bone = m_BonePieces.Pop();
        if (bone == null) return;
        bone.transform.position = m_SpawnPoint.position;
        bone.gameObject.SetActive(true);
        bone.StartLifeTimer(5f);
    }

    protected void ChangeBoneState(BoneState state)
    {
        if (isServer) RpcChangeBoneState(state);
        else CmdChangeBoneState(state);
    }

    [Command]
    protected void CmdChangeBoneState(BoneState state)
    {
        Debugger($"CMD - Old State: {m_State}, New State: {state}");
        RpcChangeBoneState(state);
    }

    [ClientRpc]
    protected void RpcChangeBoneState(BoneState state)
    {
        Debugger($"RPC - Old State: {m_State}, New State: {state}");
        m_State = state;
    }
}

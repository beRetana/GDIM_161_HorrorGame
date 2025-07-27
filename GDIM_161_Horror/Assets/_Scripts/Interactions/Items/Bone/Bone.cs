using Grpc.Core;
using Interactions;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Bone : NetworkPickableItem
{
    [Space(5), Header("Instance Properties")]
    [SerializeField] protected Transform[] m_BonePieceModels;
    [SerializeField] protected Transform[] m_SpawnLocation;
    [SerializeField] protected Transform m_CrushedBone;
    [SerializeField] protected Transform m_NormalBone;
    [SerializeField] protected Animator m_BoneAnimator;
    [SerializeField] protected LayerMask m_CrushableLayers;
    [SerializeField] protected LayerMask m_ObstructableLayers;
    [SerializeField] protected string m_CrushingText = "Hold To Crush Bone";
    [SerializeField] protected float m_CrushingDistance = 1.2f;

    protected Stack<BonePiece> m_BonePieces;
    protected Transform m_PlayerCameraTransform;
    protected PlayerInteractionsHUD m_PlayerHUD;
    protected const string CRUSHING = "CRUSHING";
    protected int m_CurrentUses; 
    protected int m_MaxUses;
    protected bool m_IsOnDominantHand;
    [SyncVar] protected BoneState m_State;

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
        m_MaxUses = m_BonePieceModels.Length;
        m_State = BoneState.Uncrushed;
        SpawnPooledObject();
    }

    private void SpawnPooledObject()
    {
        for (int i = 0; i < m_BonePieceModels.Length; ++i)
        {
            BonePiece piece = Instantiate(m_BonePieceModels[i]).GetComponent<BonePiece>();
            if (isServer) NetworkServer.Spawn(piece.transform.root.gameObject);
            m_BonePieces.Push(piece);
            piece.gameObject.SetActive(false);
        }
    }

    [ClientRpc]
    public override void RpcSetPossessed(bool toPossess, int playerID)
    {
        base.RpcSetPossessed(toPossess, playerID);

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
        
        if (item != null)
        {
            uint itemID = item.transform.root.GetComponent<NetworkIdentity>().netId;
            if (_interactableItem.GetNetworkID().netId == itemID)
            {
                m_IsOnDominantHand = true;
            }
            else if (m_IsOnDominantHand)
            {
                NotDominanteState();
            }
        }
        else
        {
            NotDominanteState();
        }
    }

    private void NotDominanteState()
    {
        m_IsOnDominantHand = false;
        ChangeBoneState(BoneState.Uncrushed);
        m_PlayerHUD?.CancelHoldingUI();
        m_PlayerHUD?.HideInteractUI();
        m_PlayerHUD?.SetIconKeyboardE();
    }

    private void Update()
    {
        if (!_isPossessed) return;
        CheckForWalls();
    }

    private void CheckForWalls()
    {
        if (!m_IsOnDominantHand || m_State == BoneState.Crushed || m_PlayerCameraTransform == null) return;

        bool hasWallInFront = Physics.Raycast(m_PlayerCameraTransform.position, 
            m_PlayerCameraTransform.forward, m_CrushingDistance, m_CrushableLayers);
        bool isObstructed = Physics.Raycast(m_PlayerCameraTransform.position, 
            m_PlayerCameraTransform.forward, m_CrushingDistance, m_ObstructableLayers);
        
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
                m_PlayerHUD?.StartHoldingUI();
                m_BoneAnimator.SetBool(CRUSHING, true);
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
                m_BoneAnimator.SetBool(CRUSHING, false);
                ChangeBoneState(BoneState.CanCrush);
                break;
            case InputActionPhase.Performed:
                m_PlayerHUD.CancelHoldingUI();
                m_PlayerHUD.HideInteractUI();
                m_PlayerHUD.SetIconKeyboardE();
                OnCrushed();
                ChangeBoneState(BoneState.Crushed);
                break;
        }
    }

    private void OnCrushed()
    {
        if (isServer) RpcOnCrushed();
        else CmdOnCrushed();
    }

    [ClientRpc]
    private void RpcOnCrushed()
    {
        m_NormalBone.gameObject.SetActive(false);
        m_CrushedBone.gameObject.SetActive(true);
    }

    [Command]
    private void CmdOnCrushed()
    {
        RpcOnCrushed();
    }

    private void TrialDropping(int playerID)
    {
        if (!isServer) return;
        DropPiece(playerID, m_CurrentUses >= m_MaxUses - 1);
    }

    [Server]
    protected void DropPiece(int playerID, bool isLast)
    {
        RpcDropPiece(playerID, isLast);
    }

    [ClientRpc]
    protected void RpcDropPiece(int playerID, bool isLast)
    {
        Debugger($"Dropping bone piece");
        BonePiece bone = m_BonePieces.Pop();
        if (bone == null) return;
        m_SpawnLocation[m_CurrentUses].gameObject.SetActive(false);
        bone.transform.position = m_SpawnLocation[m_CurrentUses].position;
        bone.transform.rotation = m_SpawnLocation[m_CurrentUses].rotation;
        bone.gameObject.SetActive(true);
        ++m_CurrentUses;

        if (!isLast) return;

        Debugger("Last use: Will auto-destroy");

        HandInventory inventory = PlayerManager.Instance.GetPlayer(playerID).GetComponent<HandInventory>();
        inventory.DropAction();
        inventory.OnSwapingHands -= OnSwappedHands;
        gameObject.SetActive(false);

        if (isServer) NetworkServer.UnSpawn(transform.root.gameObject);
    }

    protected void ChangeBoneState(BoneState state)
    {
        if (isServer) RpcChangeBoneState(state);
        else CmdChangeBoneState(state);
    }

    [Command(requiresAuthority = false)]
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

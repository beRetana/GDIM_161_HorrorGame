using UnityEngine;
using System;
using Mirror;
using Player;
using OtherUtils;
using UnityEngine.InputSystem;
using Mono.CSharp;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
public class PlayerBase : NetworkBehaviour, IDebugger
{
    static private int _myID = 0; // 0, 1, 2, 3
    protected PlayerInput _playerInput;
    private NewNetworkManager _networkmanager;
    public event Action<byte, bool> OnPlayerUp;

    public NewNetworkManager NetworkManager
    {
        get
        {
            if (_networkmanager != null)
            {
                return _networkmanager;
            }
            return _networkmanager = NewNetworkManager.singleton as NewNetworkManager;
        }
    }

    //protected float m_
    protected bool m_EnableFunctionality;
    protected bool _debugger;

    #region enums
    public enum PlayerState
    {
        Locked,
        Unlocked,
        Limp,
        Downed
    }
    public enum PlayerActionEnum // NOT IN USE... yet
    {
        None = 0,               // 00000
        Idling = 1 << 0,        // 00001
        Running = 1 << 1,       // 00010
        Jumping = 1 << 2,       // 00100
        Interacting = 1 << 3,   // 01000
    }

    #endregion
    #region delegates


    #endregion
    #region SerializeFields

    [Header("State Stats Scritable Objects")]
    [SerializeField] private SO_PlayerStats lockedStats;
    [SerializeField] private SO_PlayerStats unlockedStats;
    [SerializeField] private SO_PlayerStats limpStats;
    [SerializeField] private SO_PlayerStats downedStats;

    [Space(5)]
    [SerializeField] protected CapsuleCollider _capsuleCollider;
    [SerializeField] protected InteractablePlayer _interaction;

    [Space(5)]
    [SerializeField] protected const string DOWN_PLAYER_TAG = "DownPlayer";
    [SerializeField] protected const string PLAYER_TAG = "Player";

    protected HandInventory _handInventory;
    protected PlayerAnimator _animator;
    protected CharacterController _controller;
    protected PlayerInteractionsHUD _playerUI;
    protected LayerMask _interactLayer = 9;
    protected LayerMask _playerLayer = 10;

    #endregion
    #region Useful Stats
    private SO_PlayerStats currentStats;
    private PlayerState playerStateEnum;

    public PlayerState CurrentState => playerStateEnum;

    #endregion
    #region Player Stats

    [SerializeField] protected GameObject cinemachineCameraTarget;
    public Transform CameraTransform => cinemachineCameraTarget.transform;

    protected Vector3 downCamPosition;
    protected Vector3 initialPosition;
    protected float moveSpeed;
    protected float sprintSpeed;
    protected float rotationSpeed;
    protected float accelerationRate;
    protected float decelerationRate;

    protected float jumpHeight;
    protected float gravity;

    protected float jumpTimeout;
    protected float fallTimeout;

    protected float groundedOffset;
    protected float groundedRadius;
    protected LayerMask groundLayers;

    protected float topClamp;
    protected float bottomClamp;

    // cinemachine
    protected float _cinemachineTargetPitch;

    // player
    protected float _speed;
    protected float _rotationVelocity;
    protected float _verticalVelocity;
    protected float _terminalVelocity = 53.0f;

    // timeout deltatime
    protected float _jumpTimeoutDelta;
    protected float _fallTimeoutDelta;

    #endregion

    //Animator anim;
    protected virtual void Start()
    {
        if (isLocalPlayer)
            _playerInput = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();
        _playerUI = GetComponent<PlayerInteractionsHUD>();
        _animator = GetComponent<PlayerAnimator>();
        _handInventory = GetComponent<HandInventory>();

        if (cinemachineCameraTarget == null)
            cinemachineCameraTarget = transform.Find("PlayerCameraRoot")?.gameObject;

        initialPosition = cinemachineCameraTarget.transform.localPosition;
        downCamPosition = new Vector3(0f, -0.8f, 0.6f);
        AssignID();
        UpdateState(PlayerState.Unlocked); 
    }
    public override string ToString() { return $"Player ID: {_myID}"; }

    #region PlayerStateMachine
    public void LockPlayer()
    {
        Debugger($"Locking Player{_myID}");
        EnterState(PlayerState.Locked);
    }
    public void UnlockPlayer()
    {
        Debugger($"Unlocking Player {_myID}");
        EnterState(PlayerState.Unlocked);
    }
    public void LimpPlayer()
    {
        Debugger($"Limping Player{_myID}");
        EnterState(PlayerState.Limp);
    }
    public void DownPlayer()
    {
        Debugger($"Locking Player{_myID}");
        EnterState(PlayerState.Downed);
    }

    private void UnlockPlayerSettings()
    {
        _interaction.OnPlayerRecued -= UnlockPlayer;
        _interaction.SetInteractive(false);
        gameObject.layer = _playerLayer;
        gameObject.tag = PLAYER_TAG;

        _controller.center = new Vector3(0f, .98f, 0f);
        _controller.height = 2f;
        _capsuleCollider.center = Vector3.up;
        _capsuleCollider.direction = 1;

        _animator.SetAnimCrawl(false);
        cinemachineCameraTarget.transform.localPosition = initialPosition;
        SetInputState(true);

        if (!isLocalPlayer) return;
        OnPlayerUp?.Invoke((byte)_handInventory.PlayerID, true);
    }

    private void DownPlayerSettings()
    {
        _interaction.SetInteractive(true);
        _interaction.OnPlayerRecued += UnlockPlayer;
        gameObject.layer = _interactLayer;
        gameObject.tag = DOWN_PLAYER_TAG;

        _controller.center = new Vector3(0f, .4f, 0f);
        _controller.height = .5f;
        _capsuleCollider.center = new Vector3(0f, 0.5f, 0f);
        _capsuleCollider.direction = 2;

        _animator.SetAnimCrawl(true);
        _handInventory.DropAllItems();
        SetInputState(false);
        cinemachineCameraTarget.transform.localPosition = downCamPosition;
        if (!isLocalPlayer || playerStateEnum == PlayerState.Downed) return;
        OnPlayerUp?.Invoke((byte)_handInventory.PlayerID, false);
    }
    private void EnterState(PlayerState enterState)
    {
        if (isServer) RpcChangeState(enterState);
        else CmdChangeState(enterState);
    }

    [Command]
    private void CmdChangeState(PlayerState enterState)
    {
        RpcChangeState(enterState);
    }

    [ClientRpc]
    private void RpcChangeState(PlayerState enterState)
    {
        Debugger($"{name} entering {enterState}");
        UpdateState(enterState);
    }
    protected void UpdateState(PlayerState enterState)
    {
        switch (enterState)
        {
            case PlayerState.Locked:
                currentStats = lockedStats;
                break;
            case PlayerState.Unlocked:
                UnlockPlayerSettings();
                currentStats = unlockedStats;
                break;
            case PlayerState.Limp:
                currentStats = limpStats;
                break;
            case PlayerState.Downed:
                DownPlayerSettings();
                currentStats = downedStats;
                break;
        }
        _playerUI.HideInteractUI();
        _playerUI.CancelHoldingUI();
        SetPlayerStats();
        playerStateEnum = enterState;
    }
    private bool SetPlayerStats()
    {
        if (!isLocalPlayer) return false;

        if (currentStats == null) return false;

        moveSpeed = currentStats.MoveSpeed;
        sprintSpeed = currentStats.SprintSpeed;
        accelerationRate = currentStats.AccelerationRate;
        decelerationRate = currentStats.DecelerationRate;

        jumpHeight = currentStats.JumpHeight;
        gravity = currentStats.Gravity;

        jumpTimeout = currentStats.JumpTimeout;
        fallTimeout = currentStats.FallTimeout;

        groundedOffset = currentStats.GroundedOffset;
        groundedRadius = currentStats.GroundedRadius;
        groundLayers = currentStats.GroundLayers;

        topClamp = currentStats.TopClamp;
        bottomClamp = currentStats.BottomClamp;

        return true;
    }

    #endregion PlayerState

    public int ID() { return _myID; }
    private bool AssignID()
    {
        int newID = GetComponent<PlayerObjectController>().PlayerID;

        if (newID != -1)
        {
            _myID = newID;
            Debugger($"Player {_myID} spawned");
            return true;
        }
        else
        {
            Debugger($"ERROR: Could not add self {this} to PlayerList. Destoring self.");
            Destroy(this);
            return false;
        }
    }

    public void SetInputState(bool active)
    {
        if (!isLocalPlayer) return;

        _handInventory.SetControlsActive(active);
        if (active)
        {
            _playerInput.actions["Swap"].Enable();
            _playerInput.actions["Interact"].Enable();
            _playerInput.actions["Drop"].Enable();
            _playerInput.actions["Throw"].Enable();
            _playerInput.actions["UseItem"].Enable();
            _playerInput.actions["Sprint"].Enable();
            _playerInput.actions["Jump"].Enable();
        }
        else
        {
            _playerInput.actions["Swap"].Disable();
            _playerInput.actions["Interact"].Disable();
            _playerInput.actions["Drop"].Disable();
            _playerInput.actions["Throw"].Disable();
            _playerInput.actions["UseItem"].Disable();
            _playerInput.actions["Sprint"].Disable();
            _playerInput.actions["Jump"].Disable();
        }
    }

    public void SetCameraRotationSpeed(float value)
    {
        rotationSpeed = value;
    }

    public void Debugger(object log)
    {
        if (_debugger) Debug.Log(log);
    }

    public void SetDebugActive(bool active)
    {
        _debugger = active;
    }
}

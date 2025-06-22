using UnityEngine;
using Mirror;
using Player;
using OtherUtils;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif
[RequireComponent(typeof(CharacterController))]
public class PlayerBase : NetworkBehaviour, IDebugger
{
    static private int _myID = 0; // 0, 1, 2, 3

#if ENABLE_INPUT_SYSTEM
    protected PlayerInput _playerInput;
#endif

    private NewNetworkManager _networkmanager;

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

    protected bool _debugger;

    #region enums
    public enum PlayerStateEnum
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
    [SerializeField] protected Arms _arms;
    [SerializeField] protected CapsuleCollider _capsuleCollider;
    [SerializeField] protected InteractablePlayer _interaction;

    [Space(5)]
    [SerializeField] protected const string DOWN_PLAYER_TAG = "DownPlayer";
    [SerializeField] protected const string PLAYER_TAG = "Player";

    protected HandInventory _handInventory;
    protected PlayerAnimator _animator;
    protected CharacterController _controller;
    protected LayerMask _interactLayer = 9;
    protected LayerMask _playerLayer = 10;

    #endregion
    #region Useful Stats
    private SO_PlayerStats currentStats;
    private PlayerStateEnum playerStateEnum;
    private PlayerActionEnum playerActionEnum;

    #endregion
    #region Player Stats

    [SerializeField] protected GameObject cinemachineCameraTarget;

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
#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#endif
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<PlayerAnimator>();
        _handInventory = GetComponent<HandInventory>();
        initialPosition = cinemachineCameraTarget.transform.localPosition;
        downCamPosition = new Vector3(0f, -0.8f, 0.6f);
        AssignID();
        EnterState(PlayerStateEnum.Unlocked);
    }
    public override string ToString() { return $"Player ID: {_myID}"; }

    #region PlayerStateMachine
    public void LockPlayer()
    {
        Debugger($"Locking Player{_myID}");
        EnterState(PlayerStateEnum.Locked);
    }
    public void UnlockPlayer()
    {
        Debugger($"Unlocking Player{_myID}");

        _controller.center = new Vector3(0f, .98f, 0f);
        _controller.height = 2f;
        _capsuleCollider.center = Vector3.up;
        _capsuleCollider.direction = 1;
        _animator?.SetAnimCrawl(false);
        cinemachineCameraTarget.transform.localPosition = initialPosition;
        SetInputState(true);

        EnterState(PlayerStateEnum.Unlocked);
    }
    public void LimpPlayer()
    {
        Debugger($"Limping Player{_myID}");
        EnterState(PlayerStateEnum.Limp);
    }
    public void DownPlayer()
    {
        Debugger($"Locking Player{_myID}");

        _controller.center = Vector3.zero;
        _controller.height = .5f;
        _capsuleCollider.center = new Vector3(0f, 0.5f, 0f);
        _capsuleCollider.direction = 2;
        _animator.SetAnimCrawl(true);
        _handInventory.DropAllItems();
        SetInputState(false);
        cinemachineCameraTarget.transform.localPosition = downCamPosition;

        EnterState(PlayerStateEnum.Downed);
    }

    private void UnlockPlayerSettings()
    {
        _interaction.gameObject.SetActive(false);
        _interaction.SetInteractive(false);
        gameObject.layer = _playerLayer;
        gameObject.tag = PLAYER_TAG;
    }

    private void DownPlayerSettings()
    {
        _interaction.gameObject.SetActive(true);
        _interaction.SetInteractive(true);
        _interaction.SetPlayerInteraction(UnlockPlayer);
        gameObject.layer = _interactLayer;
        gameObject.tag = DOWN_PLAYER_TAG;
    }
    private void EnterState(PlayerStateEnum enterState)
    {
        if (isServer) RpcChangeState(enterState);
        else CmdChangeState(enterState);
    }

    [Command]
    private void CmdChangeState(PlayerStateEnum enterState)
    {
        RpcChangeState(enterState);
    }

    [ClientRpc]
    private void RpcChangeState(PlayerStateEnum enterState)
    {
        Debugger($"{name} entering {enterState}");
        playerStateEnum = enterState;
        UpdateState(enterState);
    }
    private void UpdateState(PlayerStateEnum enterState)
    {
        switch (enterState)
        {
            case PlayerStateEnum.Locked:
                currentStats = lockedStats;
                break;
            case PlayerStateEnum.Unlocked:
                UnlockPlayerSettings();
                currentStats = unlockedStats;
                break;
            case PlayerStateEnum.Limp:
                currentStats = limpStats;
                break;
            case PlayerStateEnum.Downed:
                DownPlayerSettings();
                currentStats = downedStats;
                break;
        }
        SetPlayerStats();
    }
    private bool SetPlayerStats()
    {
        if (currentStats == null) return false;

        moveSpeed = currentStats.MoveSpeed;
        sprintSpeed = currentStats.SprintSpeed;
        rotationSpeed = currentStats.RotationSpeed;
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


        if (cinemachineCameraTarget == null)
        {
            cinemachineCameraTarget = this.transform.Find("PlayerCameraRoot")?.gameObject;
        }

        return true;
    }

    #endregion PlayerState

    public int ID() { return _myID; }
    private bool AssignID()
    {
        int newID = GetComponent<PlayerObjectController>().PlayerIdNumber;

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

    public void Debugger(object log)
    {
        if (_debugger) Debug.Log(log);
    }

    public void SetDebugActive(bool active)
    {
        _debugger = active;
    }
}

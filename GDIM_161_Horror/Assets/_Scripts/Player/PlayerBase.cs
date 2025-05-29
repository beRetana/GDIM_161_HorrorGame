using UnityEngine;
using Mirror;
using Player;

public class PlayerBase :
#if MIRROR
    NetworkBehaviour
#else
    MonoBehaviour
#endif
{
    static private int _myID = 0; // 0, 1, 2, 3
    [SerializeField] protected bool _debugger;

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

    #endregion
    #region Useful Stats
    private SO_PlayerStats currentStats;
    private PlayerStateEnum playerStateEnum;
    private PlayerActionEnum playerActionEnum;

    #endregion
    #region Player Stats

    [SerializeField] protected GameObject cinemachineCameraTarget;

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
        AssignID();
        SetPlayerStats();
        EnterState(PlayerStateEnum.Unlocked);
    }
    public override string ToString() { return $"Player ID: {_myID}"; }

    #region PlayerStateMachine
    public void LockPlayer()
    {
        Debugger($"Unlocking Player{_myID}");
        EnterState(PlayerStateEnum.Locked);
    }
    public void UnlockPlayer()
    {
        Debugger($"Locking Player{_myID}");
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
        EnterState(PlayerStateEnum.Downed);
    }
    private void EnterState(PlayerStateEnum enterState)
    {
#if MIRROR
        // Only use Mirror commands if networking is active
        if (Mirror.NetworkClient.active || Mirror.NetworkServer.active)
        {
            if (isServer)
                RpcChangeState(enterState);
            else
                CmdChangeState(enterState);
        }
        else
        {
            // Fallback to local state change in non-networked mode
            playerStateEnum = enterState;
            UpdateState(enterState);
        }
#else
        playerStateEnum = enterState;
        UpdateState(enterState);
#endif
    }

#if MIRROR
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
#endif

#if !MIRROR
    // For non-networked version, just call this directly
    private void RpcChangeState(PlayerStateEnum enterState)
    {
        Debugger($"{name} entering {enterState}");
        playerStateEnum = enterState;
        UpdateState(enterState);
    }
#endif

    private void UpdateState(PlayerStateEnum enterState)
    {
        switch (enterState)
        {
            case PlayerStateEnum.Locked:
                currentStats = lockedStats;
                break;
            case PlayerStateEnum.Unlocked:
                currentStats = unlockedStats;
                break;
            case PlayerStateEnum.Limp:
                currentStats = limpStats;
                break;
            case PlayerStateEnum.Downed:
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
        var poc = GetComponent<PlayerObjectController>();
        int newID = poc != null ? poc.PlayerIdNumber : -1;

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

    protected void Debugger(object log)
    {
        if (_debugger) Debug.Log(log);
    }
}

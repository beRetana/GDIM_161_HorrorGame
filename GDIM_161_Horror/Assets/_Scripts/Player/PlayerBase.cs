using UnityEngine;
using Mirror;
using Player;

public class PlayerBase : NetworkBehaviour
{
    [SyncVar] protected int _myID = -1;
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

    [SyncVar] protected float moveSpeed;
    [SyncVar] protected float sprintSpeed;
    [SyncVar] protected float rotationSpeed;
    [SyncVar] protected float accelerationRate;
    [SyncVar] protected float decelerationRate;

    [SyncVar] protected float jumpHeight;
    [SyncVar] protected float gravity;

    [SyncVar] protected float jumpTimeout;
    [SyncVar] protected float fallTimeout;

    [SyncVar] protected float groundedOffset;
    [SyncVar] protected float groundedRadius;
    protected LayerMask groundLayers;

    [SyncVar] protected float topClamp;
    [SyncVar] protected float bottomClamp;

    // cinemachine
    [SyncVar] protected float _cinemachineTargetPitch;

    // player
    [SyncVar] protected float _speed;
    [SyncVar] protected float _rotationVelocity;
    [SyncVar] protected float _verticalVelocity;
    [SyncVar] protected float _terminalVelocity = 53.0f;

    // timeout deltatime
    [SyncVar] protected float _jumpTimeoutDelta;
    [SyncVar] protected float _fallTimeoutDelta;

    #endregion

    //Animator anim;
    protected virtual void Start()
    {
        SetPlayerStats();
        EnterState(PlayerStateEnum.Unlocked);
    }
    public override string ToString() { return $"Player ID: {_myID}"; }

    #region PlayerStateMachine
    public void LockPlayer()
    {
        Debug.Log($"Unlocking Player{_myID}");
        EnterState(PlayerStateEnum.Locked);
    }
    public void UnlockPlayer()
    {
        Debug.Log($"Locking Player{_myID}");
        EnterState(PlayerStateEnum.Unlocked);
    }
    public void LimpPlayer()
    {
        Debug.Log($"Limping Player{_myID}");
        EnterState(PlayerStateEnum.Limp);
    }
    public void DownPlayer()
    {
        Debug.Log($"Locking Player{_myID}");
        EnterState(PlayerStateEnum.Downed);
    }
    private void EnterState(PlayerStateEnum enterState)
    {
        Debug.Log($"{name} entering {enterState}");
        playerStateEnum = enterState;
        
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

    public void AssignID()
    {
        _myID = PlayerNetworkController.LocalInstance.ConnectionID;
    }

}

using MessengerSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class PlayerBase : MonoBehaviour
{
    private static int _staticClassID = 1;
    private int _myID;

    #region enums
    public enum PlayerStateEnum
    {
        Locked,
        Unlocked,
        Limp,
        Downed
    }
    public enum PlayerActionEnum
    {
        None = 0,               // 00000
        Idling = 1 << 0,        // 00001
        Running = 1 << 1,       // 00010
        Jumping = 1 << 2,       // 00100
        Interacting = 1 << 3,   // 01000
    }

    #endregion



    #region delegates

    public delegate void PlayerSpawn(PlayerBase player);
    public static event PlayerSpawn OnPlayerSpawn;

    #endregion



    #region Scriptable Object References

    [SerializeField] private SO_PlayerStats lockedStats;
    [SerializeField] private SO_PlayerStats unlockedStats;
    [SerializeField] private SO_PlayerStats limpStats;
    [SerializeField] private SO_PlayerStats downedStats;

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

    float deltaCycleTimer = 5f;
    float cycleTimer = 5f;

    //Animator anim;
    protected virtual void Awake()
    {
        AssignID();
        SetPlayerStats();
        EnterState(PlayerStateEnum.Unlocked);
    }

    protected void TickCycleTimer()
    {
        deltaCycleTimer -= Time.deltaTime;
        if (deltaCycleTimer< 0f)
        {
            deltaCycleTimer = cycleTimer;
            int nextState = ((int)playerStateEnum + 1)%4;
            EnterState((PlayerStateEnum)nextState);

        }
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

    private bool AssignID()
    {
        if (_staticClassID > 4)
        {
            Debug.LogError($"Invalid Player Amount: {_staticClassID}");
            Destroy(this.gameObject);
            return false;
        }
        _myID = _staticClassID;
        ++_staticClassID;
        Debug.Log($"Player {_myID} spawned");

        OnPlayerSpawn?.Invoke(this);

        return true;
    }


    public int ID() { return _myID; }


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

    public override string ToString()
    {
        return $"Player ID: {_myID}";
    }
}

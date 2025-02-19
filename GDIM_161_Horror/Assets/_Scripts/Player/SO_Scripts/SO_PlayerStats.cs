using UnityEngine;

[CreateAssetMenu(fileName = "SO_PlayerStats", menuName = "Scriptable Objects/SO_PlayerStats")]
public class SO_PlayerStats : ScriptableObject
{
    [Header("Player, in m/s")]
    [SerializeField]
    float moveSpeed;
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }

    [SerializeField]
    float sprintSpeed;
    public float SprintSpeed { get => sprintSpeed; private set => sprintSpeed = value; }

    [SerializeField]
    float rotationSpeed;
    public float RotationSpeed { get => rotationSpeed; private set => rotationSpeed = value; }

    [SerializeField]
    float accelerationRate;
    public float AccelerationRate { get => accelerationRate; private set => accelerationRate = value; }

    [SerializeField]
    float decelerationRate;
    public float DecelerationRate { get => decelerationRate; private set => decelerationRate = value; }


    [Space(10)]
    [SerializeField]
    float jumpHeight;
    public float JumpHeight { get => jumpHeight; private set => jumpHeight = value; }

    [SerializeField]
    float gravity;
    public float Gravity { get => gravity; private set => gravity = value; }


    [Space(10)]
    [SerializeField]
    float jumpTimeout;
    public float JumpTimeout { get => jumpTimeout; private set => jumpTimeout = value; }

    [SerializeField]
    float fallTimeout;
    public float FallTimeout { get => fallTimeout; private set => fallTimeout = value; }


    [Header("Player Grounded")]
    [SerializeField]
    float groundedOffset;
    public float GroundedOffset { get => groundedOffset; private set => groundedOffset = value; }

    [SerializeField]
    float groundedRadius;
    public float GroundedRadius { get => groundedRadius; private set => groundedRadius = value; }

    [SerializeField]
    LayerMask groundLayers;
    public LayerMask GroundLayers { get => groundLayers; private set => groundLayers = value; }

    [Header("Cinemachine")]
 
    [SerializeField]
    float topClamp;
    public float TopClamp { get => topClamp; private set => topClamp = value; }

    [SerializeField]
    float bottomClamp;
    public float BottomClamp { get => bottomClamp; private set => bottomClamp = value; }

}

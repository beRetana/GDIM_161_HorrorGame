using UnityEngine;
using System;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using Mirror;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : NetworkBehaviour
	{
		#region VARIABLES

		[Header("Player, in m/s")]
		[SerializeField] float MoveSpeed = 4.0f;
        [SerializeField] float SprintSpeed = 6.0f;
		[SerializeField] float RotationSpeed = 1.0f;
		[SerializeField] float AccelerationRate = 10.0f;
        [SerializeField] float DecelerationRate = 10.0f;

		[Space(10)]
        [SerializeField] float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        [SerializeField] float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        [SerializeField] float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        [SerializeField] float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		public bool Grounded { get; private set; } = true;
		[Tooltip("Useful for rough ground")]
        [SerializeField] float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        [SerializeField] float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		[SerializeField] LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        [SerializeField] GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
        [SerializeField] float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
        [SerializeField] float BottomClamp = -90.0f;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

        #endregion

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;
		
		/// EDITOR ONLY!!!!
		private bool IsCurrentDeviceMouse
		{
			get{
				#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		

		private void Awake()
		{
			if (_mainCamera == null) _mainCamera = GameObject.FindGameObjectWithTag("MainCamera"); // get a reference to our main camera
        }

        private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

		private void Update()
		{
			JumpAndGravity();
			GroundedCheck();
			Move();
		}

		private void LateUpdate()
		{
			CameraRotation();
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		

		private void Move()
		{
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;	// a reference to the players current horizontal velocity

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            #region acceleration
            if (currentHorizontalSpeed < (targetSpeed - speedOffset)) // accelerate
			{
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * AccelerationRate); // curved result (not linear) for organic speed change
				_speed = Mathf.Round(_speed * 1000f) / 1000f; // round speed to 3 decimal places
			}
			else if (currentHorizontalSpeed > (targetSpeed + speedOffset)) // decelerate
			{
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * DecelerationRate); // curved result (not linear) for organic speed change
				_speed = Mathf.Round(_speed * 1000f) / 1000f; // round speed to 3 decimal places
			}
			else
			{
				_speed = targetSpeed;
			}
            #endregion

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;	// normalise input direction


            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y; // move

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				_fallTimeoutDelta = FallTimeout; // reset the fall timeout timer

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
					_verticalVelocity = -2f;

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity); // the square root of H * -2 * G = how much velocity needed to reach desired height

				if (_jumpTimeoutDelta >= 0.0f)
					_jumpTimeoutDelta -= Time.deltaTime;
			}
			else
			{
				_jumpTimeoutDelta = JumpTimeout;

				if (_fallTimeoutDelta >= 0.0f)
					_fallTimeoutDelta -= Time.deltaTime;

				_input.jump = false; // if we are not grounded, do not jump
            }

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
				//if (_verticalVelocity < -10.0f)
			}
		}

        #region camera
        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude < _threshold) return; // check if there is an input

            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;	//Don't multiply mouse input by Time.deltaTime

            _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp); // clamp pitch rotation
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f); // update Cinemachine target pitch

            transform.Rotate(Vector3.up * _rotationVelocity); // rotate player left and right
        }
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

        #endregion

        private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}
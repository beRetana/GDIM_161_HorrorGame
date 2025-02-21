using UnityEngine;
using Mirror;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

	


namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
	#if ENABLE_INPUT_SYSTEM
		[RequireComponent(typeof(PlayerInput))]
	#endif
	public class FirstPersonController : PlayerBase
	{
		

		public bool grounded { get; private set; }
		

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

        protected override void Start()
		{	
			base.Start();

			

			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();

			#if ENABLE_INPUT_SYSTEM
				_playerInput = GetComponent<PlayerInput>();
			#else
						Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
			#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = jumpTimeout;
			_fallTimeoutDelta = fallTimeout;
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
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
			grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			if (_input.look.sqrMagnitude < _threshold) return; // check if there is an input
            
			float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;	//Don't multiply mouse input by Time.deltaTime

            _cinemachineTargetPitch += _input.look.y * rotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = _input.look.x * rotationSpeed * deltaTimeMultiplier;

            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp); // clamp pitch rotation
            cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f); // update Cinemachine target pitch

            transform.Rotate(Vector3.up * _rotationVelocity); // rotate player left and right


            Vector3 currentArmRotation = _arms.transform.localRotation.eulerAngles;
            float armPitch = Mathf.LerpAngle(currentArmRotation.x, _cinemachineTargetPitch * 0.8f, Time.deltaTime * 10f); // Interpolate arm rotation for smoothness
            _arms.transform.localRotation = Quaternion.Euler(armPitch, currentArmRotation.y, currentArmRotation.z);

            //_arms.ArmsRotation(_input.look.y, IsCurrentDeviceMouse);
            //_arms.ArmsRotation(_rotationVelocity, _cinemachineTargetPitch);
        }

        private void Move()
		{
			float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;	// a reference to the players current horizontal velocity

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < (targetSpeed - speedOffset)) // accelerate
			{
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * accelerationRate); // curved result (not linear) for organic speed change
				_speed = Mathf.Round(_speed * 1000f) / 1000f; // round speed to 3 decimal places
			}

			else if (currentHorizontalSpeed > (targetSpeed + speedOffset)) // decelerate
			{
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * decelerationRate); // curved result (not linear) for organic speed change
				_speed = Mathf.Round(_speed * 1000f) / 1000f; // round speed to 3 decimal places
			}

			else
			{
				_speed = targetSpeed;
			}

			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;	// normalise input direction


            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y; // move

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (grounded)
			{
				_fallTimeoutDelta = fallTimeout; // reset the fall timeout timer

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
					_verticalVelocity = -2f;

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
					_verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity); // the square root of H * -2 * G = how much velocity needed to reach desired height

				if (_jumpTimeoutDelta >= 0.0f)
					_jumpTimeoutDelta -= Time.deltaTime;
			}
			else
			{
				_jumpTimeoutDelta = jumpTimeout;

				if (_fallTimeoutDelta >= 0.0f)
					_fallTimeoutDelta -= Time.deltaTime;

				_input.jump = false; // if we are not grounded, do not jump
            }

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = grounded ? new Color(0.0f, 1.0f, 0.0f, 0.35f) : new Color(1.0f, 0.0f, 0.0f, 0.35f); // green : red

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
		}
	}
}
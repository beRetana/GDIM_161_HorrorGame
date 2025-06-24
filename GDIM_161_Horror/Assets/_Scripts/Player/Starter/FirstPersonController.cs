using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class FirstPersonController : PlayerBase
    {
        private StarterAssetsInputs _input;
        private PlayerHeadBobbing _headBobbing;
        private const float _THRESHOLD = 0.01f;
        public bool isWalking { get; private set; }

        float _stepSoundTime;

        [SerializeField] private string _buildScene = "BUILD_1";

        public bool grounded { get; private set; }

        /// EDITOR ONLY!!!!
        private bool IsCurrentDeviceMouse
        {
            get
            {
                return _playerInput.currentControlScheme == "KeyboardMouse";
            }
        }
        private bool _gravityOn = true;

        public bool GravityOn { get => _gravityOn; set => _gravityOn = value; }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected override void Start()
        {   
            base.Start();
            _input = GetComponent<StarterAssetsInputs>();
            _headBobbing = GetComponent<PlayerHeadBobbing>();
            _playerInput = GetComponent<PlayerInput>();

            // Reset timeouts on start
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != _buildScene) return;
            //Cursor.lockState = CursorLockMode.Locked;
            StartCoroutine(FindSpawnPoint());
        }

        private IEnumerator FindSpawnPoint()
        {
            yield return null;

            PlayerSpawnPosition[] spawnPoints = FindObjectsByType<PlayerSpawnPosition>(FindObjectsSortMode.None);
            
            foreach (PlayerSpawnPosition spawnPoint in spawnPoints)
            {
                if (spawnPoint.IsOccupied) continue;
                transform.position = spawnPoint.UseSpawner();
                break;
            }
        }

        private void Update()
        {   
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void FixedUpdate()
        {
            UpdateSpeedAnimation();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void UpdateSpeedAnimation()
        {
            _animator.SetAnimSpeed(new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude);
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
            grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude < _THRESHOLD) return;

            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetPitch += _input.look.y * rotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = _input.look.x * rotationSpeed * deltaTimeMultiplier;

            Debugger($"X Rotation Velovity: {_input.look.x} * {rotationSpeed} * {deltaTimeMultiplier}");

            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);
            cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0f);

            transform.Rotate(Vector3.up * _rotationVelocity);

            //Vector3 currentArmRotation = _arms.transform.localRotation.eulerAngles;
            //float armPitch = Mathf.LerpAngle(currentArmRotation.x, _cinemachineTargetPitch * 0.8f, Time.deltaTime * 10f);
            //_arms.transform.localRotation = Quaternion.Euler(armPitch, currentArmRotation.y, currentArmRotation.z);
        }

        //public void PlayFootstep()
        //{
        //    RuntimeManager.PlayOneShot(_forestFootstep, transform.position);
        //}

        private void Move()
        {
            _stepSoundTime += Time.deltaTime;

            float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;

            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
                isWalking = false;
            }
            else
            {
                isWalking = true; 
            }

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < (targetSpeed - speedOffset))
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * accelerationRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else if (currentHorizontalSpeed > (targetSpeed + speedOffset))
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * decelerationRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else _speed = targetSpeed;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero)
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;

            _headBobbing.SetNoise(_speed / sprintSpeed);
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (!_gravityOn) return;

            if (grounded)
            {
                _fallTimeoutDelta = fallTimeout;

                if (_verticalVelocity < 0.0f)
                    _verticalVelocity = -2f;

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                if (_jumpTimeoutDelta >= 0.0f)
                    _jumpTimeoutDelta -= Time.deltaTime;

            }
            else
            {
                _jumpTimeoutDelta = jumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                    _fallTimeoutDelta -= Time.deltaTime;

                _input.jump = false;
                _animator.SetAnimJump(false);
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += gravity * Time.deltaTime;
                _animator.SetAnimJump(true);
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
            Gizmos.color = grounded ? new Color(0.0f, 1.0f, 0.0f, 0.35f) : new Color(1.0f, 0.0f, 0.0f, 0.35f);
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
        }
    }
}
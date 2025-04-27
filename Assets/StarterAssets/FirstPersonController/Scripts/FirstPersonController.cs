using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player Movement")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;
        [Tooltip("How fast the character slows down")]
        public float Deceleration = 10.0f;
        [Tooltip("How fast the character speeds up")]
        public float Acceleration = 15.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;
        [Tooltip("Add small head bobbing effect when moving")]
        public bool EnableHeadBob = true;
        [Tooltip("Intensity of head bobbing effect")]
        [Range(0, 0.1f)]
        public float HeadBobAmount = 0.05f;
        [Tooltip("Speed of head bobbing effect")]
        public float HeadBobSpeed = 14.0f;

        [Header("Jump Settings")]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;
        [Tooltip("Allow double jumping")]
        public bool AllowDoubleJump = false;
        [Tooltip("Time required to pass before being able to jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state")]
        public float FallTimeout = 0.15f;
        [Tooltip("Apply a small force when landing to enhance feel")]
        public bool EnableLandingEffect = true;
        [Tooltip("Force applied when landing")]
        public float LandingForce = 0.15f;

        [Header("Ground Detection")]
        [Tooltip("If the character is grounded or not")]
        public bool Grounded = true;
        [Tooltip("Ground offset for detection")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;
        [Tooltip("Slope angle limit in degrees")]
        [Range(0, 60)]
        public float MaxSlopeAngle = 45f;

        [Header("Camera Settings")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;
        [Tooltip("Camera sensitivity multiplier")]
        [Range(0.1f, 5f)]
        public float CameraSensitivity = 1.0f;
        [Tooltip("Smooth camera rotation")]
        public bool SmoothCameraRotation = true;
        [Tooltip("How much to smooth camera rotation")]
        [Range(1f, 20f)]
        public float CameraSmoothTime = 5f;

        [Header("Footsteps")]
        [Tooltip("Enable footstep sounds")]
        public bool EnableFootsteps = true;
        [Tooltip("Footstep sound effect")]
        public AudioClip[] FootstepSounds;
        [Tooltip("Footstep volume")]
        [Range(0, 1)]
        public float FootstepVolume = 0.5f;
        [Tooltip("Time between footsteps")]
        public float FootstepInterval = 0.5f;

        // Private variables
        private float _cinemachineTargetPitch;
        private float _speed;
        private float _rotationVelocity; // Fixed: Added missing variable
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private Vector3 _lastGroundedPosition;
        private float _footstepTimer;
        private bool _hasDoubleJumped;
        private float _originalHeight;
        private bool _wasGroundedLastFrame;
        private float _headBobTimer;
        private Vector3 _initialCameraPosition;
        private RaycastHit _slopeHit;
        private Vector3 _impact = Vector3.zero;
        private Vector3 _smoothedLookInput;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private AudioSource _audioSource;
        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        // Check if we're on a slope that's too steep
        private bool OnSteepSlope()
        {
            if (!Grounded) return false;
            
            if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, (_controller.height / 2) + 0.5f))
            {
                float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle > MaxSlopeAngle;
            }
            
            return false;
        }

        // Get adjusted movement direction based on slope
        private Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            if (!OnSteepSlope()) return direction;
            
            return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
        }

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            // Add audio source component if needed
            if (EnableFootsteps && !TryGetComponent(out _audioSource))
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.spatialBlend = 1.0f;
                _audioSource.volume = FootstepVolume;
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            _originalHeight = _controller.height;
            _lastGroundedPosition = transform.position;
            _wasGroundedLastFrame = Grounded;
            
            if (CinemachineCameraTarget != null)
            {
                _initialCameraPosition = CinemachineCameraTarget.transform.localPosition;
            }
            
            // Lock cursor to screen center and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            GroundedCheck();
            HandleLanding();
            JumpAndGravity();
            Move();
            
            if (EnableFootsteps)
            {
                HandleFootsteps();
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
            
            if (EnableHeadBob && Grounded && _speed > 0.1f)
            {
                ApplyHeadBob();
            }
            else if (CinemachineCameraTarget != null)
            {
                // Reset camera position when not moving
                CinemachineCameraTarget.transform.localPosition = Vector3.Lerp(
                    CinemachineCameraTarget.transform.localPosition,
                    _initialCameraPosition,
                    Time.deltaTime * HeadBobSpeed
                );
            }
        }

        private void ApplyHeadBob()
        {
            if (CinemachineCameraTarget == null) return;
            
            _headBobTimer += Time.deltaTime * HeadBobSpeed * (_speed / MoveSpeed);
            
            float bobOffsetY = Mathf.Sin(_headBobTimer) * HeadBobAmount;
            float bobOffsetX = Mathf.Cos(_headBobTimer / 2) * HeadBobAmount * 0.5f;
            
            Vector3 targetBobPosition = new Vector3(
                _initialCameraPosition.x + bobOffsetX,
                _initialCameraPosition.y + bobOffsetY,
                _initialCameraPosition.z
            );
            
            CinemachineCameraTarget.transform.localPosition = Vector3.Lerp(
                CinemachineCameraTarget.transform.localPosition,
                targetBobPosition,
                Time.deltaTime * HeadBobSpeed
            );
        }

        private void HandleFootsteps()
        {
            if (!Grounded || _speed < 0.1f)
            {
                _footstepTimer = 0;
                return;
            }

            _footstepTimer += Time.deltaTime * (_speed / MoveSpeed);

            if (_footstepTimer >= FootstepInterval)
            {
                _footstepTimer = 0;
                PlayFootstepSound();
            }
        }

        private void PlayFootstepSound()
        {
            if (_audioSource == null || FootstepSounds == null || FootstepSounds.Length == 0) return;
            
            int index = Random.Range(0, FootstepSounds.Length);
            _audioSource.PlayOneShot(FootstepSounds[index], FootstepVolume);
        }

        private void GroundedCheck()
        {
            _wasGroundedLastFrame = Grounded;
            
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
            
            if (Grounded)
            {
                _lastGroundedPosition = transform.position;
            }
        }

        private void HandleLanding()
        {
            if (Grounded && !_wasGroundedLastFrame && EnableLandingEffect)
            {
                // Apply landing impact force
                float fallDistance = _lastGroundedPosition.y - transform.position.y;
                if (fallDistance > 0.5f)
                {
                    _impact = Vector3.down * Mathf.Clamp(fallDistance * LandingForce, 0, 0.5f);
                    
                    // Play landing sound here if you have one
                }
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                
                // Apply sensitivity adjustment
                Vector2 sensitivityAdjustedLook = _input.look * CameraSensitivity;
                
                if (SmoothCameraRotation)
                {
                    _smoothedLookInput = Vector3.Lerp(_smoothedLookInput, 
                        new Vector3(sensitivityAdjustedLook.x, sensitivityAdjustedLook.y, 0), 
                        Time.deltaTime * CameraSmoothTime);
                    
                    _cinemachineTargetPitch += _smoothedLookInput.y * RotationSpeed * deltaTimeMultiplier;
                    _rotationVelocity = _smoothedLookInput.x * RotationSpeed * deltaTimeMultiplier;
                }
                else
                {
                    _cinemachineTargetPitch += sensitivityAdjustedLook.y * RotationSpeed * deltaTimeMultiplier;
                    _rotationVelocity = sensitivityAdjustedLook.x * RotationSpeed * deltaTimeMultiplier;
                }

                // Clamp the pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Update Cinemachine camera target pitch
                if (CinemachineCameraTarget != null)
                {
                    CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                }

                // Rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            // Set target speed based on sprint and if we're on a steep slope
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            
            if (OnSteepSlope())
            {
                targetSpeed *= 0.5f;
            }

            // Set to 0 if no input
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // Get current horizontal speed
            Vector3 currentHorizontalVelocity = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z);
            float currentHorizontalSpeed = currentHorizontalVelocity.magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // Accelerate or decelerate
            if (currentHorizontalSpeed < targetSpeed - speedOffset)
            {
                // Accelerating
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * Acceleration);
            }
            else if (currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // Decelerating
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * Deceleration);
            }
            else
            {
                _speed = targetSpeed;
            }

            // Round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;

            // Input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // Adjust to player's rotation when moving
            if (_input.move != Vector2.zero)
            {
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            // Adjust movement for slopes
            Vector3 movementDirection = GetSlopeMoveDirection(inputDirection.normalized);
            
            // Calculate move vector
            Vector3 moveVector = movementDirection * (_speed * Time.deltaTime) + 
                                new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;
            
            // Apply impact force (like landing)
            if (_impact.magnitude > 0.2f)
            {
                moveVector += _impact * Time.deltaTime;
                _impact = Vector3.Lerp(_impact, Vector3.zero, Time.deltaTime * 5f);
            }

            // Move the player
            _controller.Move(moveVector);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;
                _hasDoubleJumped = false;

                // Reset vertical velocity when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // Calculate jump velocity
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    _input.jump = false;
                }

                // Jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // Reset jump timeout
                _jumpTimeoutDelta = JumpTimeout;

                // Double jump
                if (AllowDoubleJump && _input.jump && !_hasDoubleJumped)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -1.8f * Gravity);
                    _hasDoubleJumped = true;
                    _input.jump = false;
                }

                // Fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // No jumping while falling
                if (!AllowDoubleJump || _hasDoubleJumped)
                {
                    _input.jump = false;
                }
            }

            // Apply gravity (with terminal velocity)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
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
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
            Color transparentBlue = new Color(0.0f, 0.0f, 1.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // Draw grounded check sphere
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
            
            // Draw slope check ray
            Gizmos.color = transparentBlue;
            Gizmos.DrawRay(transform.position, Vector3.down * ((_controller.height / 2) + 0.5f));
        }
    }
}
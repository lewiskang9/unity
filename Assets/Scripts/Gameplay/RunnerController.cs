using UnityEngine;

namespace SpectrumDrift.Gameplay
{
    public class RunnerController : MonoBehaviour
    {
        [Header("Forward")]
        [SerializeField, Min(0f)] private float baseForwardSpeed = 8f;
        [SerializeField, Min(0f)] private float maxForwardSpeed = 18f;

        [Header("Lateral")]
        [SerializeField, Min(0f)] private float laneWidth = 4.5f;
        [SerializeField, Min(0f)] private float lateralSensitivity = 0.015f;
        [SerializeField, Min(0f)] private float keyboardLateralSpeed = 8f;
        [SerializeField, Min(0f)] private float lateralSmooth = 14f;

        private float _targetX;
        private float _dynamicForwardSpeed;
        private Vector2 _lastPointerPosition;
        private bool _pointerHeld;

        public float CurrentForwardSpeed => _dynamicForwardSpeed;
        public bool IsRunning { get; private set; }

        private void Awake()
        {
            _dynamicForwardSpeed = baseForwardSpeed;
        }

        private void OnEnable()
        {
            StartRun();
        }

        private void Update()
        {
            if (!IsRunning)
            {
                return;
            }

            HandleLateralInput();
            Move();
        }

        public void StartRun()
        {
            IsRunning = true;
        }

        public void StopRun()
        {
            IsRunning = false;
        }

        public void SetForwardSpeedModifier(float additiveSpeed)
        {
            _dynamicForwardSpeed = Mathf.Clamp(baseForwardSpeed + additiveSpeed, 0f, maxForwardSpeed);
        }

        private void HandleLateralInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        _pointerHeld = true;
                        _lastPointerPosition = touch.position;
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (_pointerHeld)
                        {
                            float deltaX = touch.position.x - _lastPointerPosition.x;
                            _targetX += deltaX * lateralSensitivity;
                            _lastPointerPosition = touch.position;
                        }
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        _pointerHeld = false;
                        break;
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                _pointerHeld = true;
                _lastPointerPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0) && _pointerHeld)
            {
                Vector2 currentMouse = Input.mousePosition;
                float deltaX = currentMouse.x - _lastPointerPosition.x;
                _targetX += deltaX * lateralSensitivity;
                _lastPointerPosition = currentMouse;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _pointerHeld = false;
            }
            else
            {
                float axis = Input.GetAxisRaw("Horizontal");
                _targetX += axis * keyboardLateralSpeed * Time.deltaTime;
            }

            _targetX = Mathf.Clamp(_targetX, -laneWidth, laneWidth);
        }

        private void Move()
        {
            Vector3 currentPosition = transform.position;
            float smoothedX = Mathf.Lerp(currentPosition.x, _targetX, Time.deltaTime * lateralSmooth);
            float nextZ = currentPosition.z + (_dynamicForwardSpeed * Time.deltaTime);
            transform.position = new Vector3(smoothedX, currentPosition.y, nextZ);
        }
    }
}

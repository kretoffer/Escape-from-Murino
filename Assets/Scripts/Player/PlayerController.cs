using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _walkSpeed = 5;
        [SerializeField] private float _runSpeed = 8;
        [SerializeField] private float _stealSpeed = 3;

        [SerializeField] private float _jumpForce = 5;
        [SerializeField] private float _rotateSpeed = 75;
        [SerializeField] private float gravity = -9.81f;

        private CharacterController _characterController;
        private Camera _playerCamera;

        private Vector3 _velocity;
        private Vector2 _rotation;


        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _playerCamera = GetComponentInChildren<Camera>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            // ROTATE
            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            mouseDelta *= _rotateSpeed * Time.deltaTime;
            _rotation.y += mouseDelta.x;
            _rotation.x = Mathf.Clamp(_rotation.x - mouseDelta.y, -90, 90);
            _playerCamera.transform.localEulerAngles = _rotation;

            // MOVE
            Vector2 _direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (Input.GetButton("Fire3"))
                _direction *= _runSpeed;
            else if (Input.GetButton("Fire1"))
                _direction *= _stealSpeed;
            else
                _direction *= _walkSpeed;

            if (Input.GetButton("Fire1"))
                _characterController.height = 1f;
            else
                _characterController.height = 2f;

            Vector3 move = Quaternion.Euler(0, _playerCamera.transform.eulerAngles.y, 0) 
                        * new Vector3(_direction.x, 0, _direction.y);

            _velocity = new Vector3(move.x, _velocity.y, move.z);
            _characterController.Move(_velocity * Time.deltaTime);

            // JUMP
            if (_characterController.isGrounded)
            {
                _velocity.y = -0.1f;
                if (Input.GetButton("Jump"))
                    _velocity.y = _jumpForce;
            }
            else
            {
                _velocity.y += gravity * Time.deltaTime;
            }

        }
    }
}
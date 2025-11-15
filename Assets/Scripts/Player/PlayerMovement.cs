using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Speeds")]
        [SerializeField] private float _walkSpeed = 5;
        [SerializeField] private float _runSpeed = 8;
        [SerializeField] private float _stealSpeed = 3;

        [Header("------")]
        [SerializeField] private float _jumpForce = 5;
        [SerializeField] private float gravity = -9.81f;

        private CharacterController _characterController;
        private Vector3 _velocity;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            float currentSpeed = _walkSpeed;
            if (Input.GetButton("Fire3"))
                currentSpeed = _runSpeed;
            else if (Input.GetButton("Fire1"))
                currentSpeed = _stealSpeed;

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 move = transform.right * horizontal + transform.forward * vertical;

            Vector3 horizontalVelocity = move * currentSpeed;

            if (_characterController.isGrounded)
            {
                _velocity.y = -0.1f;
                if (Input.GetButton("Jump"))
                {
                    _velocity.y = _jumpForce;
                }
            }
            else
            {
                _velocity.y += gravity * Time.deltaTime;
            }

            Vector3 finalVelocity = new Vector3(horizontalVelocity.x, _velocity.y, horizontalVelocity.z);
            _characterController.Move(finalVelocity * Time.deltaTime);

            if (Input.GetButton("Fire1"))
                _characterController.height = 1f;
            else
                _characterController.height = 2f;
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
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
    private PlayerStats _playerStats;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        bool isRunning = Input.GetButton("Run");
        bool isCrouching = Input.GetButton("Steal");

        float currentSpeed = _walkSpeed;
        if (isRunning)
            currentSpeed = _runSpeed;
        else if (isCrouching)
            currentSpeed = _stealSpeed;

        _playerStats.SetMovementState(isRunning, isCrouching);

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
                _playerStats.OnJump();
            }
        }
        else
        {
            _velocity.y += gravity * Time.deltaTime;
        }

        Vector3 finalVelocity = new Vector3(horizontalVelocity.x, _velocity.y, horizontalVelocity.z);
        _characterController.Move(finalVelocity * Time.deltaTime);

        if (isCrouching)
            _characterController.height = 1f;
        else
            _characterController.height = 2f;
    }
}

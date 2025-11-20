using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 1500;
    private Transform _cameraTransform;

    private bool _isActive = false;

    private float _rotationX = 0f;

    void Start()
    {
        _cameraTransform = GetComponentInChildren<Camera>().transform;
        Activate();
    }

    void Update()
    {
        if (!_isActive) return;
        float mouseX = Input.GetAxis("Mouse X") * _rotateSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _rotateSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
        _cameraTransform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
    }

    public void Activate()
    {
        _isActive = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Deactivate()
    {
        _isActive = false;
        Cursor.lockState = CursorLockMode.None;
    }
}

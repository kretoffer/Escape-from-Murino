using UnityEngine;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _rotateSpeed = 1500;
        private Transform _cameraTransform;

        private float _rotationX = 0f;

        void Start()
        {
            _cameraTransform = GetComponentInChildren<Camera>().transform;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            float mouseX = Input.GetAxis("Mouse X") * _rotateSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * _rotateSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up * mouseX);

            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
            _cameraTransform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
        }
    }
}

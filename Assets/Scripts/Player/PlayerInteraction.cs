using UnityEngine;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float _interactionDistance = 5f;
        private Transform _cameraTransform;
        private HandInventory _handInventory;

        private void Start()
        {
            _cameraTransform = GetComponentInChildren<Camera>().transform;
            _handInventory = GetComponent<HandInventory>();
            if (_cameraTransform == null)
            {
                Debug.LogError("Camera transform not found on player object or its children. Make sure there is a child object with a Camera component.");
                enabled = false;
            }
        }

        private void Update()
        {
            Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance))
            {
                // Handle IInteractive
                if (hit.collider.TryGetComponent(out IInteractive interactiveObject))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        interactiveObject.Interact();
                    }
                }

                // Handle IPicked
                if (hit.collider.TryGetComponent(out IPicked pickedObject))
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        if (_handInventory.TryEquip(pickedObject.item))
                        {
                            pickedObject.Pick();
                        }
                    }
                }
            }
        }
    }
}

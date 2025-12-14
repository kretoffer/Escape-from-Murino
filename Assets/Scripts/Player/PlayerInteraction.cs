using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float _interactionDistance = 5f;
    [SerializeField] private CluesController _cluesController;

    private Transform _cameraTransform;
    private HandInventory _handInventory;
    private Memories _memories;
    private GameObject _focusedGameObject;
    private readonly List<Clue> _activeClues = new List<Clue>();

    [SerializeField] private Sprite interactClue;
    [SerializeField] private Sprite pickClue;
    [SerializeField] private Sprite memotyClue;

    private void Start()
    {
        _cameraTransform = GetComponentInChildren<Camera>().transform;
        _handInventory = GetComponent<HandInventory>();
        _memories = GetComponent<Memories>();
        if (_cameraTransform == null)
        {
            Debug.LogError("Camera transform not found on player object or its children. Make sure there is a child object with a Camera component.");
            enabled = false;
        }

        if (_cluesController == null)
        {
            Debug.LogError("Clues Controller is not set in the inspector. Please assign it.");
            enabled = false;
        }
    }

    private void Update()
    {
        Ray ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
        GameObject currentFocusedObject = null;

        if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance))
        {
            currentFocusedObject = hit.collider.gameObject;

            // Handle IInteractive
            if (hit.collider.TryGetComponent(out IInteractive interactiveObject))
            {
                if (Input.GetKeyDown(KeyCode.C))
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

            // Handle IMemoryable
            if (hit.collider.TryGetComponent(out IMemoryable memoryabledObject))
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    _memories.AddMemory(memoryabledObject.memory);
                    memoryabledObject.Save();
                }
            }
        }

        if (currentFocusedObject != _focusedGameObject)
        {
            _focusedGameObject = currentFocusedObject;

            foreach (var clue in _activeClues)
            {
                _cluesController.Remove(clue);
            }
            _activeClues.Clear();

            if (_focusedGameObject != null)
            {
                if (_focusedGameObject.TryGetComponent<IInteractive>(out IInteractive interactiveObject))
                {
                    _activeClues.Add(_cluesController.Create(interactiveObject.name, interactClue));
                }
                if (_focusedGameObject.TryGetComponent<IPicked>(out _))
                {
                    _activeClues.Add(_cluesController.Create("Pick up", pickClue));
                }
                if (_focusedGameObject.TryGetComponent<IMemoryable>(out _))
                {
                    _activeClues.Add(_cluesController.Create("Remember", memotyClue));
                }
            }
        }
    }
}

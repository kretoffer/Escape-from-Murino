using UnityEngine;
using System.Collections.Generic;
using System;

public class Memories : MonoBehaviour
{
    [SerializeField] private GameObject memoryUI;
    private CameraController _cameraController;

    public List<Memory> memories = new List<Memory>();

    public event Action<Memory> OnMemoryAdded;
    public event Action<Memory> OnMemoryRemoved;

    void Awake()
    {
        _cameraController = GetComponent<CameraController>();
    }

    public void AddMemory(Memory memory)
    {
        if (!memories.Contains(memory))
        {
            memories.Add(memory);
            OnMemoryAdded?.Invoke(memory);
        }
    }

    public void RemoveMemory(Memory memory)
    {
        if (memories.Contains(memory))
        {
            memories.Remove(memory);
            OnMemoryRemoved?.Invoke(memory);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            memoryUI.SetActive(!memoryUI.activeSelf);
            if (memoryUI.activeSelf) _cameraController.Deactivate();
            else _cameraController.Activate();
        }
    }
}
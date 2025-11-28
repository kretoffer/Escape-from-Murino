using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MemoryController : MonoBehaviour
{
    [SerializeField] private Memories memories;
    [SerializeField] private GameObject memoryUIPrefab;
    [SerializeField] private Transform memoryUIParent;
    [SerializeField] private Text memoryFullText;

    private Dictionary<Memory, GameObject> memoryUIItems = new Dictionary<Memory, GameObject>();

    private void Start()
    {
        if (memories != null)
        {
            memories.OnMemoryAdded += HandleMemoryAdded;
            memories.OnMemoryRemoved += HandleMemoryRemoved;
            InitialPopulate();
        }
        else
        {
            Debug.LogError("Memories script not assigned to MemoryController.");
        }

        if (memoryFullText != null)
        {
            memoryFullText.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (memories != null)
        {
            memories.OnMemoryAdded -= HandleMemoryAdded;
            memories.OnMemoryRemoved -= HandleMemoryRemoved;
        }
    }

    private void InitialPopulate()
    {
        if (memories != null)
        {
            foreach (Memory memory in memories.memories)
            {
                HandleMemoryAdded(memory);
            }
        }
    }

    private void HandleMemoryAdded(Memory memory)
    {
        if (memoryUIPrefab != null && memoryUIParent != null)
        {
            GameObject memoryUIObject = Instantiate(memoryUIPrefab, memoryUIParent);
            MemoryUIItem memoryUIItem = memoryUIObject.GetComponent<MemoryUIItem>();
            if (memoryUIItem != null)
            {
                memoryUIItem.Setup(memory, this);
                memoryUIItems[memory] = memoryUIObject;
            }
            else
            {
                Debug.LogError("MemoryUIPrefab does not have a MemoryUIItem component.");
            }
        }
        else
        {
            Debug.LogWarning("MemoryUIPrefab or MemoryUIParent is not assigned in MemoryController.");
        }
    }

    private void HandleMemoryRemoved(Memory memory)
    {
        if (memoryUIItems.TryGetValue(memory, out GameObject memoryUIObject))
        {
            Destroy(memoryUIObject);
            memoryUIItems.Remove(memory);
        }
    }

    public void ShowMemoryText(Memory memory)
    {
        if (memoryFullText != null)
        {
            memoryFullText.gameObject.SetActive(true);
            memoryFullText.text = memory.text;
        }
    }
}
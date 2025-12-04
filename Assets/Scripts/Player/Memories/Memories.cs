using UnityEngine;
using System.Collections.Generic;
using System;

public class Memories : MonoBehaviour
{
    public List<Memory> memories = new List<Memory>();

    public event Action<Memory> OnMemoryAdded;
    public event Action<Memory> OnMemoryRemoved;

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
}
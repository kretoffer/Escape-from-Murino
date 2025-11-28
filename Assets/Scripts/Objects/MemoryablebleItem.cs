using UnityEngine;

public class MemoryableItem : MonoBehaviour, IMemoryable
{
    [field: SerializeField] public Memory memory { get; set; }

    public void Save()
    {
        
    }

    private void Start()
    {
        if (memory == null)
        {
            Debug.LogError("item can't be null");
            Destroy(gameObject);
        }
    }
}

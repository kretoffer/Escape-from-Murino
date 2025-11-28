using UnityEngine;

public class PickableItem : MonoBehaviour, IPicked
{
    [field: SerializeField] public ItemData item { get; set; }

    public void Pick()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        if (item == null)
        {
            Debug.LogError("item can't be null");
            Destroy(gameObject);
        }
    }
}

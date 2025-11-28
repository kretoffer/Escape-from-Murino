using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Visual")]
    public string itemName;
    public Sprite icon;
    public Sprite inventoryTexture;
    public Mesh mesh;
    public GameObject prefab;
    [Header("Settings")]
    public int height = 1;
    public int width = 1;
    public bool isTwoHanded = false;
    public float weight;
}

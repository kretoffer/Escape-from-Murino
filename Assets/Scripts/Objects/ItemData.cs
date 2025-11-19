using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Visual")]
    public string itemName;
    public Sprite icon;
    public Sprite inventoryTexture;
    public Mesh mesh;
    [Header("Settings")]
    public bool isTwoHanded = false;
    public float weight;
}

using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    [Tooltip("Reference to the Inventory script (e.g., on your Player GameObject).")]
    public Inventory inventory;

    [Tooltip("Reference to an ItemData ScriptableObject to add to the inventory.")]
    public ItemData testItem;

    void Update()
    {
        // Press 'T' key to add a test item to the inventory
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (inventory == null)
            {
                Debug.LogError("Inventory reference is not set in InventoryTester.");
                return;
            }
            if (testItem == null)
            {
                Debug.LogError("Test ItemData is not set in InventoryTester.");
                return;
            }

            // Use the new TryAddItem method to find a free spot automatically
            bool added = inventory.TryAddItem(testItem);
            if (added)
            {
                Debug.Log($"Added {testItem.itemName} to inventory.");
            }
            else
            {
                Debug.LogWarning($"Failed to add {testItem.itemName} to inventory. No free space found.");
            }
        }
    }
}

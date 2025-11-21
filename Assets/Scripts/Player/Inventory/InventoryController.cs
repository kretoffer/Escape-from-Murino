using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    private Inventory inventory;

    [Tooltip("The prefab for the visual item in the inventory.")]
    [SerializeField] private GameObject inventoryItemPrefab;

    [Tooltip("The parent transform for the grid items.")]
    [SerializeField] private RectTransform gridContainer;

    [Tooltip("The size of a single grid cell in pixels.")]
    [SerializeField] private float cellSize = 100f;
    public float CellSize => cellSize;

    // Maps the data item to its UI game object representation.
    private Dictionary<InventoryItem, GameObject> itemToGameObjectMap = new Dictionary<InventoryItem, GameObject>();

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            inventory = player.GetComponent<Inventory>();
            if (inventory == null)
            {
                Debug.LogError("Inventory component not found on the Player object.");
            }
        }
        else
        {
            Debug.LogError("Player object not found. Make sure your player has the 'Player' tag.");
        }
    }

    private void OnEnable()
    {
        if (inventory == null)
        {
            // Error is logged in Awake, but we still need to stop execution here.
            return;
        }

        // Subscribe to inventory events
        inventory.OnItemAdded += AddItemUI;
        inventory.OnItemRemoved += RemoveItemUI;
        
        // Initial draw
        RefreshAllItems();
    }

    private void OnDisable()
    {
        if (inventory == null) return;

        inventory.OnItemAdded -= AddItemUI;
        inventory.OnItemRemoved -= RemoveItemUI;

        ClearAllItemsUI();
    }
    private void AddItemUI(InventoryItem item)
    {
        if (inventoryItemPrefab == null || gridContainer == null)
        {
            Debug.LogError("InventoryItemPrefab or GridContainer is not assigned.");
            return;
        }

        GameObject itemGO = Instantiate(inventoryItemPrefab, gridContainer);
        itemToGameObjectMap[item] = itemGO;

        // Setup the draggable component
        DraggableItem draggable = itemGO.GetComponent<DraggableItem>();
        if (draggable != null)
        {
            draggable.Setup(inventory, this, item);
        }

        RectTransform rectTransform = itemGO.GetComponent<RectTransform>();
        // Set anchor to top-left
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);

        rectTransform.anchoredPosition = new Vector2(item.x * cellSize, -item.y * cellSize);
        rectTransform.sizeDelta = new Vector2(item.GetWidth() * cellSize, item.GetHeight() * cellSize);
        draggable.indicator.GetComponent<RectTransform>().sizeDelta = new Vector2(item.GetWidth() * cellSize, item.GetHeight() * cellSize);

        // Set the item's icon
        Image itemIcon = itemGO.GetComponentInChildren<Image>();
        if (itemIcon != null && item.itemData.inventoryTexture != null)
        {
            itemIcon.sprite = item.itemData.inventoryTexture;
        }
    }
    private void RemoveItemUI(InventoryItem item)
    {
        if (itemToGameObjectMap.ContainsKey(item))
        {
            Destroy(itemToGameObjectMap[item]);
            itemToGameObjectMap.Remove(item);
        }
    }
    private void RefreshAllItems()
    {
        ClearAllItemsUI();
        foreach (var item in inventory.GetItems())
        {
            AddItemUI(item);
        }
    }
    private void ClearAllItemsUI()
    {
        foreach (var itemGO in itemToGameObjectMap.Values)
        {
            Destroy(itemGO);
        }
        itemToGameObjectMap.Clear();
    }
}

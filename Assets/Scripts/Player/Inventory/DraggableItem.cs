using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles the drag-and-drop functionality for an inventory item UI element.
/// Must be placed on the inventory item prefab.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Image))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventoryItem inventoryItem { get; private set; }

    private Inventory inventory;
    private InventoryController inventoryController;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    public void Setup(Inventory inv, InventoryController controller, InventoryItem item)
    {
        inventory = inv;
        inventoryController = controller;
        inventoryItem = item;
        GetComponent<Image>().sprite = item.itemData.icon;
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventoryItem == null) return;

        originalParent = rectTransform.parent;
        
        // Visually lift the item
        rectTransform.SetParent(inventoryController.transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inventoryItem == null) return;

        // Follow mouse
        rectTransform.position = eventData.position;

        // Rotate item on 'R' key press
        if (Input.GetKeyDown(KeyCode.R))
        {
            inventoryItem.isRotated = !inventoryItem.isRotated;
            rectTransform.sizeDelta = new Vector2(inventoryItem.GetWidth() * inventoryController.CellSize, inventoryItem.GetHeight() * inventoryController.CellSize);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (inventoryItem == null) return;

        // Get drop position
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)originalParent,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);

        int newX = Mathf.FloorToInt(localPoint.x / inventoryController.CellSize)+5;
        int newY = Mathf.FloorToInt(-localPoint.y / inventoryController.CellSize)+3;

        // Move item in data model
        inventory.MoveItem(inventoryItem, newX, newY);

        // Snap back to grid UI
        rectTransform.SetParent(originalParent);
        rectTransform.anchoredPosition = new Vector2(inventoryItem.x * inventoryController.CellSize, -inventoryItem.y * inventoryController.CellSize);
        
        // Restore visual state
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }
}

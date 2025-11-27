using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private bool isDraged = false;

    [Header("Indicator")]
    [SerializeField] public Image indicator;
    [SerializeField] private Color defaultColor = new Color(0, 0, 0, 0);
    [SerializeField] private Color canColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] private Color cantColor = new Color(1, 0, 0, 0.5f);
    private RectTransform indicatorRectTransform;

    public void Setup(Inventory inv, InventoryController controller, InventoryItem item)
    {
        inventory = inv;
        inventoryController = controller;
        inventoryItem = item;
        GetComponent<Image>().sprite = item.itemData.icon;
        indicator.color = defaultColor;
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        indicatorRectTransform = indicator.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventoryItem == null) return;

        originalParent = rectTransform.parent;
        
        // Visually lift the item
        rectTransform.SetParent(inventoryController.transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;

        isDraged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inventoryItem == null) return;

        // Follow mouse
        rectTransform.position = eventData.position;

        // Check Possible
        if (inventory.IsPlacementToSlot(rectTransform))
        {
            indicator.color = canColor;
            return;
        }
        var pos = GetXY(eventData);
        indicator.color = inventory.IsPlacementPossible(
            pos.x, 
            pos.y, 
            inventoryItem.itemData.width, 
            inventoryItem.itemData.height, 
            inventoryItem) 
        ? canColor : cantColor;
    }

    private void Update()
    {
        // Rotate item on 'R' key press
        if (isDraged && Input.GetKeyDown(KeyCode.R))
        {
            inventoryItem.isRotated = !inventoryItem.isRotated;
            rectTransform.sizeDelta = new Vector2(inventoryItem.GetWidth() * inventoryController.CellSize, inventoryItem.GetHeight() * inventoryController.CellSize);
            indicatorRectTransform.sizeDelta = new Vector2(inventoryItem.GetWidth() * inventoryController.CellSize, inventoryItem.GetHeight() * inventoryController.CellSize);
        }
    }

    private (int x, int y) GetXY(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)originalParent,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);

        RectTransform parentRect = (RectTransform)originalParent;
        int deltaX = Mathf.FloorToInt(parentRect.rect.width/2);
        int deltaY = Mathf.FloorToInt(parentRect.rect.height/2);
        int newX = Mathf.FloorToInt((localPoint.x + deltaX) / inventoryController.CellSize);
        int newY = Mathf.FloorToInt((-localPoint.y + deltaY) / inventoryController.CellSize);

        return (newX, newY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (inventoryItem == null) return;
        // Restore visual state
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        indicator.color = defaultColor;

        isDraged = false;

        if (inventory.IsPlacementToSlot(rectTransform))
        {
            if (inventory.EquipFromInventoryToHand(inventoryItem, rectTransform))
            {
                // Item was successfully equipped. The InventoryController will handle destroying the UI object
                // via the OnItemRemoved event. We don't need to do anything else here.
                return;
            }
        }

        // If not placed in a slot, or if equipping failed, move it within the inventory.
        var pos = GetXY(eventData);

        // Move item in data model
        inventory.MoveItem(inventoryItem, pos.x, pos.y);

        // Snap back to grid UI
        rectTransform.SetParent(originalParent);
        rectTransform.anchoredPosition = new Vector2(inventoryItem.x * inventoryController.CellSize, -inventoryItem.y * inventoryController.CellSize);
    }
}

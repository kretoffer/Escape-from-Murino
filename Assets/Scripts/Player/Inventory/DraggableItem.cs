using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Image))]
public class DraggableItem : AbstractDraggableItem
{
    public InventoryItem inventoryItem { get; private set; }

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    [Header("Indicator")]
    [SerializeField] public Image indicator;
    private RectTransform indicatorRectTransform;

    // --- Abstract property implementations ---
    protected override Image Indicator => indicator;
    protected override RectTransform DraggableRectTransform => rectTransform;
    protected override RectTransform IndicatorRectTransform => indicatorRectTransform;
    protected override RectTransform GridContainer => (RectTransform)originalParent;
    protected override int ItemWidth => inventoryItem.GetWidth();
    protected override int ItemHeight => inventoryItem.GetHeight();
    protected override InventoryItem ItemToIgnore => inventoryItem;
    protected override void Rotate()
    {
        inventoryItem.isRotated = !inventoryItem.isRotated;
    }

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

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (inventoryItem == null) return;

        originalParent = rectTransform.parent;
        
        // Visually lift the item
        rectTransform.SetParent(inventoryController.transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;

        isDragged = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (inventoryItem == null) return;
        // Restore visual state
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        indicator.color = defaultColor;

        isDragged = false;

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

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public abstract class AbstractDraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // --- CONFIGURATION ---
    [Header("Indicator Colors")]
    [SerializeField] protected Color defaultColor = new Color(0, 0, 0, 0);
    [SerializeField] protected Color canColor = new Color(0, 1, 0, 0.5f);
    [SerializeField] protected Color cantColor = new Color(1, 0, 0, 0.5f);

    // --- PROTECTED STATE ---
    protected Inventory inventory;
    protected InventoryController inventoryController;
    protected bool isDragged = false;

    // --- ABSTRACT MEMBERS ---
    protected abstract Image Indicator { get; }
    protected abstract RectTransform DraggableRectTransform { get; }
    protected abstract RectTransform IndicatorRectTransform { get; }
    protected abstract RectTransform GridContainer { get; }
    protected abstract int ItemWidth { get; }
    protected abstract int ItemHeight { get; }
    protected abstract InventoryItem ItemToIgnore { get; }
    protected abstract void Rotate();

    // --- INTERFACE IMPLEMENTATION ---
    public abstract void OnBeginDrag(PointerEventData eventData);
    public abstract void OnEndDrag(PointerEventData eventData);

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!isDragged) return;

        DraggableRectTransform.position = eventData.position;

        if (Indicator == null) return;

        if (inventory.IsPlacementToSlot(DraggableRectTransform))
        {
            Indicator.color = canColor;
            return;
        }

        var pos = GetXY(eventData);
        Indicator.color = inventory.IsPlacementPossible(pos.x, pos.y, ItemWidth, ItemHeight, ItemToIgnore) ? canColor : cantColor;
    }

    // --- VIRTUAL METHODS ---
    protected virtual void Update()
    {
        if (isDragged && Input.GetKeyDown(KeyCode.R))
        {
            Rotate();
            DraggableRectTransform.sizeDelta = new Vector2(ItemWidth * inventoryController.CellSize, ItemHeight * inventoryController.CellSize);
            if (IndicatorRectTransform != null)
            {
                IndicatorRectTransform.sizeDelta = DraggableRectTransform.sizeDelta;
            }
        }
    }

    // --- HELPER METHODS ---
    protected (int x, int y) GetXY(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GridContainer,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);

        RectTransform parentRect = GridContainer;
        float deltaX = parentRect.rect.width / 2;
        float deltaY = parentRect.rect.height / 2;
        int newX = Mathf.FloorToInt((localPoint.x + deltaX) / inventoryController.CellSize);
        int newY = Mathf.FloorToInt((-localPoint.y + deltaY) / inventoryController.CellSize);

        return (newX, newY);
    }
}

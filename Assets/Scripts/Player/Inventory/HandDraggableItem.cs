using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HandDraggableItem : AbstractDraggableItem
{
    public Hand hand;
    public bool isPocket;

    private HandInventory handInventory;

    private ItemData currentItemData;
    private HandSlot currentSlot;
    private Image slotImage;

    private GameObject draggedItemGO;
    private RectTransform draggedItemRectTransform;
    private CanvasGroup draggedItemCanvasGroup;

    private Image indicatorOnDraggedItem;
    private RectTransform indicatorRectTransformOnDraggedItem;
    private bool isRotated = false;
    private bool isInitialized = false;

    // --- Abstract property implementations ---
    protected override Image Indicator => indicatorOnDraggedItem;
    protected override RectTransform DraggableRectTransform => draggedItemRectTransform;
    protected override RectTransform IndicatorRectTransform => indicatorRectTransformOnDraggedItem;
    protected override RectTransform GridContainer => inventoryController.GridContainer;
    protected override int ItemWidth => isRotated ? currentItemData.height : currentItemData.width;
    protected override int ItemHeight => isRotated ? currentItemData.width : currentItemData.height;
    protected override InventoryItem ItemToIgnore => null;
    protected override void Rotate()
    {
        isRotated = !isRotated;
    }

    void Start()
    {
        Initialize();
    }

    private bool Initialize()
    {
        if (isInitialized) return true;

        slotImage = GetComponent<Image>();
        inventoryController = FindObjectOfType<InventoryController>();
        if (inventoryController == null)
        {
            Debug.LogWarning("InventoryController not found in scene. Will try again on drag.");
            return false;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            handInventory = player.GetComponent<HandInventory>();
            inventory = player.GetComponent<Inventory>();
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found in scene.");
            return false;
        }

        if (isPocket)
        {
            currentSlot = (hand == Hand.Left) ? handInventory.leftPocketSlot : handInventory.rightPocketSlot;
        }
        else
        {
            currentSlot = (hand == Hand.Left) ? handInventory.leftHandSlot : handInventory.rightHandSlot;
        }
        
        if (currentSlot == null)
        {
            Debug.LogError("Current slot is null. Hand/Pocket slots might not be initialized in HandInventory.");
            return false;
        }

        isInitialized = true;
        return true;
    }

    public override void DropItem()
    {
        inventory.DropItemFromHand(hand, isPocket);
        slotImage.enabled = true;
        Destroy(draggedItemGO);
        isDragged = false;
        draggedItemGO = null;
        draggedItemRectTransform = null;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInitialized)
        {
            if (!Initialize())
            {
                Debug.LogError("Failed to initialize HandDraggableItem on drag.");
                return;
            }
        }
        
        currentItemData = currentSlot.currentItem;

        if (currentItemData == null) return;

        if (inventoryController.InventoryItemPrefab == null)
        {
            Debug.LogError("InventoryItemPrefab is not set in the InventoryController. Please assign it in the Inspector.");
            return;
        }

        draggedItemGO = Instantiate(inventoryController.InventoryItemPrefab, inventoryController.transform.root);
        draggedItemRectTransform = draggedItemGO.GetComponent<RectTransform>();
        
        DraggableItem originalDraggable = draggedItemGO.GetComponent<DraggableItem>();
        if(originalDraggable != null)
        {
            originalDraggable.enabled = false;
            indicatorOnDraggedItem = originalDraggable.indicator;
        }

        Image draggedImage = draggedItemGO.GetComponent<Image>();
        if (draggedImage != null)
        {
            draggedImage.sprite = currentItemData.inventoryTexture;
        }

        draggedItemCanvasGroup = draggedItemGO.GetComponent<CanvasGroup>();
        draggedItemCanvasGroup.blocksRaycasts = false;
        draggedItemCanvasGroup.alpha = 0.7f;

        draggedItemRectTransform.sizeDelta = new Vector2(currentItemData.width * inventoryController.CellSize, currentItemData.height * inventoryController.CellSize);

        if (indicatorOnDraggedItem != null)
        {
            indicatorRectTransformOnDraggedItem = indicatorOnDraggedItem.GetComponent<RectTransform>();
            indicatorRectTransformOnDraggedItem.sizeDelta = draggedItemRectTransform.sizeDelta;
            indicatorOnDraggedItem.color = defaultColor;
        }

        slotImage.enabled = false;
        isRotated = false;
        isDragged = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItemGO == null) { slotImage.enabled = true; return; }

        isDragged = false;

        RectTransform draggedRT = draggedItemGO.GetComponent<RectTransform>();
        bool placedInSlot = false;

        var targets = new[] {
            new { slot = handInventory.rightHandSlot, rt = handInventory._rightHandInventory.GetComponent<RectTransform>(), type = "Hand", hand = Hand.Right },
            new { slot = handInventory.leftHandSlot, rt = handInventory._leftHandInventory.GetComponent<RectTransform>(), type = "Hand", hand = Hand.Left },
            new { slot = handInventory.rightPocketSlot, rt = handInventory._rightPocket.GetComponent<RectTransform>(), type = "Pocket", hand = Hand.Right },
            new { slot = handInventory.leftPocketSlot, rt = handInventory._leftPocket.GetComponent<RectTransform>(), type = "Pocket", hand = Hand.Left }
        };

        foreach (var target in targets)
        {
            if (currentSlot != target.slot && target.slot.IsEmpty() && UIHelper.IsPartialOverlap(draggedRT, target.rt))
            {
                ItemData itemToMove = isPocket ? handInventory.UnequipFromPocket(this.hand) : handInventory.UnequipFromHand(this.hand);

                if (itemToMove != null)
                {
                    if (target.type == "Hand") handInventory.TryEquipToHand(itemToMove, target.hand);
                    else handInventory.TryEquipToPocket(itemToMove, target.hand);
                    
                    placedInSlot = true;
                    break; 
                }
            }
        }

        if (!placedInSlot)
        {
            var pos = GetXY(eventData);
            InventoryItem newItem = new InventoryItem(currentItemData, pos.x, pos.y) { isRotated = this.isRotated };

            if (inventory.AddItem(newItem))
            {
                if (isPocket) handInventory.UnequipFromPocket(this.hand);
                else handInventory.UnequipFromHand(this.hand);
            }
        }

        slotImage.enabled = true;
        Destroy(draggedItemGO);
        draggedItemGO = null;
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HandDraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Hand hand;
    public bool isPocket;

    private HandInventory handInventory;
    private Inventory inventory;
    private InventoryController inventoryController;

    private ItemData currentItemData;
    private HandSlot currentSlot;
    private Image slotImage;

    private GameObject draggedItemGO;
    private RectTransform gridContainer;
    private CanvasGroup draggedItemCanvasGroup;

    private Image indicator;
    private Color defaultColor = new Color(0, 0, 0, 0);
    private Color canColor = new Color(0, 1, 0, 0.5f);
    private Color cantColor = new Color(1, 0, 0, 0.5f);
    private RectTransform indicatorRectTransform;
    private bool isRotated = false;
    private bool isInitialized = false;

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

        gridContainer = inventoryController.GridContainer;
        
        string slotType = isPocket ? "Pocket" : "Hand";
        Debug.Log($"HandDraggableItem initialized for {hand} {slotType}.");
        isInitialized = true;
        return true;
    }

    public void OnBeginDrag(PointerEventData eventData)
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
        
        string slotType = isPocket ? "Pocket" : "Hand";
        Debug.Log($"OnBeginDrag called for {hand} {slotType}. Item: {(currentItemData != null ? currentItemData.itemName : "null")}");

        if (currentItemData == null) return;

        if (inventoryController.InventoryItemPrefab == null)
        {
            Debug.LogError("InventoryItemPrefab is not set in the InventoryController. Please assign it in the Inspector.");
            return;
        }

        draggedItemGO = Instantiate(inventoryController.InventoryItemPrefab, inventoryController.transform.root);
        
        DraggableItem originalDraggable = draggedItemGO.GetComponent<DraggableItem>();
        if(originalDraggable != null)
        {
            originalDraggable.enabled = false;
            indicator = originalDraggable.indicator;
        }

        Image draggedImage = draggedItemGO.GetComponent<Image>();
        if (draggedImage != null)
        {
            draggedImage.sprite = currentItemData.inventoryTexture;
        }

        draggedItemCanvasGroup = draggedItemGO.GetComponent<CanvasGroup>();
        draggedItemCanvasGroup.blocksRaycasts = false;
        draggedItemCanvasGroup.alpha = 0.7f;

        RectTransform rectTransform = draggedItemGO.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(currentItemData.width * inventoryController.CellSize, currentItemData.height * inventoryController.CellSize);

        if (indicator != null)
        {
            indicatorRectTransform = indicator.GetComponent<RectTransform>();
            indicatorRectTransform.sizeDelta = rectTransform.sizeDelta;
            indicator.color = defaultColor;
        }

        slotImage.enabled = false;
        isRotated = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedItemGO == null) return;

        draggedItemGO.transform.position = eventData.position;

        if (indicator == null) return;

        if (inventory.IsPlacementToSlot(indicatorRectTransform))
        {
            indicator.color = canColor;
            return;
        }

        var pos = GetXY(eventData);
        int w = isRotated ? currentItemData.height : currentItemData.width;
        int h = isRotated ? currentItemData.width : currentItemData.height;

        indicator.color = inventory.IsPlacementPossible(pos.x, pos.y, w, h, null) ? canColor : cantColor;
    }

    void Update()
    {
        if (draggedItemGO != null && Input.GetKeyDown(KeyCode.R))
        {
            isRotated = !isRotated;
            int w = isRotated ? currentItemData.height : currentItemData.width;
            int h = isRotated ? currentItemData.width : currentItemData.height;
            draggedItemGO.GetComponent<RectTransform>().sizeDelta = new Vector2(w * inventoryController.CellSize, h * inventoryController.CellSize);
            if(indicatorRectTransform != null)
            {
                indicatorRectTransform.sizeDelta = new Vector2(w * inventoryController.CellSize, h * inventoryController.CellSize);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItemGO == null) { slotImage.enabled = true; return; }

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

    private (int x, int y) GetXY(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridContainer,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);

        RectTransform parentRect = gridContainer;
        float deltaX = parentRect.rect.width / 2;
        float deltaY = parentRect.rect.height / 2;
        int newX = Mathf.FloorToInt((localPoint.x + deltaX) / inventoryController.CellSize);
        int newY = Mathf.FloorToInt((-localPoint.y + deltaY) / inventoryController.CellSize);

        return (newX, newY);
    }
}

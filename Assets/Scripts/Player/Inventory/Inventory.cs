using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public event Action<InventoryItem> OnItemAdded;
    public event Action<InventoryItem> OnItemRemoved;

    private HandInventory _handInventory;

    public bool isOpen => inventoryUI.activeSelf;

    [Header("Grid Settings")]
    [SerializeField] private int width = 6;
    [SerializeField] private int height = 3;

    private InventoryItem[,] grid;
    private List<InventoryItem> items = new List<InventoryItem>();

    [Header("UI")]
    [SerializeField] private GameObject inventoryUI;

    private RectTransform _leftHandInventory;
    private RectTransform _rightHandInventory;
    private RectTransform _leftPocket;
    private RectTransform _rightPocket;

    [Header("Drop Settings")]
    private Transform playerCameraTransform;
    [SerializeField] private float dropForce = 5f;
    [SerializeField] private float dropOffset = 1.5f;

    void Awake()
    {
        grid = new InventoryItem[width, height];
        // items = new List<InventoryItem>(); // Moved to declaration
        _handInventory = GetComponent<HandInventory>();
        playerCameraTransform = GetComponentInChildren<Camera>().transform;

        _leftHandInventory = _handInventory._leftHandInventory.GetComponent<RectTransform>();
        _rightHandInventory = _handInventory._rightHandInventory.GetComponent<RectTransform>();
        _leftPocket = _handInventory._leftPocket.GetComponent<RectTransform>();
        _rightPocket = _handInventory._rightPocket.GetComponent<RectTransform>();
    }

    public void DropItemFromHand(Hand hand, bool isPocket)
    {   
        var item = isPocket ? _handInventory.UnequipFromPocket(hand) : _handInventory.UnequipFromHand(hand);
        if (item == null)
            return;
        DropItem(item);
    }

    public void DropItemFromGrid(InventoryItem item)
    {
        if (item == null || item.itemData.prefab == null)
            return;

        DropItem(item.itemData);
        RemoveItem(item);
    }

    private void DropItem(ItemData itemData)
    {
        Vector3 spawnPosition = playerCameraTransform.position + playerCameraTransform.forward * dropOffset;
        GameObject droppedItemObj = Instantiate(itemData.prefab, spawnPosition, Quaternion.identity);
        
        if (droppedItemObj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(playerCameraTransform.forward * dropForce, ForceMode.Impulse);
        }
    }

    public List<InventoryItem> GetItems()
    {
        return items;
    }


    public bool IsPlacementPossible(InventoryItem item)
    {
        return IsPlacementPossible(item.x, item.y, item.GetWidth(), item.GetHeight(), item);
    }

    public bool IsPlacementPossible(int startX, int startY, int itemWidth, int itemHeight, InventoryItem item)
    {
        if (startX < 0 || startY < 0 || startX + itemWidth > width || startY + itemHeight > height)
        {
            return false;
        }

        for (int i = 0; i < itemWidth; i++)
        {
            for (int j = 0; j < itemHeight; j++)
            {
                if (grid[startX + i, startY + j] != null && grid[startX + i, startY + j] != item)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void PlaceItemOnGrid(InventoryItem item)
    {
        for (int i = 0; i < item.GetWidth(); i++)
        {
            for (int j = 0; j < item.GetHeight(); j++)
            {
                grid[item.x + i, item.y + j] = item;
            }
        }
    }

    private void RemoveItemFromGrid(InventoryItem item)
    {
        for (int i = 0; i < item.GetWidth(); i++)
        {
            for (int j = 0; j < item.GetHeight(); j++)
            {
                int currentX = item.x + i;
                int currentY = item.y + j;
                if (currentX < width && currentY < height && grid[currentX, currentY] == item)
                {
                    grid[currentX, currentY] = null;
                }
            }
        }
    }

    public bool AddItem(InventoryItem newItem)
    {
        if (!IsPlacementPossible(newItem))
        {
            return false;
        }

        items.Add(newItem);
        PlaceItemOnGrid(newItem);

        OnItemAdded?.Invoke(newItem);
        return true;
    }

    public void RemoveItem(InventoryItem itemToRemove)
    {
        if (itemToRemove == null || !items.Contains(itemToRemove))
        {
            return;
        }

        RemoveItemFromGrid(itemToRemove);
        items.Remove(itemToRemove);
        OnItemRemoved?.Invoke(itemToRemove);
    }

    public bool MoveItem(InventoryItem item, int newX, int newY)
    {
        RemoveItemFromGrid(item);

        int oldX = item.x;
        int oldY = item.y;

        item.x = newX;
        item.y = newY;

        if (IsPlacementPossible(item))
        {
            PlaceItemOnGrid(item);
            return true;
        }
        else
        {
            item.x = oldX;
            item.y = oldY;
            PlaceItemOnGrid(item);
            return false;
        }
    }

    public bool IsPlacementToSlot(RectTransform item)
    {
        if (UIHelper.IsPartialOverlap(item, _leftHandInventory) && _handInventory.leftHandSlot.IsEmpty())
            return true;
        if (UIHelper.IsPartialOverlap(item, _rightHandInventory) && _handInventory.rightHandSlot.IsEmpty())
            return true;
        if (UIHelper.IsPartialOverlap(item, _leftPocket) && _handInventory.leftPocketSlot.IsEmpty())
            return true;
        if (UIHelper.IsPartialOverlap(item, _rightPocket) && _handInventory.rightPocketSlot.IsEmpty())
            return true;
        return false;
    }

    public bool EquipFromInventoryToHand(InventoryItem item, RectTransform rectTransformItem)
    {
        bool equipped = false;
        if (UIHelper.IsPartialOverlap(rectTransformItem, _leftHandInventory) && _handInventory.leftHandSlot.IsEmpty())
            equipped = _handInventory.TryEquipToHand(item.itemData, Hand.Left);
        else if (UIHelper.IsPartialOverlap(rectTransformItem, _rightHandInventory) && _handInventory.rightHandSlot.IsEmpty())
            equipped = _handInventory.TryEquipToHand(item.itemData, Hand.Right);
        else if (UIHelper.IsPartialOverlap(rectTransformItem, _leftPocket) && _handInventory.leftPocketSlot.IsEmpty())
            equipped = _handInventory.TryEquipToPocket(item.itemData, Hand.Left);
        else if (UIHelper.IsPartialOverlap(rectTransformItem, _rightPocket) && _handInventory.rightPocketSlot.IsEmpty())
            equipped = _handInventory.TryEquipToPocket(item.itemData, Hand.Right);

        if (equipped)
        {
            RemoveItem(item);
        }
        
        return equipped;
    }

    // FOR TEST
    public bool TryAddItem(ItemData itemData)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (IsPlacementPossible(x, y, itemData.width, itemData.height, null))
                {
                    InventoryItem newItem = new InventoryItem(itemData, x, y);
                    AddItem(newItem);
                    return true;
                }
            }
        }
        return false;
    }
}

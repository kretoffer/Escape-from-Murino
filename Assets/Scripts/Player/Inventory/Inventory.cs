using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public event Action<InventoryItem> OnItemAdded;
    public event Action<InventoryItem> OnItemRemoved;

    [Header("Grid Settings")]
    [SerializeField] private int width = 6;
    [SerializeField] private int height = 3;

    private InventoryItem[,] grid;
    private List<InventoryItem> items;

    [Header("UI")]
    [SerializeField] private GameObject inventoryUI;
    private CameraController _cameraController;

    void Awake()
    {
        grid = new InventoryItem[width, height];
        items = new List<InventoryItem>();
        _cameraController = GetComponent<CameraController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            if (inventoryUI.activeSelf) _cameraController.Deactivate();
            else _cameraController.Activate();
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

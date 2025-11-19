using UnityEngine;

public class HandSlot
{
    public ItemData currentItem { get; private set; }

    public HandSlot()
    {
        currentItem = null;
    }

    public void Equip(ItemData item)
    {
        currentItem = item;
    }

    public ItemData Unequip()
    {
        ItemData itemToReturn = currentItem;
        currentItem = null;
        return itemToReturn;
    }
    
    public bool IsEmpty()
    {
        return currentItem == null;
    }
}
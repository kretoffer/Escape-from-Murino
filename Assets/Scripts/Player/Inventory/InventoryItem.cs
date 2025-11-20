public class InventoryItem
{
    public ItemData itemData;
    public int x;
    public int y;
    public bool isRotated;

    public InventoryItem(ItemData data, int x, int y)
    {
        this.itemData = data;
        this.x = x;
        this.y = y;
        this.isRotated = false;
    }

    public void Rotate()
    {
        isRotated = !isRotated;
    }

    public int GetWidth()
    {
        return isRotated ? itemData.height : itemData.width;
    }

    public int GetHeight()
    {
        return isRotated ? itemData.width : itemData.height;
    }
}

using UnityEngine;
using UnityEngine.UI;

public static class UIHelper
{
    public static bool IsOverlapping(RectTransform rect1, RectTransform rect2)
    {
        Rect r1 = GetWorldRect(rect1);
        Rect r2 = GetWorldRect(rect2);
        return r1.Overlaps(r2);
    }

    private static Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector2 size = corners[2] - corners[0];
        return new Rect(corners[0], size);
    }

    public static Rect GetPartialRect(RectTransform rt, Vector2 size)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        // Левый верхний угол в мировых координатах
        Vector3 topLeft = corners[1]; // corners: 0=bottomLeft, 1=topLeft, 2=topRight, 3=bottomRight

        // Создаём прямоугольник 100x100 от этого угла
        return new Rect(topLeft.x, topLeft.y - size.y, size.x, size.y);
    }

    public static bool IsPartialOverlap(RectTransform rt1, RectTransform rt2)
    {
        Rect r1 = GetPartialRect(rt1, new Vector2(100, 100));
        Rect r2 = GetWorldRect(rt2); // обычный полный прямоугольник

        return r1.Overlaps(r2);
    }
}

using UnityEngine;

public class ElevatorButtons : InteractiveObject
{
    public enum Direction { Up, Down }
    [SerializeField] private Direction direction = Direction.Up;

    override public void Interact()
    {
        if (direction == Direction.Up)
        {
            FloorController.Instance.GoUp();
        }
        else
        {
            FloorController.Instance.GoDown();
        }
    }

    void Start()
    {
        name = direction == Direction.Up ? "Вверх" : "Вниз"; 
    }
}

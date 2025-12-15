using UnityEngine;

public class ElevatorButtons : MonoBehaviour, IInteractive
{
    public enum Direction { Up, Down }
    public string name { get; private set; }
    [SerializeField] private Direction direction = Direction.Up;

    public void Interact()
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

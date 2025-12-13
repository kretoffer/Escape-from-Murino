using UnityEngine;

public class TowerDoor : MonoBehaviour, IInteractive
{
    public enum Direction { In, Out }
    public string name { get; private set; }
    [SerializeField] private Direction direction = Direction.In;
    [SerializeField] private TowerType towerType;

    public void Interact()
    {
        if (direction == Direction.In)
        {
            TowerDataController.Instance.GoIn(towerType);
        }
        else
        {
            TowerDataController.Instance.GoOut();
        }
    }

    void Start()
    {
        name = direction == Direction.In ? "Войти" : "Выйти";
    }
}

using UnityEngine;

public class ElevatorButtons : InteractiveObject
{
    [SerializeField] private byte bias = 1;
    override public void Interact()
    {
        FloorController.Instance.floor += bias;
        FloorController.Instance.Reboot();
    }
}

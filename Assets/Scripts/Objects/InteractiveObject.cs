using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour, IInteractive
{
    [field: SerializeField] public new string name { get; set; }

    public abstract void Interact();
}

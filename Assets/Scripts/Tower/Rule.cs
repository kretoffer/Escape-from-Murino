
using UnityEngine;

public abstract class Rule : ScriptableObject
{
    public abstract void Apply();
    public abstract void Revert();
}

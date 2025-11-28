using UnityEngine;

[CreateAssetMenu(fileName = "New Memory", menuName = "Memories/Memory")]
public class Memory : ScriptableObject
{
    public string name;
    public string description;
    public string text;
    public Sprite icon;
}

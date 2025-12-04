using UnityEngine;

[CreateAssetMenu(fileName = "New Memory", menuName = "Memories/Memory")]
public class Memory : ScriptableObject
{
    public new string name;
    public string description;
    public string text;
    public Sprite icon;
}

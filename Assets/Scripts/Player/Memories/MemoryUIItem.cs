using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MemoryUIItem : MonoBehaviour
{
    public Memory memory { get; private set; }
    private MemoryController memoryController;

    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    // [SerializeField] private Image iconImage;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnMemoryClicked);
    }

    public void Setup(Memory memoryToSetup, MemoryController controller)
    {
        this.memory = memoryToSetup;
        this.memoryController = controller;
        
        if (nameText != null) nameText.text = memory.name;
        if (descriptionText != null) descriptionText.text = memory.description;
        // if (iconImage != null) iconImage.sprite = memory.icon;

        gameObject.name = $"Memory - {memory.name}";
    }

    private void OnMemoryClicked()
    {
        if (memoryController != null)
        {
            memoryController.ShowMemoryText(memory);
        }
    }
}
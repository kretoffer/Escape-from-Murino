using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    void Start()
    {
        // Create a new GameObject named "RuleManager"
        GameObject ruleManagerObject = new GameObject("RuleManager");

        // Add the RuleManager script to the GameObject
        ruleManagerObject.AddComponent<RuleManager>();

        // Destroy this setup script
        Destroy(gameObject);
    }
}

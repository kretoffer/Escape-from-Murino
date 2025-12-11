using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    void Start()
    {
        // Load all floor prefabs from the "floors/1" folder in Resources
        GameObject[] floorPrefabs = Resources.LoadAll<GameObject>("floors/1");

        // Check if there are any prefabs
        if (floorPrefabs.Length > 0)
        {
            // Pick a random prefab
            GameObject randomFloorPrefab = floorPrefabs[Random.Range(0, floorPrefabs.Length)];

            // Instantiate the random floor at position (0, 0, 0)
            Instantiate(randomFloorPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No floor prefabs found in Resources/floors/1");
        }

        // Create a new GameObject named "RuleManager"
        GameObject ruleManagerObject = new GameObject("RuleManager");

        // Add the RuleManager script to the GameObject
        ruleManagerObject.AddComponent<RuleManager>();

        // Destroy this setup script
        Destroy(gameObject);
    }
}

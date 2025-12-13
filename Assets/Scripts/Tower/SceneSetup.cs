using UnityEngine;

// Data structures for JSON parsing
[System.Serializable]
public class TowerFloorInfo
{
    public string name;
    public int count;
}

[System.Serializable]
public class FloorsConfig
{
    public TowerFloorInfo[] towerFloors;
}

public class SceneSetup : MonoBehaviour
{
    void Start()
    {
        // Load the JSON config file
        TextAsset configFile = Resources.Load<TextAsset>("floorsConfig");
        if (configFile == null)
        {
            Debug.LogError("Failed to load floorsConfig.json from Resources. Make sure the file exists and is not in a subdirectory.");
            // Destroy this setup script to prevent further errors
            Destroy(gameObject);
            return;
        }

        FloorsConfig config = JsonUtility.FromJson<FloorsConfig>(configFile.text);
        
        string towerTypeName = TowerDataController.Instance.tower.ToString();
        
        TowerFloorInfo floorInfo = System.Array.Find(config.towerFloors, info => info.name == towerTypeName);

        if (floorInfo != null && floorInfo.count > 0)
        {
            // Pick a random floor number (assuming floors are named floor1, floor2, etc.)
            int randomFloorIndex = Random.Range(1, floorInfo.count + 1);
            string prefabName = "floor" + randomFloorIndex;

            string path = "FloorsData/" + towerTypeName + "/" + prefabName;

            GameObject floorPrefab = Resources.Load<GameObject>(path);

            if (floorPrefab != null)
            {
                Instantiate(floorPrefab, Vector3.zero, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Failed to load floor prefab at path: " + path);
            }
        }
        else
        {
            Debug.LogError("No floor config found for tower type: '" + towerTypeName + "' in floorsConfig.json, or its count is zero.");
        }

        // Create a new GameObject named "RuleManager"
        GameObject ruleManagerObject = new GameObject("RuleManager");

        // Add the RuleManager script to the GameObject
        ruleManagerObject.AddComponent<RuleManager>();

        // Destroy this setup script
        Destroy(gameObject);
    }
}

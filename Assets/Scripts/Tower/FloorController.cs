using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FloorController : MonoBehaviour
{
    public static FloorController Instance { get; private set; }

     public int floor = 0;
    private bool isRebooting = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 1) return;
        isRebooting = false;
    }

    private void Reboot()
    {
        if (isRebooting)
        {
            Debug.LogWarning("Reboot already in progress.");
            return;
        }

        isRebooting = true;
        SceneManager.LoadScene(1);
    }

    public void GoUp()
    {
        floor++;
        Reboot();
    }

    public void GoDown()
    {
        floor--;
        if (floor < 0) // Should not happen if floor 0 is handled by GoOut
        {
            floor = 0; // Prevent negative floor
        }

        if (floor == 0) // If going to floor 0, exit tower
        {
            TowerDataController.Instance.GoOut();
        }
        else
        {
            Reboot();
        }
    }
}


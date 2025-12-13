using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FloorController : MonoBehaviour
{
    public static FloorController Instance { get; private set; }

    public int floor = 0;
    private AsyncOperation sceneLoadingUpOperation;
    private AsyncOperation sceneLoadingDownOperation;
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
        isRebooting = false;
        StartCoroutine(PreloadSceneRoutine());
    }

    private IEnumerator PreloadSceneRoutine()
    {
        // We wait a frame to let the new scene initialize before we start loading it again.
        yield return null; 

        // Unity allows only one async scene load operation at a time.
        // We prioritize preloading the 'up' movement.
        sceneLoadingUpOperation = SceneManager.LoadSceneAsync(1);
        if (sceneLoadingUpOperation != null)
        {
            sceneLoadingUpOperation.allowSceneActivation = false;
        }

        if (floor <= 1)
        {
            // 'Down' would go to a different scene (0), which we can't preload.
            sceneLoadingDownOperation = null;
        }
        else
        {
            // 'Down' goes to the same scene as 'up' (1), so we can reuse the operation.
            sceneLoadingDownOperation = sceneLoadingUpOperation;
        }
    }

    private void Reboot(AsyncOperation sceneLoadingOperation)
    {
        if (isRebooting)
        {
            Debug.LogWarning("Reboot already in progress.");
            return;
        }

        // Check if the target scene is the one we have been preloading and it's ready.
        if (sceneLoadingOperation != null && sceneLoadingOperation.progress >= 0.9f)
        {
            isRebooting = true;
            // The scene is preloaded, activate it for a fast transition.
            sceneLoadingOperation.allowSceneActivation = true;
        }
    }

    public void GoUp()
    {
        floor++;
        Reboot(sceneLoadingUpOperation);
    }

    public void GoDown()
    {
        floor--;
        if (sceneLoadingDownOperation != null)
        {
            // Use preloaded scene if available
            Reboot(sceneLoadingDownOperation);
        }
        else
        {
            TowerDataController.Instance.GoOut();
        }
    }
}


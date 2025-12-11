using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FloorController : MonoBehaviour
{
    public static FloorController Instance { get; private set; }

    public int floor = 0;
    private AsyncOperation sceneLoadingOperation;
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
        // When a new scene is loaded, we automatically start pre-loading it
        // in the background. This makes it ready for the next Reboot call.
        isRebooting = false;
        StartCoroutine(PreloadSceneRoutine(scene.buildIndex));
    }

    private IEnumerator PreloadSceneRoutine(int sceneIndex)
    {
        // We wait a frame to let the new scene initialize before we start loading it again.
        yield return null; 
        sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
        if (sceneLoadingOperation != null)
        {
            sceneLoadingOperation.allowSceneActivation = false;
        }
    }

    /// <summary>
    /// Activates the pre-loaded scene, effectively rebooting the level.
    /// </summary>
    public void Reboot()
    {
        if (isRebooting)
        {
            Debug.LogWarning("Reboot already in progress.");
            return;
        }

        // A scene is ready for activation when its progress is at least 0.9f.
        if (sceneLoadingOperation != null && sceneLoadingOperation.progress >= 0.9f)
        {
            isRebooting = true;
            // The scene is preloaded, now we can activate it.
            sceneLoadingOperation.allowSceneActivation = true;
        }
        else
        {
            Debug.LogError("Reboot failed: Scene is not ready for activation.");
        }
    }
}

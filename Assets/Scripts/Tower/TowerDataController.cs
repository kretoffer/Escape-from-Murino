using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum TowerType
{
    Medium
}

public class TowerDataController : MonoBehaviour
{
    public static TowerDataController Instance { get; private set; }
    public TowerType tower;

    private Vector3 PlayerPosition;
    private Vector3 PlayerRotation;

    public bool isEntered = false;
    public bool isCameOut = false;

    private GameObject _player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RepositionPlayerCoroutine());
    }

    private IEnumerator RepositionPlayerCoroutine()
    {
        yield return null;

        _player = GameObject.FindWithTag("Player");

        if (isCameOut)
        {
            _player.transform.position = PlayerPosition;
            _player.transform.rotation = Quaternion.Euler(PlayerRotation) * Quaternion.Euler(0, 180f, 0);
            isCameOut = false;    
        }
    }

    public void GoIn()
    {
        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player");
        }
        
        PlayerPosition = _player.transform.position;
        PlayerRotation = _player.transform.rotation.eulerAngles;
        isEntered = true;
        SceneManager.LoadScene(1);
    }

    public void GoOut()
    {
        isCameOut = true;
        SceneManager.LoadScene(0);
    }
}
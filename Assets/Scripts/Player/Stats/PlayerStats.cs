using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Max Stats")]
    public float maxHunger = 100f;
    public float maxSanity = 100f;
    public float maxFatigue = 100f;

    [Header("Current Stats")]
    public float currentHunger;
    public float currentSanity;
    public float currentFatigue;

    [Header("Base Decay Rates (per second)")]
    public float hungerDecayRate = 0.5f;
    public float sanityDecayRate = 0.1f;
    public float fatigueDecayRate = 0.2f;

    [Header("State Multipliers")]
    public float runHungerMultiplier = 2f;
    public float runFatigueMultiplier = 3f;
    public float crouchHungerMultiplier = 0.5f;

    [Header("Action Costs")]
    public float jumpHungerCost = 1f;
    public float jumpFatigueCost = 2f;

    private bool _isRunning;
    private bool _isCrouching;
    
    private IAbnormal[] _abnormals;
    private Camera _camera;

    void Start()
    {
        currentHunger = maxHunger;
        currentSanity = maxSanity;
        currentFatigue = maxFatigue;
        
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            _camera = player.GetComponentInChildren<Camera>();
            if (_camera == null)
            {
                Debug.LogWarning("[PlayerStats] Could not find Camera in children of Player object. Using Camera.main as fallback.");
                _camera = Camera.main;
            }
        }
        else
        {
            Debug.LogError("[PlayerStats] Player object with tag 'Player' not found! Using Camera.main as fallback.");
            _camera = Camera.main;
        }

        if (_camera == null)
        {
            Debug.LogError("[PlayerStats] No camera found (neither child of Player nor MainCamera). Abnormal effects will not work.");
        }

        _abnormals = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IAbnormal>().ToArray();
    }

    void Update()
    {
        var currentHungerDecay = hungerDecayRate;
        var currentFatigueDecay = fatigueDecayRate;
        var currentSanityDecay = sanityDecayRate;

        if (_isRunning)
        {
            currentHungerDecay *= runHungerMultiplier;
            currentFatigueDecay *= runFatigueMultiplier;
        }
        else if (_isCrouching)
        {
            currentHungerDecay *= crouchHungerMultiplier;
        }

        var abnormalMultiplier = 0f;
        if (_camera != null && _abnormals != null)
        {
            foreach (var abnormal in _abnormals)
            {
                var monoBehaviour = abnormal as MonoBehaviour;
                if (monoBehaviour == null || !monoBehaviour.isActiveAndEnabled) continue;
                
                var abnormalRenderer = monoBehaviour.GetComponent<Renderer>();
                if (abnormalRenderer != null && IsVisible(abnormalRenderer))
                {
                    abnormalMultiplier += abnormal.Level;
                }
            }
        }
        
        currentSanityDecay *= 1 + abnormalMultiplier;

        ChangeHunger(-currentHungerDecay * Time.deltaTime);
        ChangeSanity(-currentSanityDecay * Time.deltaTime);
        ChangeFatigue(-currentFatigueDecay * Time.deltaTime);
    }

    private bool IsVisible(Renderer renderer)
    {
        // First, check if the object is within the camera's frustum
        var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
        if (!GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
        {
            return false;
        }

        // Then, do a raycast from the camera to the object to check for occlusion
        Vector3 direction = renderer.bounds.center - _camera.transform.position;
        if (Physics.Raycast(_camera.transform.position, direction, out var hit, direction.magnitude))
        {
            // If we hit something, check if it's the object we're looking for
            if (hit.collider.gameObject == renderer.gameObject)
            {
                return true; // The object is visible
            }
        } else {
            // if the raycast doesn't hit anything, it means the object is visible
            return true;
        }

        return false; // The object is occluded
    }

    public void SetMovementState(bool isRunning, bool isCrouching)
    {
        _isRunning = isRunning;
        _isCrouching = isCrouching;
    }

    public void OnJump()
    {
        ChangeHunger(-jumpHungerCost);
        ChangeFatigue(-jumpFatigueCost);
    }

    public void ChangeHunger(float amount)
    {
        currentHunger += amount;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
    }

    public void ChangeSanity(float amount)
    {
        currentSanity += amount;
        currentSanity = Mathf.Clamp(currentSanity, 0, maxSanity);
    }

    public void ChangeFatigue(float amount)
    {
        currentFatigue += amount;
        currentFatigue = Mathf.Clamp(currentFatigue, 0, maxFatigue);
    }
}

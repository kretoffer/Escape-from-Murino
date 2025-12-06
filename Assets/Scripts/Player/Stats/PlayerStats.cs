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

    void Start()
    {
        currentHunger = maxHunger;
        currentSanity = maxSanity;
        currentFatigue = maxFatigue;
    }

    void Update()
    {
        float currentHungerDecay = hungerDecayRate;
        float currentFatigueDecay = fatigueDecayRate;

        if (_isRunning)
        {
            currentHungerDecay *= runHungerMultiplier;
            currentFatigueDecay *= runFatigueMultiplier;
        }
        else if (_isCrouching)
        {
            currentHungerDecay *= crouchHungerMultiplier;
        }

        ChangeHunger(-currentHungerDecay * Time.deltaTime);
        ChangeSanity(-sanityDecayRate * Time.deltaTime);
        ChangeFatigue(-currentFatigueDecay * Time.deltaTime);
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

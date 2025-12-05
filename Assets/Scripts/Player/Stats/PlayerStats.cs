using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHunger = 100f;
    public float currentHunger;

    public float maxSanity = 100f;
    public float currentSanity;

    public float maxFatigue = 100f;
    public float currentFatigue;

    // Decay rates per second
    public float hungerDecayRate = 0.5f;
    public float sanityDecayRate = 0.1f;
    public float fatigueDecayRate = 0.2f;

    void Start()
    {
        currentHunger = maxHunger;
        currentSanity = maxSanity;
        currentFatigue = maxFatigue;
    }

    void Update()
    {
        // Decay stats over time
        ChangeHunger(-hungerDecayRate * Time.deltaTime);
        ChangeSanity(-sanityDecayRate * Time.deltaTime);
        ChangeFatigue(-fatigueDecayRate * Time.deltaTime);
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

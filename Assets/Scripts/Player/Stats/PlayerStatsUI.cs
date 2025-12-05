using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    private PlayerStats playerStats;

    public Slider hungerSlider;
    public Slider sanitySlider;
    public Slider fatigueSlider;

    void Start()
    {
        // Automatically find the PlayerStats component by tag if not assigned
        if (playerStats == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerStats = player.GetComponent<PlayerStats>();
            }
        }
    }

    void Update()
    {
        if (playerStats != null)
        {
            if (hungerSlider != null)
            {
                hungerSlider.value = playerStats.currentHunger / playerStats.maxHunger;
            }

            if (sanitySlider != null)
            {
                sanitySlider.value = playerStats.currentSanity / playerStats.maxSanity;
            }

            if (fatigueSlider != null)
            {
                fatigueSlider.value = playerStats.currentFatigue / playerStats.maxFatigue;
            }
        }
    }
}

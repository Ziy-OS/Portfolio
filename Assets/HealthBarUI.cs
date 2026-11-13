using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameEventSO healthSO;
    public Slider healthSlider;

    private void Start()
    {
        if (healthSO == null || healthSlider == null)
        {
            Debug.LogWarning("Assign the Health SO and Health Slider!");
            return;
        }

        healthSO.ResetHealth();

        healthSlider.minValue = 0f;
        healthSlider.maxValue = healthSO.maxHealth;
        healthSlider.value = healthSO.currentHealth;
    }

    private void Update()
    {
        if (healthSO != null && healthSlider != null)
        {
            healthSlider.value = healthSO.currentHealth;
        }
    }
}


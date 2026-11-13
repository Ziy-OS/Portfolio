using UnityEngine;

[CreateAssetMenu(fileName = "HealthSO", menuName = "Game Data/Health")]
public class GameEventSO : ScriptableObject
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;

// Reset health
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }


    public void Subtract(float amount)
    {
        currentHealth = Mathf.Max(0f, currentHealth - amount);
        Debug.Log("Current Health:" + currentHealth);
    }
}



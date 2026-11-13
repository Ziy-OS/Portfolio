using UnityEngine;
using UnityEngine.Events;

public class HealthTrigger : MonoBehaviour
{
    public GameEventSO healthSO;
    public float damageAmount = 10f;

    // Play Audio
    public AudioSource damageTakenAudioSource;

    public UnityEvent onHealthSubtracted;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (healthSO == null)
        {
            Debug.LogWarning("HealthSO is not assigned!");
            return;
        }

        healthSO.Subtract(damageAmount);
        onHealthSubtracted?.Invoke();
        damageTakenAudioSource.PlayOneShot(damageTakenAudioSource.clip);
    }
}


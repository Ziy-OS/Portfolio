
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Slider energySlider;

    [Header("ScriptableObject References")]
    [SerializeField] private FloatVariableSO energyVariable;
    [SerializeField] private TelekinesisGameEventSO energyChangedEvent;

    private void OnEnable()
    {
        // Subscribe to the energy changed event.
        energyChangedEvent.RegisterListener(UpdateEnergyBar);

        // Display the current energy when enabled.
        UpdateEnergyBar();
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent unwanted event calls.
        energyChangedEvent.UnregisterListener(UpdateEnergyBar);
    }

    private void UpdateEnergyBar()
    {
        // Update the Slider using the shared energy value.
        energySlider.value = energyVariable.value;
    }
}


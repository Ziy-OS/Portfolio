using UnityEngine;

/// <summary>
/// Lab 8 - the general-purpose version of the "holds a number" half of
/// Lab 5's HealthSO. Stores one float, and nothing else - no Subtract(),
/// no game-specific logic. Any script can read or write .value directly,
/// which is what makes it reusable for health, energy, stamina, or
/// anything else that's just a shared number.
/// </summary>
[CreateAssetMenu(fileName = "NewFloatVariable", menuName = "Game Data/Float Variable")]
public class FloatVariableSO : ScriptableObject
{
    [Tooltip("The current value. Any script can read or write this directly.")]
    public float value;
}

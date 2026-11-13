using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Lab 8 - a ScriptableObject that holds no data at all. Its only job is
/// to be Raised() - announcing "something happened" to anyone who has
/// registered interest, without either side needing a direct reference
/// to the other. Same register/unregister shape as Week 6's Input System
/// subscription, just subscribing to this asset instead of an InputAction.
/// </summary>
[CreateAssetMenu(fileName = "NewGameEvent", menuName = "Game Events/Game Event")]
public class TelekinesisGameEventSO : ScriptableObject
{
    private readonly List<UnityAction> listeners = new List<UnityAction>();

    /// <summary>
    /// Calls every registered listener, in reverse order so a listener
    /// that unregisters itself mid-callback doesn't break the loop.
    /// </summary>
    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].Invoke();
        }
    }

    public void RegisterListener(UnityAction listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnregisterListener(UnityAction listener)
    {
        listeners.Remove(listener);
    }
}

using System;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    // C# Event that any script can subscribe to
    public static event Action OnTick;

    [Header("Simulation Settings")]
    public float tickInterval = 0.2f; // 0.2 seconds = 5 ticks per second
    public bool isSimulationRunning = false;

    private float timer;

    void Update()
    {
        // If in Build Mode, do not advance the simulation
        if (!isSimulationRunning) return;

        timer += Time.deltaTime;

        // When the timer exceeds our interval, fire the tick
        if (timer >= tickInterval)
        {
            // Crucial: Subtract the interval instead of setting timer = 0.
            // This prevents "time drift" if frames stutter.
            timer -= tickInterval;

            // Broadcast the tick to all listeners
            OnTick?.Invoke();
        }
    }

    // Call this from a UI Button
    public void ToggleSimulation()
    {
        isSimulationRunning = !isSimulationRunning;
    }
}
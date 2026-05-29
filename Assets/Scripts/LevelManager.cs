using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Dependencies")]
    public TickManager tickManager;

    [Header("Level Quota")]
    public ResourceType targetResource = ResourceType.Iron;
    public int targetAmount = 10;

    private int currentAmount = 0;

    private void OnEnable()
    {
        FactoryEvents.OnItemSunk += HandleItemSunk;
    }

    private void OnDisable()
    {
        FactoryEvents.OnItemSunk -= HandleItemSunk;
    }

    private void TriggerWinCondition()
    {
        Debug.Log("LEVEL COMPLETE! Quota met.");

        // Stop the simulation instantly
        tickManager.isSimulationRunning = false;

        // TODO: Enable a "Victory" UI Canvas panel with a "Next Level" button
    }

    private void Start()
    {
        // Initialize the UI on frame one
        FactoryEvents.OnQuotaUpdated?.Invoke(currentAmount, targetAmount);
    }

    private void HandleItemSunk(ResourceType item)
    {
        if (item == targetResource)
        {
            currentAmount++;

            // Broadcast the new score
            FactoryEvents.OnQuotaUpdated?.Invoke(currentAmount, targetAmount);

            if (currentAmount >= targetAmount) TriggerWinCondition();
        }
    }
}
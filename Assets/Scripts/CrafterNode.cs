using UnityEngine;

public class CrafterNode : MonoBehaviour, IFactoryNode, IItemReceiver
{
    public Vector2Int GridPosition { get; set; }
    public Vector2Int FacingDirection { get; set; } // The direction it outputs the finished item

    [Header("Recipe Settings")]
    public ResourceType requiredInputA = ResourceType.Iron;
    public ResourceType requiredInputB = ResourceType.Copper;
    public ResourceType outputItem = ResourceType.Circuit;
    public int ticksToCraft = 5;

    [Header("Internal Storage")]
    public bool hasInputA = false;
    public bool hasInputB = false;
    public bool hasFinishedItem = false;

    [Header("State Tracking")]
    private bool isCrafting = false;
    private int currentCraftingTick = 0;

    private void OnEnable()
    {
        TickManager.OnTick += OnTick;
    }

    private void OnDisable()
    {
        TickManager.OnTick -= OnTick;
    }

    // 1. INPUT PHASE: Catching items from conveyors
    public bool TryReceiveItem(ResourceType item)
    {
        // Reject if we are currently crafting or already holding a finished item
        if (isCrafting || hasFinishedItem) return false;

        // Try to slot the item into requirement A
        if (item == requiredInputA && !hasInputA)
        {
            hasInputA = true;
            FactoryEvents.OnItemDespawned?.Invoke(GridPosition); // Consume the 3D visual
            return true;
        }

        // Try to slot the item into requirement B
        if (item == requiredInputB && !hasInputB)
        {
            hasInputB = true;
            FactoryEvents.OnItemDespawned?.Invoke(GridPosition); // Consume the 3D visual
            return true;
        }

        // If it's the wrong item or we already have that ingredient, reject it
        return false;
    }

    // 2. PROCESSING & OUTPUT PHASE
    public void OnTick()
    {
        // State 1: We have a finished item and need to push it out
        if (hasFinishedItem)
        {
            TryPushOutput();
            return; // Don't do anything else until the output clears
        }

        // State 2: We are actively crafting
        if (isCrafting)
        {
            currentCraftingTick++;
            if (currentCraftingTick >= ticksToCraft)
            {
                // Crafting complete!
                isCrafting = false;
                hasFinishedItem = true;
                currentCraftingTick = 0;
                Debug.Log($"CRAFTER: Finished crafting {outputItem} at {GridPosition}");
            }
            return;
        }

        // State 3: We are waiting to start crafting
        if (hasInputA && hasInputB && !isCrafting)
        {
            // Start the machine
            isCrafting = true;
            hasInputA = false;
            hasInputB = false; // Consume the ingredients logically
            Debug.Log($"CRAFTER: Started crafting {outputItem} at {GridPosition}");
        }
    }

    private void TryPushOutput()
    {
        Vector2Int targetPosition = GridPosition + FacingDirection;
        IFactoryNode nextNode = GridManager.Instance.GetNodeAt(targetPosition);

        if (nextNode is IItemReceiver receiver)
        {
            if (receiver.TryReceiveItem(outputItem))
            {
                // The belt took our item!
                hasFinishedItem = false;

                // Trigger the visual creation of the new item
                FactoryEvents.OnItemSpawned?.Invoke(GridPosition, targetPosition, outputItem);
            }
            else
            {
                Debug.LogWarning($"CRAFTER BLOCKED: Output at {targetPosition} is full.");
            }
        }
    }
}
using UnityEngine;

public class SplitterNode : MonoBehaviour, IFactoryNode
{
    public Vector2Int GridPosition { get; set; }
    public Vector2Int FacingDirection { get; set; } // The direction it receives items from

    [Header("Logical State")]
    public ResourceType currentItem = ResourceType.None;

    // The buffer for items arriving this tick
    public ResourceType itemArrivingNextTick = ResourceType.None;

    [Header("Splitter Settings")]
    public Vector2Int outputDirectionA;
    public Vector2Int outputDirectionB;

    // The state tracker: true = Output A, false = Output B
    private bool sendToA = true;

    private void OnEnable()
    {
        TickManager.OnTick += OnTick;
    }

    private void OnDisable()
    {
        TickManager.OnTick -= OnTick;
    }

    public void OnTick()
    {
        // 1. Accept any item that arrived during the previous tick
        if (itemArrivingNextTick != ResourceType.None)
        {
            currentItem = itemArrivingNextTick;
            itemArrivingNextTick = ResourceType.None;
        }

        // 2. If we have no item to route, do nothing
        if (currentItem == ResourceType.None) return;

        // 3. Determine which direction we are trying to output to this tick
        Vector2Int currentOutputDirection = sendToA ? outputDirectionA : outputDirectionB;
        Vector2Int targetPosition = GridPosition + currentOutputDirection;

        IFactoryNode nextNode = GridManager.Instance.GetNodeAt(targetPosition);

        // 4. Try to push the data
        if (nextNode is ConveyorNode nextConveyor)
        {
            // Check if the receiving conveyor is completely empty
            if (nextConveyor.currentItem == ResourceType.None &&
                nextConveyor.itemArrivingNextTick == ResourceType.None)
            {
                // Push the item to the next belt's buffer
                nextConveyor.itemArrivingNextTick = this.currentItem;
                this.currentItem = ResourceType.None;

                // CRITICAL: We only flip the toggle upon a SUCCESSFUL transfer.
                // If the belt was full, sendToA doesn't flip, and we try again next tick.
                sendToA = !sendToA;

                Debug.Log($"SPLITTER: Routed item to {(sendToA ? "B" : "A")} at {targetPosition}");
            }
            else
            {
                Debug.Log($"SPLITTER BLOCKED: Target at {targetPosition} is full. Waiting.");
            }
        }
    }
}
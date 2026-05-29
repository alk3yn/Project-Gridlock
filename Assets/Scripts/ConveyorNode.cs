using Mono.Cecil;
using UnityEngine;

public class ConveyorNode : MonoBehaviour, IFactoryNode, IItemReceiver
{
    public Vector2Int GridPosition { get; set; }
    public Vector2Int FacingDirection { get; set; }

    public ResourceType currentItem = ResourceType.None;
    public ResourceType itemArrivingNextTick = ResourceType.None;

    private void OnEnable() => TickManager.OnTick += OnTick;
    private void OnDisable() => TickManager.OnTick -= OnTick;

    // The Interface Method: Catches items from Spawners or previous belts
    public bool TryReceiveItem(ResourceType item)
    {
        // Only accept if both the current slot and the buffer are completely empty
        if (currentItem == ResourceType.None && itemArrivingNextTick == ResourceType.None)
        {
            itemArrivingNextTick = item;
            return true;
        }
        return false;
    }

    public void OnTick()
    {
        // 1. Accept any item that arrived during the previous tick
        if (itemArrivingNextTick != ResourceType.None)
        {
            currentItem = itemArrivingNextTick;
            itemArrivingNextTick = ResourceType.None;
        }

        // 2. If the belt is empty, we have nothing to push forward
        if (currentItem == ResourceType.None) return;

        // 3. Calculate where this item wants to go
        Vector2Int targetPosition = GridPosition + FacingDirection;
        IFactoryNode nextNode = GridManager.Instance.GetNodeAt(targetPosition);

        // 4. Try to push the data to ANY node that can receive items
        if (nextNode is IItemReceiver receiver)
        {
            // If the receiver successfully takes the item...
            if (receiver.TryReceiveItem(this.currentItem))
            {
                // ...Trigger the 2D visual sprite movement!
                FactoryEvents.OnItemMoved?.Invoke(GridPosition, targetPosition);

                // Clear our logical data
                this.currentItem = ResourceType.None;
                Debug.Log($"Tick! Item moved from {GridPosition} to {targetPosition}");
            }
        }
    }
}
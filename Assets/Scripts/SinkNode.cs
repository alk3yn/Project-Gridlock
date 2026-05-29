using UnityEngine;

public class SinkNode : MonoBehaviour, IFactoryNode, IItemReceiver
{
    public Vector2Int GridPosition { get; set; }
    public Vector2Int FacingDirection { get; set; }

    [Header("Sink Settings")]
    public ResourceType expectedResource = ResourceType.Iron;

    // The interface method called by the Conveyor
    public bool TryReceiveItem(ResourceType item)
    {
        // Only accept the item if it matches what the Sink wants
        if (item == expectedResource)
        {
            // 1. Tell the Level Manager we scored!
            FactoryEvents.OnItemSunk?.Invoke(item);

            // 2. Tell the Visual Manager to recycle the 3D model
            FactoryEvents.OnItemDespawned?.Invoke(GridPosition);

            Debug.Log($"SINK: Consumed {item} at {GridPosition}");
            return true;
        }

        // If it's the wrong item, reject it. The belt backs up.
        Debug.LogWarning($"SINK REJECTED: Expected {expectedResource}, but got {item}");
        return false;
    }

    public void OnTick()
    {
        // The Sink doesn't actively do anything on its own tick, 
        // it purely reacts when a Conveyor calls TryReceiveItem().
    }
}
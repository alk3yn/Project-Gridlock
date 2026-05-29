using Mono.Cecil;
using UnityEngine;

public class SpawnerNode : MonoBehaviour, IFactoryNode
{
    public Vector2Int GridPosition { get; set; }
    public Vector2Int FacingDirection { get; set; }

    [Header("Spawner Settings")]
    public ResourceType resourceToSpawn = ResourceType.Iron;
    public int ticksBetweenSpawns = 3;

    private int currentTickCount = 0;

    private void OnEnable() => TickManager.OnTick += OnTick;
    private void OnDisable() => TickManager.OnTick -= OnTick;

    public void OnTick()
    {
        currentTickCount++;
        if (currentTickCount < ticksBetweenSpawns) return;

        Vector2Int targetPosition = GridPosition + FacingDirection;
        IFactoryNode nextNode = GridManager.Instance.GetNodeAt(targetPosition);

        // Talk to the Interface, not the Conveyor specifically
        if (nextNode is IItemReceiver receiver)
        {
            if (receiver.TryReceiveItem(this.resourceToSpawn))
            {
                currentTickCount = 0;

                // Tell the Visual Manager to instantiate a 2D Sprite
                FactoryEvents.OnItemSpawned?.Invoke(GridPosition, targetPosition, resourceToSpawn);

                Debug.Log($"SPAWN: {resourceToSpawn} created at {GridPosition} and pushed to {targetPosition}");
            }
            else
            {
                Debug.Log($"SPAWN BLOCKED: Node at {targetPosition} is full.");
            }
        }
    }
}
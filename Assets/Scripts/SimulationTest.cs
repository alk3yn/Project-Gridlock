using UnityEngine;

public class SimulationTest : MonoBehaviour
{
    void Start()
    {
        // 1. Create the Spawner at (0,0) facing Right (1,0)
        SpawnerNode spawner = new GameObject("Spawner").AddComponent<SpawnerNode>();
        spawner.FacingDirection = new Vector2Int(1, 0);
        spawner.resourceToSpawn = ResourceType.Iron;
        spawner.ticksBetweenSpawns = 3;
        GridManager.Instance.PlaceNode(new Vector2Int(0, 0), spawner);

        // 2. Create a line of 3 Conveyors starting at (1,0)
        for (int x = 1; x <= 3; x++)
        {
            ConveyorNode belt = new GameObject($"Belt_{x}").AddComponent<ConveyorNode>();
            belt.FacingDirection = new Vector2Int(1, 0); // Facing Right
            GridManager.Instance.PlaceNode(new Vector2Int(x, 0), belt);
        }

        Debug.Log("Test Grid Initialized. Toggle 'Is Simulation Running' on the TickManager to start.");
    }
}
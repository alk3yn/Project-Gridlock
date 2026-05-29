using System.Collections.Generic;
using UnityEngine;

public class VisualManager : MonoBehaviour
{
    // Maps a logical grid tile to the visual object currently moving into it
    private Dictionary<Vector2Int, GameObject> visualMap = new Dictionary<Vector2Int, GameObject>();

    // We need a reference to the TickManager to know the duration for our Lerp
    public TickManager tickManager;

    private void OnEnable()
    {
        FactoryEvents.OnItemSpawned += HandleSpawn;
        FactoryEvents.OnItemMoved += HandleMove;
        FactoryEvents.OnItemDespawned += HandleDespawn;
    }

    private void OnDisable()
    {
        FactoryEvents.OnItemSpawned -= HandleSpawn;
        FactoryEvents.OnItemMoved -= HandleMove;
        FactoryEvents.OnItemDespawned -= HandleDespawn;
    }

    private void HandleSpawn(Vector2Int from, Vector2Int to, ResourceType type)
    {
        GameObject newVisual = ResourcePool.Instance.GetIron();

        // Convert logical 2D grid coordinates to 3D world space (Assuming Z is forward/up)
        Vector3 worldFrom = new Vector3(from.x, from.y, 0f);
        Vector3 worldTo = new Vector3(to.x, to.y, 0f);

        newVisual.transform.position = worldFrom;

        ResourceVisual visualScript = newVisual.GetComponent<ResourceVisual>();
        visualScript.StartMoving(worldFrom, worldTo, tickManager.tickInterval);

        // Register the visual at its target destination in the map
        visualMap[to] = newVisual;
    }

    private void HandleMove(Vector2Int from, Vector2Int to)
    {
        if (visualMap.TryGetValue(from, out GameObject visualObj))
        {
            // Remove it from the old logical location
            visualMap.Remove(from);

            Vector3 worldFrom = new Vector3(from.x, from.y, 0f);
            Vector3 worldTo = new Vector3(to.x, to.y, 0f);

            ResourceVisual visualScript = visualObj.GetComponent<ResourceVisual>();
            visualScript.StartMoving(worldFrom, worldTo, tickManager.tickInterval);

            // Register it at the new logical location
            visualMap[to] = visualObj;
        }
    }

    private void HandleDespawn(Vector2Int pos)
    {
        if (visualMap.TryGetValue(pos, out GameObject visualObj))
        {
            visualMap.Remove(pos);
            ResourcePool.Instance.ReturnIron(visualObj);
        }
    }
}
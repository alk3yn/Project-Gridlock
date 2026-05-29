using System.Collections.Generic; // Required for Dictionaries
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    // UPGRADE: Replaced the fixed 20x20 array with an infinite Dictionary.
    // It only stores data where a machine actually exists, saving massive memory!
    private Dictionary<Vector2Int, IFactoryNode> grid = new Dictionary<Vector2Int, IFactoryNode>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlaceNode(Vector2Int position, IFactoryNode node)
    {
        // Add or overwrite the node at this exact coordinate
        grid[position] = node;
        node.GridPosition = position;
    }

    public bool IsTileEmpty(Vector2Int position)
    {
        // If the dictionary doesn't contain the coordinate, the tile is empty!
        return !grid.ContainsKey(position);
    }

    public IFactoryNode GetNodeAt(Vector2Int position)
    {
        // Safely check if a machine exists at this coordinate
        if (grid.TryGetValue(position, out IFactoryNode node))
        {
            return node;
        }
        return null;
    }
}
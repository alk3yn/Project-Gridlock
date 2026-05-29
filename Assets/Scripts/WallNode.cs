using UnityEngine;

// Occupies a grid space but does absolutely nothing.
public class WallNode : MonoBehaviour, IFactoryNode
{
    public Vector2Int GridPosition { get; set; }
    public Vector2Int FacingDirection { get; set; }
    public void OnTick() { }
}
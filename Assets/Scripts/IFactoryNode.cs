using UnityEngine;

public interface IFactoryNode
{
    // The exact X, Y coordinate on your logical grid
    Vector2Int GridPosition { get; set; }

    // What direction is this machine facing?
    Vector2Int FacingDirection { get; set; }

    // This is called once per simulation tick
    void OnTick();
}
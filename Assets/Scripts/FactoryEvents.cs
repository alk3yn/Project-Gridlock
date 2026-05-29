using System;
using UnityEngine;

// A static class to hold global gameplay events
public static class FactoryEvents
{
    // Fired by the Spawner. Passes: start pos, target pos, resource type
    public static Action<Vector2Int, Vector2Int, ResourceType> OnItemSpawned;

    // Fired by Conveyors/Splitters. Passes: from pos, to pos
    public static Action<Vector2Int, Vector2Int> OnItemMoved;

    // Fired by Sinks/Crafters. Passes: current pos
    public static Action<Vector2Int> OnItemDespawned;

    // Fired when the Sink consumes an item. Passes: resource type
    public static Action<ResourceType> OnItemSunk;

    // Fired when the quota progress changes. Passes: current amount, target amount
    public static Action<int, int> OnQuotaUpdated;
}
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildController : MonoBehaviour
{
    [Header("Dependencies")]
    public Camera mainCamera;
    public TickManager tickManager;

    [Header("Building Data")]
    public GameObject currentPrefabToBuild;
    private Vector2Int currentFacingDirection = new Vector2Int(1, 0);
    private int rotationAngle = 0;

    [Header("Hologram Visuals")]
    public Material hologramMaterial; // Assign a translucent blue/green material here
    private GameObject ghostCursor;
    private GameObject lastPrefabBuiltFrom; // Tracks if we changed the selected tool

    private Plane gridPlane = new Plane(Vector3.up, Vector3.zero);

    void Update()
    {
        // 1. Hide the cursor and lock building if simulation is running
        if (tickManager.isSimulationRunning)
        {
            if (ghostCursor != null) ghostCursor.SetActive(false);
            return;
        }

        // 2. Ensure we have the correct hologram generated
        if (ghostCursor == null || currentPrefabToBuild != lastPrefabBuiltFrom)
        {
            GenerateGhostCursor();
        }

        ghostCursor.SetActive(true);

        HandleRotationInput();
        UpdateCursorPositionAndHandleClick();
    }

    private void GenerateGhostCursor()
    {
        // Clean up the old cursor if we switched tools
        if (ghostCursor != null) Destroy(ghostCursor);

        // Instantiate a copy of the prefab to use as our cursor
        ghostCursor = Instantiate(currentPrefabToBuild);
        ghostCursor.name = "HologramCursor";
        lastPrefabBuiltFrom = currentPrefabToBuild;

        // STRIP LOGIC: Destroy all scripts implementing IFactoryNode so the cursor doesn't act like a real machine
        var logicComponents = ghostCursor.GetComponentsInChildren<MonoBehaviour>();
        foreach (var comp in logicComponents)
        {
            if (comp is IFactoryNode) Destroy(comp);
        }
        // STRIP PHYSICS: Remove 2D colliders
        var colliders = ghostCursor.GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders) Destroy(col);

        // APPLY VISUALS: Tint the SpriteRenderer transparent blue instead of changing materials
        var renderers = ghostCursor.GetComponentsInChildren<SpriteRenderer>();
        foreach (var rend in renderers)
        {
            rend.color = new Color(0.2f, 0.8f, 1f, 0.5f); // 50% transparent blue
        }
    }

    private void HandleRotationInput()
    {
        // Check if a keyboard exists, then check if 'R' was pressed
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            rotationAngle = (rotationAngle + 90) % 360;
            currentFacingDirection = new Vector2Int(currentFacingDirection.y, -currentFacingDirection.x);
            Debug.Log($"Rotated. Now facing: {currentFacingDirection}");
        }
    }

    private void UpdateCursorPositionAndHandleClick()
    {
        // Safety check to ensure a mouse is plugged in
        if (Mouse.current == null) return;

        // Read the mouse position from the New Input System
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector2Int gridPos = WorldToGrid(mouseWorldPos);

        ghostCursor.transform.position = GridToWorld(gridPos);
        ghostCursor.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

        // Read the left mouse button click
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryPlaceComponent(gridPos);
        }
    }

    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
    }

    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, gridPosition.y, 0f);
    }

    private void TryPlaceComponent(Vector2Int gridPos)
    {
        if (!GridManager.Instance.IsTileEmpty(gridPos)) return;

        Vector3 spawnWorldPos = GridToWorld(gridPos);
        GameObject newComponent = Instantiate(currentPrefabToBuild, spawnWorldPos, Quaternion.Euler(0, rotationAngle, 0));

        IFactoryNode logicNode = newComponent.GetComponent<IFactoryNode>();
        if (logicNode != null)
        {
            logicNode.FacingDirection = currentFacingDirection;
            GridManager.Instance.PlaceNode(gridPos, logicNode);
        }
    }
}
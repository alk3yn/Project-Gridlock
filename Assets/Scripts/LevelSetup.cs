using UnityEngine;

public class LevelSetup : MonoBehaviour
{
    void Start()
    {
        // Manually register pre-placed level pieces to the grid
        RegisterNode(transform.Find("Machine_Spawner"));
        RegisterNode(transform.Find("Machine_Sink"));
        RegisterNode(transform.Find("Wall"));
    }

    void RegisterNode(Transform obj)
    {
        if (obj != null && obj.TryGetComponent(out IFactoryNode node))
        {
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(obj.position.x), Mathf.RoundToInt(obj.position.y));
            GridManager.Instance.PlaceNode(pos, node);
            obj.position = new Vector3(pos.x, pos.y, 0); // Snap visually just in case
        }
    }
}
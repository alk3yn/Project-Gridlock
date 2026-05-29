using System.Collections.Generic;
using UnityEngine;

public class ResourcePool : MonoBehaviour
{
    public static ResourcePool Instance { get; private set; }

    [Header("Pool Settings")]
    public GameObject ironPrefab; // Assign a small 3D cube or sphere in the inspector
    public int poolSize = 50;

    private Queue<GameObject> ironPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Pre-warm the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(ironPrefab, transform);
            obj.SetActive(false);
            ironPool.Enqueue(obj);
        }
    }

    public GameObject GetIron()
    {
        if (ironPool.Count > 0)
        {
            GameObject obj = ironPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        // Fallback if the pool runs out (ideally, expand the pool)
        return Instantiate(ironPrefab, transform);
    }

    public void ReturnIron(GameObject obj)
    {
        obj.SetActive(false);
        ironPool.Enqueue(obj);
    }
}
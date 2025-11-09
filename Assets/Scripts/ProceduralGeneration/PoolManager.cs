using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple object pool per prefab type. Thread-unsafe (Unity main thread).
/// </summary>
public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class PoolEntry
    {
        public GameObject prefab;
        public int preload = 0;
    }

    [Header("Preload (optional)")]
    public PoolEntry[] entries;

    // prefab -> queue of instances
    private readonly Dictionary<GameObject, Queue<GameObject>> _pools = new();

    void Awake()
    {
        // Create queues and preload
        foreach (var e in entries)
        {
            if (e.prefab == null) continue;
            if (!_pools.ContainsKey(e.prefab)) _pools[e.prefab] = new Queue<GameObject>();
            for (int i = 0; i < e.preload; i++)
            {
                var go = Instantiate(e.prefab);
                go.SetActive(false);
                _pools[e.prefab].Enqueue(go);
            }
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        if (prefab == null) return null;

        if (!_pools.TryGetValue(prefab, out var q))
        {
            q = new Queue<GameObject>();
            _pools[prefab] = q;
        }

        GameObject go = q.Count > 0 ? q.Dequeue() : Instantiate(prefab);
        if (parent != null) go.transform.SetParent(parent, false);
        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);
        return go;
    }

    public void Despawn(GameObject prefab, GameObject instance)
    {
        if (prefab == null || instance == null) return;
        if (!_pools.TryGetValue(prefab, out var q))
        {
            q = new Queue<GameObject>();
            _pools[prefab] = q;
        }
        instance.SetActive(false);
        instance.transform.SetParent(null); // or keep parent; your call
        q.Enqueue(instance);
    }
}

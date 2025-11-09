using UnityEngine;

/// <summary>
/// Base behavior for a block. Detects player stepping using OnTriggerEnter (isTrigger=true).
/// </summary>
[RequireComponent(typeof(Collider))]
public class BlockBase : MonoBehaviour
{
    [Header("Runtime wiring")]
    public PoolManager pool;             // assigned by generator
    public GameObject sourcePrefab;      // prefab this instance belongs to (for pooling)
    public RowContainer owningRow;       // set by generator when spawned

    [Header("Tuning")]
    public float crumbleDelayOnStep = 2.5f;  // time until the *row* despawns after first step
    public bool oneShot = true;               // prevent multiple triggers

    protected bool _stepped;

    protected virtual void Awake()
    {
        // Find child colliders and make sure they relay to this parent.
        var childColliders = GetComponentsInChildren<Collider>(includeInactive: true);
        foreach (var col in childColliders)
        {
            if (col.gameObject == this.gameObject) continue; // skip parent (if any)
            col.isTrigger = true;

            var collider = col.GetComponent<Collider>();
            if (collider == null) continue;

            var relay = col.GetComponent<TriggerRelay>();
            if (relay == null) relay = col.gameObject.AddComponent<TriggerRelay>();
            relay.Init(this);
        }
    }

    protected virtual void OnEnable()
    {
        _stepped = false;
    }

    public void OnChildTriggerEnter(Collider other)
    {
        if (_stepped && oneShot) return;
        if (!other.CompareTag("Player")) return;

        _stepped = true;
        OnStepped(other);
    }

    /// <summary>
    /// Called once when the player steps on this block.
    /// Default: schedule the whole row to despawn after a delay.
    /// </summary>
    protected virtual void OnStepped(Collider player)
    {
        owningRow?.OnBlockStepped(this);
        owningRow?.generator?.SetSafeMode(false);
        // Add VFX/SFX here before despawn.
        gameObject.GetComponentInChildren<ShaderToggleSmooth>().DisableSmooth();
        owningRow?.ScheduleDespawnAll(crumbleDelayOnStep);
    }

    /// <summary>
    /// Return only this block to pool (used by RowContainer when it clears all).
    /// </summary>
    public virtual void ReturnToPool()
    {
        if (pool != null && sourcePrefab != null)
            pool.Despawn(sourcePrefab, gameObject);
        else
            gameObject.SetActive(false);
    }
}
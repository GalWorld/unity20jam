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
    public float crumbleDelayOnStep = 1.35f;  // time until the *row* despawns after first step
    public bool oneShot = true;               // prevent multiple triggers

    protected bool _stepped;

    protected virtual void OnEnable()
    {
        _stepped = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        // Called when something enters our trigger. Use tag/layer to filter player.
        // https://docs.unity3d.com/ScriptReference/Collider.OnTriggerEnter.html
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
        // You can add VFX/SFX here before despawn.
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
using UnityEngine;

public class SpawnBlock : BlockBase
{
    [Header("Spawn settings")]
    public bool consumeOnStep = true;

    protected override void OnStepped(Collider player)
    {
        // Example: request a buff/debuff spawn (replace with your item system)
        // ItemSpawner.Instance?.SpawnRandomAt(transform.position);

        if (consumeOnStep)
        {
            // Despawn only this block (not entire row)
            ReturnToPool();
        }
        // Do NOT schedule row despawn here unless you want it to vanish too.
    }
}

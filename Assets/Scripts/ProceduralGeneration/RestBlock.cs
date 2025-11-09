using UnityEngine;

public class RestBlock : BlockBase
{
    // On safe zones we don't schedule row despawn on step.
    protected override void OnStepped(Collider player)
    {
        owningRow?.OnBlockStepped(this);
        owningRow?.generator?.SetSafeMode(true);
        // Keeps the platform safe.
    }
}

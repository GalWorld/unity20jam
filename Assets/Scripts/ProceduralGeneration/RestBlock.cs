using UnityEngine;

public class RestBlock : BlockBase
{
    // On safe zones we don't schedule row despawn on step.
    protected override void OnStepped(Collider player)
    {
        // Optionally: play VFX/SFX, grant small heal, etc.
        // Do NOT call owningRow.ScheduleDespawnAll(...)
        // Keeps the platform safe.
    }
}

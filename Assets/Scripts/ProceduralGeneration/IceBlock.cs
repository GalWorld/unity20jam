using UnityEngine;

/// <summary>
/// Ice block: adds slip flag (your controller reads it) or quickens despawn.
/// </summary>
public class IceBlock : BlockBase
{
    [Header("Ice Tweaks")]
    public float extraQuickDespawn = -0.4f; // shorter delay when stepped

    protected override void OnStepped(Collider player)
    {
        // Example: reduce delay for row crumble on ice.
        if (owningRow != null)
            owningRow.ScheduleDespawnAll(Mathf.Max(0.1f, crumbleDelayOnStep + extraQuickDespawn));
        base.OnStepped(player);
    }
}
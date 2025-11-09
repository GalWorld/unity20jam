using UnityEngine;

/// <summary>
/// Ice block: adds slip flag (your controller reads it) or quickens despawn.
/// </summary>
public class IceBlock : BlockBase
{
    protected override void OnStepped(Collider player)
    {
        if (owningRow != null)
            owningRow.ScheduleDespawnAll(Mathf.Max(0.1f, crumbleDelayOnStep));
        base.OnStepped(player);
    }
}
using UnityEngine;

public class SpawnBlock : BlockBase
{
    [Header("Spawn settings")]
    public bool consumeOnStep = true;

    protected override void OnStepped(Collider player)
    {
        if (owningRow != null)
            owningRow.ScheduleDespawnAll(Mathf.Max(0.1f, crumbleDelayOnStep));
        base.OnStepped(player);
    }
}

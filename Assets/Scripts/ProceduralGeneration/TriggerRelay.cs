using UnityEngine;

/// <summary>
/// Relays trigger events up to the parent BlockBase.
/// </summary>
public class TriggerRelay : MonoBehaviour
{
    private BlockBase _parent;

    public void Init(BlockBase parent) => _parent = parent;

    private void OnTriggerEnter(Collider other) => _parent?.OnChildTriggerEnter(other);
}

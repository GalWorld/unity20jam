using UnityEngine;

[CreateAssetMenu(menuName = "Climber/Block Type", fileName = "BlockTypeDef")]
public class BlockTypeDefSO : ScriptableObject
{
    [Tooltip("Unique ID used by generators / JSON. e.g. normal, ice, trap")]
    public string id = "normal";

    [Tooltip("Prefab to instantiate for this block type.")]
    public GameObject prefab;

    [Header("Gameplay")]
    [Tooltip("If true, player slips on this block.")]
    public bool isSlippery;

    [Tooltip("If true, block behaves as a trap (spikes, damage, crumble, etc.).")]
    public bool isTrap;
}

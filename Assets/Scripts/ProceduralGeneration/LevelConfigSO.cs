using UnityEngine;

[CreateAssetMenu(menuName = "Climber/Level Config", fileName = "LevelConfig")]
public class LevelConfigSO : ScriptableObject
{
    [Tooltip("Seed for deterministic row-by-row generation.")]
    public int seed = 12345;
    [Header("Grid / Row Settings")]
    [Tooltip("Number of columns per row.")]
    public int width = 7;
    [Tooltip("World X spacing between columns.")]
    public float columnWidth = 1.0f;
    [Tooltip("How much each row rises vertically (Y) compared to the previous row.")]
    public float stepRiseY = 1.0f;

    [Tooltip("How much each row goes back (Z) compared to the previous row.")]
    public float stepRunZ = 0.8f;

    [Tooltip("Optional lateral nudge per row to emphasize the zig-zag feel. 0 = disabled.")]
    public float stepLateralNudgeX = 0f;

    [Header("Densities (0..1)")]
    [Range(0f, 1f)] public float trapDensity = 0.1f;
    [Range(0f, 1f)] public float iceDensity = 0.15f;

    [Header("Stair / Fill Policy")]
    [Range(0f, 1f)] public float globalFillChance = 0.45f;   
    [Range(0f, 1f)] public float keepClearRadius = 1f;        // cells near target kept mostly clear
    [Range(0f, 1f)] public float wiggleSideChance = 0.55f;    // chance that targetX moves ±1 instead of 0
    [Range(0, 6)] public int maxFillPerRow = 3;           // hard cap of extra blocks per row
    [Range(0f, 1f)] public float pillarPenalty = 0.6f;        // reduce fill chance if block below (avoids pillars)
    [Range(0f, 1f)] public float holeBias = 0.15f;            // pushes to leave additional holes so it doesn't look like a wall

    [Header("Path Policy")]
    [Tooltip("Guarantee at least one climbable position per next row (one-step climb).")]
    public bool guaranteeClimbStep = true;

    [Tooltip("Optional: create small plateaus every N rows (-1 to disable).")]
    public int restPlatformEveryNRows = -1;
}

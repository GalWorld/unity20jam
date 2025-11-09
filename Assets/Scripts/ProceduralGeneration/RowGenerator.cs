using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class RowGenerator : MonoBehaviour
{
    [Header("Authoring")]
    [SerializeField] private LevelConfigSO config;
    [SerializeField] private Transform rowsRoot;
    [SerializeField] private PoolManager pool;                

    [Header("Spawn Loop")]
    [SerializeField] private int bootstrapRows = 12;
    [SerializeField] private float spawnEverySeconds = 0.35f;

    [Header("Densities (0..1)")]
    [Range(0f, 1f)] [SerializeField] private float spawnDensity = 0.08f; 
    [Range(0f, 1f)] [SerializeField] private float iceDensity = 0.15f;


    [Header("Prefabs (by type id)")]
    [SerializeField] private GameObject normalPrefab;
    [SerializeField] private GameObject icePrefab;
    [SerializeField] private GameObject restPrefab;   
    [SerializeField] private GameObject spawnPrefab;

    private Random rng;
    private float _cellSizeX = 1f;
    private bool[] lastRow;
    private int currentRowIndex = -1;
    private int _prevTargetX = -1;


    // Active rows for recycling
    private readonly LinkedList<RowContainer> _activeRows = new();

    private void Awake()
    {
        rng = new Random(config.seed);

        lastRow = new bool[config.width];
        for (int x = 0; x < config.width; x++) lastRow[x] = true; // ground row
        currentRowIndex = -1;

        _cellSizeX = Mathf.Max(
            config.columnWidth,
            GetPrefabWidth(normalPrefab),
            GetPrefabWidth(icePrefab),
            GetPrefabWidth(restPrefab),
            GetPrefabWidth(spawnPrefab)
        );
    }

    private void Start()
    {
        for (int i = 0; i < bootstrapRows; i++) GenerateNextRow();
        StartCoroutine(SpawnLoop());
    }

    private System.Collections.IEnumerator SpawnLoop()
    {
        var wait = new WaitForSeconds(spawnEverySeconds);
        while (true)
        {
            GenerateNextRow();
            yield return wait;
        }
    }

    public void GenerateNextRow()
    {
        currentRowIndex++;

        var row = GetRowContainer(currentRowIndex);

        int targetX = ChooseTargetColumn(lastRow, rng, _prevTargetX);
        _prevTargetX = targetX;
        SpawnBlock(row, targetX, currentRowIndex, "normal");

        if (config.restPlatformEveryNRows > 0 &&
            currentRowIndex > 0 &&
            currentRowIndex % config.restPlatformEveryNRows == 0)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                int x = Mathf.Clamp(targetX + dx, 0, config.width - 1);
                if (!HasBlockAt(row, x)) SpawnBlock(row, x, currentRowIndex, "rest");
            }
        }

        int fills = 0;
        for (int x = 0; x < config.width; x++)
        {
            if (HasBlockAt(row, x)) continue;
            if (fills >= config.maxFillPerRow) break;

            if (ShouldSpawnHere(x, targetX, lastRow))
            {
                string t = RollTypeId(rng, spawnDensity, iceDensity);
                SpawnBlock(row, x, currentRowIndex, t);
                fills++;
            }
        }

        lastRow = new bool[config.width];
        foreach (var b in row.blocks)
        {
            if (b == null) continue;
            int cx = XFromName(b.name);
            if (cx >= 0 && cx < lastRow.Length) lastRow[cx] = true;
        }
    }

    private bool ShouldSpawnHere(int x, int targetX, bool[] prevRow)
    {
        if (Mathf.Abs(x - targetX) <= config.keepClearRadius)
        {
            return UnityEngine.Random.value < 0.15f;
        }

        float chance = config.globalFillChance * (1f - config.holeBias);

        if (prevRow != null && x >= 0 && x < prevRow.Length && prevRow[x])
        {
            chance *= (1f - config.pillarPenalty); 
        }

        if (x == 0 || x == prevRow.Length - 1)
        {
            chance *= 0.75f;
        }

        return UnityEngine.Random.value < chance;
    }


    // ---------- helpers ----------
    private RowContainer GetRowContainer(int rowIndex)
    {
        // Try reuse a disabled container
        RowContainer reused = null;
        foreach (var node in _activeRows)
        {
            if (!node.gameObject.activeSelf)
            {
                reused = node;
                break;
            }
        }

        if (reused == null)
        {
            var go = new GameObject($"Row_{rowIndex}");
            go.transform.SetParent(rowsRoot, false);
            reused = go.AddComponent<RowContainer>();
            _activeRows.AddLast(reused);
        }

        reused.rowIndex = rowIndex;
        reused.gameObject.SetActive(true);
        reused.blocks.Clear();
        float y = rowIndex * config.stepRiseY;   
        float z = rowIndex * config.stepRunZ;    

        float x = 0f;
        if (Mathf.Abs(config.stepLateralNudgeX) > 0.0001f)
        {
            x = ((rowIndex % 2 == 0) ? -1f : 1f) * config.stepLateralNudgeX * rowIndex;
        }

        reused.transform.localPosition = new Vector3(x, y, z); 
        return reused;
    }

    private void SpawnBlock(RowContainer row, int x, int y, string typeId)
    {
        GameObject prefab = typeId switch
        {
            "ice" => icePrefab != null ? icePrefab : normalPrefab,
            "rest" => restPrefab != null ? restPrefab : normalPrefab,
            "spawn" => spawnPrefab != null ? spawnPrefab : normalPrefab,
            _ => normalPrefab
        };

        Vector3 localPos = new Vector3(x * _cellSizeX, 0f, 0f);
        var go = pool.Spawn(prefab, row.transform.TransformPoint(localPos), Quaternion.identity, row.transform);
        go.name = $"{typeId}_r{y}_x{x}";
        go.transform.localPosition = localPos;

        var block = go.GetComponent<BlockBase>() ?? go.AddComponent<BlockBase>();
        block.pool = pool;
        block.sourcePrefab = prefab;
        block.owningRow = row;

        row.Register(block);
    }

    private bool HasBlockAt(RowContainer row, int x)
    {
        // We encoded x in name; alternatively, store x in a custom component field
        foreach (var b in row.blocks)
        {
            if (b == null) continue;
            if (b.name.EndsWith($"_x{x}")) return true;
        }
        return false;
    }

    private int ChooseTargetColumn(bool[] prevRow, Random r, int prevTargetX)
    {
        var candidates = new List<int>();

        if (prevTargetX >= 0 && config.wiggleSideChance > 0f && r.NextDouble() < config.wiggleSideChance)
        {
            if (prevTargetX - 1 >= 0 && prevRow[prevTargetX - 1]) candidates.Add(prevTargetX - 1);
            if (prevTargetX + 1 < prevRow.Length && prevRow[prevTargetX + 1]) candidates.Add(prevTargetX + 1);
        }

        for (int x = 0; x < prevRow.Length; x++)
        {
            if (!prevRow[x]) continue;
            if (!candidates.Contains(x)) candidates.Add(x);
            if (x - 1 >= 0 && !candidates.Contains(x - 1)) candidates.Add(x - 1);
            if (x + 1 < prevRow.Length && !candidates.Contains(x + 1)) candidates.Add(x + 1);
        }

        if (candidates.Count == 0) return Mathf.FloorToInt(prevRow.Length / 2f);
        return candidates[r.Next(0, candidates.Count)];
    }

    private string RollTypeId(Random r, float damageD, float iceD)
    {
        double roll = r.NextDouble();
        if (roll < damageD) return "damage";
        if (roll < damageD + iceD) return "ice";
        return "normal";
    }

    private int XFromName(string n)
    {
        // parse trailing _xN (quick & dirty; replace by dedicated field later)
        int idx = n.LastIndexOf("_x", StringComparison.Ordinal);
        if (idx < 0) return -1;
        if (int.TryParse(n[(idx + 2)..], out int x)) return x;
        return -1;
    }

    private float GetPrefabWidth(GameObject prefab)
    {
        if (prefab == null) return 0f;

        // Instantiate temporarily (inactive), measure renderers, then destroy.
        var temp = Instantiate(prefab);
        temp.SetActive(false);

        var renderers = temp.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
        {
            Destroy(temp);
            return 0f;
        }

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);

        float width = b.size.x;
        Destroy(temp);
        return width;
    }
}

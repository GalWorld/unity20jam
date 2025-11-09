using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all block instances of a single row. Can despawn entire row after a delay.
/// </summary>
public class RowContainer : MonoBehaviour
{
    public int rowIndex;
    public RowGenerator generator;
    public bool isRestRow;

    [Header("Score")]
    public int rowPoints = 10;           // points awarded when row is first stepped
    public IScoreSink scoreSink;         // set by RowGenerator
    private bool _rowScored = false;

    public readonly List<BlockBase> blocks = new();

    private bool _despawnScheduled;
    private Coroutine _co;

    public void Register(BlockBase b)
    {
        if (!blocks.Contains(b)) blocks.Add(b);
        b.owningRow = this;
    }

    public void OnBlockStepped(BlockBase source)
    {
        // Score once per row (first time any block in this row is stepped)
        if (!_rowScored)
        {
            _rowScored = true;
            scoreSink?.AddScore(rowPoints, this, source);
            generator?.SetPlayerRow(rowIndex);
        }
    }

    public void ScheduleDespawnAll(float delay)
    {
        if (isRestRow) return;
        if (_despawnScheduled) return;
        _despawnScheduled = true;
        _co = StartCoroutine(DespawnAllAfter(delay));
    }

    private IEnumerator DespawnAllAfter(float delay)
    {
        yield return new WaitForSeconds(delay); // scaled time

        // Return all blocks to pool
        foreach (var b in blocks)
        {
            if (b != null) b.ReturnToPool();
        }
        blocks.Clear();

        // Also disable this container 
        // gameObject.SetActive(false);
    }
}

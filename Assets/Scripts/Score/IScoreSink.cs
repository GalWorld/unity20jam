using UnityEngine;

public interface IScoreSink
{
    // Called when the player advances (first step on a row).
    void AddScore(int points, RowContainer row, BlockBase source);
}

using System;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour, IScoreSink
{
    [Header("Scoring")]
    [SerializeField] private TMP_Text scoreText; 
    [SerializeField] private int score = 0;

    public void AddScore(int points, RowContainer row, BlockBase source)
    {
        score += points;
        if (scoreText != null) scoreText.text = score.ToString();
    }
}

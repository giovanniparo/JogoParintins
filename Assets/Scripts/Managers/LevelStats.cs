using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStats
{
    int numMiss;
    int numSlow;
    int numFast;
    int numGood;
    int numExcelent;
    int score;

    public LevelStats()
    {
        numMiss = 0;
        numSlow = 0;
        numFast = 0;
        numGood = 0;
        numExcelent = 0;
        score = 0;
    }

    public void IncLevelStats(string type)
    {
        switch(type)
        {
            case "miss":
                numMiss++;
                break;
            case "slow":
                numSlow++;
                break;
            case "fast":
                numFast++;
                break;
            case "good":
                numGood++;
                break;
            case "exc":
                numExcelent++;
                break;
            default:
                Debug.LogWarning("LevelStats type not defined!");
                break;
        }
    }

    public void UpdateScore(int score)
    {
        this.score = score;
    }
}

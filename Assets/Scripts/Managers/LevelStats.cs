using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LevelData
{
    public string name;
    public int music;
    public int team;
    public int numMiss;
    public int numSlow;
    public int numFast;
    public int numGood;
    public int numExcelent;
    public int score;
    public int maxChainSize;
    public int numOfLevelHits;
    public int grade;
    public int idSession;
}

[System.Serializable]
public class LevelStats
{
    public LevelData levelData;

    public LevelStats(string name, int music, int team, int idSession)
    {
        levelData.name = name;
        levelData.music = music;
        levelData.team = team;
        levelData.numMiss = 0;
        levelData.numSlow = 0;
        levelData.numFast = 0;
        levelData.numGood = 0;
        levelData.numExcelent = 0;
        levelData.score = 0;
        levelData.maxChainSize = 0;
        levelData.numOfLevelHits = 0;
        levelData.grade = 0;
        levelData.idSession = idSession;
    }

    public void IncLevelStats(string type)
    {
        switch(type)
        {
            case "miss":
                levelData.numMiss++;
                break;
            case "slow":
                levelData.numSlow++;
                break;
            case "fast":
                levelData.numFast++;
                break;
            case "good":
                levelData.numGood++;
                break;
            case "exc":
                levelData.numExcelent++;
                break;
            default:
                Debug.LogWarning("LevelStats type not defined!");
                break;
        }

        levelData.numOfLevelHits++;
    }

    public int GetLevelStats(string type)
    {
        switch (type)
        {
            case "miss":
                return levelData.numMiss;
            case "slow":
                return levelData.numSlow;
            case "fast":
                return levelData.numFast;
            case "good":
                return levelData.numGood;
            case "exc":
                return levelData.numExcelent;
            case "score":
                return levelData.score;
            case "chain":
                return levelData.maxChainSize;
            default:
                Debug.LogWarning("LevelStats type not defined!");
                break;
        }

        Debug.LogWarning("Could not get levelStats for: " + type + " data");
        return 0;
    }

    public void UpdateScore(int score)
    {
        this.levelData.score = score;
    }

    public void UpdateMaxChain(int chain)
    {
        this.levelData.maxChainSize = chain;
    }

    public int GetLevelGrade()
    {
        int grade = 0;

        grade = (int)(((levelData.numGood + levelData.numExcelent) / (float)levelData.numOfLevelHits) * 100);
        levelData.grade = grade;

        return grade;
    }
}

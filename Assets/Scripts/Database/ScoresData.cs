using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoresData
{
    public List<LevelData> scoreData;

    public ScoresData()
    {
        scoreData = new List<LevelData>();
    }
}

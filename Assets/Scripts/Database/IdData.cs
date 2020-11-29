using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IdData
{
    public bool error;
    public string message;
    public int id;
    public int music;
    public int team;
    public int[] grades = new int[3];

    public IdData()
    {
        error = true;
        message = "";
        id = 0;
        music = 4;
        team = 2;
        grades[0] = -1;
        grades[1] = -1;
        grades[2] = -1;
    }

    public void Clear()
    {
        error = true;
        message = "";
        id = 0;
        music = 4;
        team = 2;
        grades[0] = -1;
        grades[1] = -1;
        grades[2] = -1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JogadorLoginData
{
    public bool error;
    public string message;
    public string name;
    public string music;
    public int team;

    public JogadorLoginData()
    {
        error = true;
        message = "";
        name = "";
        music = "";
        team = 0;
    }
}

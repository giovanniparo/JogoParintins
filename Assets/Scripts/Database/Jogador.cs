﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JogadorLoginData
{
    public bool error;
    public string message;
    public string name;

    public JogadorLoginData()
    {
        error = true;
        message = "";
        name = "";
    }

    public void Clear()
    {
        error = true;
        message = "";
        name = "";
    }
}

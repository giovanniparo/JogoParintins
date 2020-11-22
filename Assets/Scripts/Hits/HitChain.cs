using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitChain
{
    public Vector2 meanPosition;
    public float spawnTime;
    public Queue<GameObject> hits;

    public HitChain(float spawnTime, float xPos, float yPos)
    {
        this.meanPosition.x = xPos;
        this.meanPosition.y = yPos;
        this.spawnTime = spawnTime;
        hits = new Queue<GameObject>();
    }
}

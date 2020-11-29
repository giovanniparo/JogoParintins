using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emote : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(this.gameObject, 3.0f);
    }
}

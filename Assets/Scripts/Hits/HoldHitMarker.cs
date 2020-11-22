using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldHitMarker : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Resize()
    {
        if (transform.localScale.x >= 0.0f)
        {
            transform.localScale -= new Vector3(speed * Time.deltaTime, speed * Time.deltaTime, 0);
        }
    }

    private void Update()
    {
        Resize();
    }

    public float GetSpeed()
    {
        return speed;
    }
}

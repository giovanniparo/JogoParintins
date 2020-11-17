using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CircleHitMarker : MonoBehaviour
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
}

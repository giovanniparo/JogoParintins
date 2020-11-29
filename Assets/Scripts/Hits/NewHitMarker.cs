using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHitMarker : MonoBehaviour
{
    public float moveSpeed;
    Transform parent;
    Vector3 moveDirection;

    private void Awake()
    {
        parent = this.transform.parent;
        moveDirection = (parent.position - transform.position).normalized;
    }

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}

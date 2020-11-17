using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnector : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public float width = 0.2f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Vector3[] initLinePos = new Vector3[2] { Vector3.zero, Vector3.zero };
        lineRenderer.SetPositions(initLinePos);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
    }

    public void CreateLine(Vector3 startPosition, Vector3 targetPosition)
    {
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, targetPosition);
        lineRenderer.enabled = true;
    }

    public void SetLineColor(Color startColor, Color endColor)
    {
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
    }

    public void DestroyLine()
    {
        Destroy(this.gameObject);
    }

}

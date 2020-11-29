using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHit : MonoBehaviour
{
    [SerializeField] GameObject marker;
    
    [SerializeField] private GameObject hitArrowPrefab;
    [SerializeField] private float arrowRadius;

    private int numOfArrows = 4;

    void Awake()
    { 
        CreateArrows();
    }

    private void Update()
    {
        if (marker.transform.localPosition.magnitude <= 0.01f)
            Destroy(this.gameObject);
    }

    public Vector3 GetCirclePoint(int arrowIndex)
    {
        Vector3 result;
        
        float angleInc = 2 * Mathf.PI / numOfArrows;

        result.x = arrowRadius * Mathf.Cos(Mathf.PI / numOfArrows + arrowIndex * angleInc);
        result.y = arrowRadius * Mathf.Sin(Mathf.PI / numOfArrows + arrowIndex * angleInc);
        result.z = 0.0f;

        return result;
    }

    public void CreateArrows()
    {
        for (int n = 0; n < numOfArrows; n++)
        {
            GameObject currentArrow = Instantiate(hitArrowPrefab, transform.position, Quaternion.identity);
            currentArrow.transform.SetParent(this.gameObject.transform);
            currentArrow.transform.localPosition = GetCirclePoint(n);
            currentArrow.transform.LookAt(transform.position);
            currentArrow.transform.right = currentArrow.transform.forward;
        }
    }
}

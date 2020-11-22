using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldHit : Hit
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lineWidth;
    [SerializeField] private GameObject holdHitMarker;
    [SerializeField] private GameObject holdHitStepPrefab;
    private LineRenderer lineRenderer;
    private Collider2D markerCollider;
    private Collider2D holdCollider;

    private Queue<GameObject> stepsQueue;

    private int numOfSteps;
    private int gotStepsCounter = 0;
    private float length;
    private float ratio;
    private Vector2 moveDirection;
    private Vector2 mousePos;
    private Vector2 endPos;
    private RaycastHit2D hit;

    [HideInInspector] public float travelTime;

    public override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
        holdCollider = GetComponent<Collider2D>();
        markerCollider = holdHitMarker.GetComponent<Collider2D>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        stepsQueue = new Queue<GameObject>();
    }

    private void Update()
    {
        if(holdHitMarker.activeSelf)
            ratio = markerCollider.bounds.extents.magnitude / holdCollider.bounds.extents.magnitude;

        if(ratio <= 1.0f)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(mousePos, Vector2.zero);

            holdHitMarker.SetActive(false);
            if (stepsQueue.Count > 0) stepsQueue.Peek().SetActive(true);
            transform.position += new Vector3(moveDirection.normalized.x, moveDirection.normalized.y, 0.0f) * moveSpeed * Time.deltaTime;
            
            //if(stepsQueue.Count > 0 && (stepsQueue.Peek().transform.position - transform.position).magnitude <= 0.001f)
            //{
                //Destroy(stepsQueue.Dequeue());
                if(hit.collider != null && Input.GetMouseButton(0) && hit.collider.gameObject.GetComponent<HoldHit>() == this)
                {
                    gotStepsCounter++;
                    FeedbackManager.instance.IncChainCounter();
                }
            //}

            if (((Vector2)transform.position - endPos).magnitude <= 0.01f)
            {
                GiveFeedback();
            }
        }
    }

    private void GiveFeedback()
    {
        if (gotStepsCounter == numOfSteps)
            FeedbackManager.instance.ExcelentFeedback();
        else if (gotStepsCounter >= numOfSteps - 2)
            FeedbackManager.instance.GoodFeedback();
        else
            FeedbackManager.instance.MissFeedback();
        Destroy(this.gameObject);
    }

    public override void Interact()
    {
        
    }

    public void SetHoldHitProps(Vector2 startPos, Vector2 endPos, Color color)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        moveDirection = endPos - startPos;
        length = moveDirection.magnitude;
        travelTime = length / moveSpeed;
        this.endPos = endPos;
    }
}

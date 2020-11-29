using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleHit : Hit
{
    public GameObject circleHitMarker;
    public GameObject numberHolder;
    [SerializeField] private float speed;

    private float ratio;
    private Collider2D circleColl;
    private Collider2D markerColl;

    private bool excelent = false;
    private bool done = false;
    private bool timed = false;
    private float timer = 0.0f;

    public override void Awake()
    {
        base.Awake();
        circleColl = GetComponent<Collider2D>();
        markerColl = circleHitMarker.GetComponent<Collider2D>();
    }

    public override void Interact()
    {
        if (ratio > 1.00f)
        {
            FeedbackManager.instance.FastFeedback();
            DestroyThis();
        } 
        else if (ratio >= 0.90f)
        {
            FeedbackManager.instance.ExcelentFeedback();
            GetComponent<SpriteRenderer>().enabled = false;
            circleHitMarker.GetComponent<SpriteRenderer>().enabled = false;
            numberHolder.SetActive(false);
            excelent = true;
        }
        else if (ratio >= 0.85f)
        {
            FeedbackManager.instance.GoodFeedback();
            AudioManager.instance.PlayHitSound();
            DestroyThis();
        }
        else
        {
            FeedbackManager.instance.SlowFeedback();
            DestroyThis();
        }
    }

    public void DestroyThis()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }

    private void Update()
    {
        ResizeMarker();
        ratio = markerColl.bounds.size.magnitude / circleColl.bounds.size.magnitude;
        timer += (Time.deltaTime);
        if (ratio - 0.9f <= 0.001f && !timed)
        {
            timed = true;
            Debug.Log(timer);
        }

        if (excelent && ratio - 0.9f <= 0.001f)
        {
            AudioManager.instance.PlayHitSound();
            DestroyThis();
        }

        if (ratio > 1.00f)
            circleHitMarker.GetComponent<SpriteRenderer>().color = Color.white;
        else if (ratio - 0.9f <= 0.001f)
            circleHitMarker.GetComponent<SpriteRenderer>().color = Color.green;
        else if (ratio - 0.85f <= 0.001f)
            circleHitMarker.GetComponent<SpriteRenderer>().color = Color.blue;
        else
            circleHitMarker.GetComponent<SpriteRenderer>().color = Color.red;

        if (ratio - 0.8f <= 0.001f)
        {
            GameManager.instance.Missed();
            FeedbackManager.instance.MissFeedback();
            DestroyThis();
        }
    }

    private void ResizeMarker()
    {
        if (circleHitMarker.transform.localScale.x >= 0.0f)
        {
            circleHitMarker.transform.localScale -= new Vector3(speed * (Time.deltaTime), speed * (Time.deltaTime), 0);
        }
    }
}

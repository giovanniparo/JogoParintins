using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleHit : Hit
{
    public GameObject circleHitMarker;
    public GameObject numberHolder;

    private float ratio;
    private Collider2D circleColl;
    private Collider2D markerColl;

    private bool excelent = false;
    private bool done = false;
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
            Destroy(this.gameObject);
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
            Destroy(this.gameObject);
        }
        else
        {
            FeedbackManager.instance.SlowFeedback();
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        ratio = markerColl.bounds.extents.magnitude / circleColl.bounds.extents.magnitude;
        
        if(excelent && ratio <= 0.9f)
        {
            AudioManager.instance.PlayHitSound();
            Destroy(this.gameObject);
        }
        
        /*timer += Time.deltaTime;
        if (ratio < 0.9f && !done)
        {
            done = true;
            Debug.Log("Timer: " + timer);

        }*/

        if (ratio > 1.00f)
            circleHitMarker.GetComponent<SpriteRenderer>().color = Color.white;
        else if(ratio >= 0.90f)
            circleHitMarker.GetComponent<SpriteRenderer>().color = Color.green;
        else if(ratio >= 0.85f)
            circleHitMarker.GetComponent<SpriteRenderer>().color = Color.blue;
        else
            circleHitMarker.GetComponent<SpriteRenderer>().color = Color.red;

        if (ratio <= 0.01f)
        {
            GameManager.instance.Missed();
            FeedbackManager.instance.MissFeedback();
            Destroy(this.gameObject);
        }
    }
}

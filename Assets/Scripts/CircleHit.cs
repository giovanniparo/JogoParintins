using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleHit : Hit
{
    [SerializeField] private SpriteRenderer numberSpriteRenderer;
    [SerializeField] private GameObject circleHitMarker;

    public SpriteRenderer spriteRenderer;

    private float ratio;
    private Collider2D circleColl;
    private Collider2D markerColl;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleColl = GetComponent<Collider2D>();
        markerColl = circleHitMarker.GetComponent<Collider2D>();
    }
    
    public void SetProperties(Sprite numSprite, Color color)
    {
        numberSpriteRenderer.sprite = numSprite;
        spriteRenderer.color = color;
    }

    public override void Interact()
    {
        if (ratio > 1.0f)
            Debug.Log("TOO SOON!");
        else if (ratio >= 0.8f)
            Debug.Log("EXCELLENT!");
        else
            Debug.Log("SLOW!");
        Destroy(this.gameObject);
    }

    private void Update()
    {
        ratio = markerColl.bounds.extents.magnitude / circleColl.bounds.extents.magnitude;
        if (ratio <= 0.01f)
        {
            GameManager.instance.Missed();
            Destroy(this.gameObject);
        }
    }
}

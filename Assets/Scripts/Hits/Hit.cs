using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hit : MonoBehaviour
{
    [SerializeField] private SpriteRenderer numberSpriteRenderer;
    private SpriteRenderer spriteRenderer;

    public float time;
    public int chainID;

    public virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public abstract void Interact();

    public void SetProperties(float time, Sprite numSprite, Color color, int chainID)
    {
        this.time = time;
        this.chainID = chainID;
        numberSpriteRenderer.sprite = numSprite;
        spriteRenderer.color = color;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArrow : MonoBehaviour
{
    public int animFrameRate;
    Transform parent;
    Animation anim;
    AnimationCurve curveX, curveY;
    AnimationClip clip;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        clip = new AnimationClip();
    }

    private void Start()
    {
        parent = transform.parent;
        Collider2D parentColl = parent.gameObject.GetComponent<Collider2D>();
        clip.legacy = true;
        clip.frameRate = animFrameRate;
        clip.wrapMode = WrapMode.Loop;

        Keyframe[] keysX;
        Keyframe[] keysY;
        keysX = new Keyframe[2];
        keysY = new Keyframe[2];
        keysX[0] = new Keyframe(0.0f, transform.localPosition.x);
        keysY[0] = new Keyframe(0.0f, transform.localPosition.y);
        keysX[1] = new Keyframe(1.0f, transform.localPosition.x - Mathf.Sign(transform.localPosition.x) * parentColl.bounds.extents.x);
        keysY[1] = new Keyframe(1.0f, transform.localPosition.y - Mathf.Sign(transform.localPosition.y) * parentColl.bounds.extents.y);

        curveX = new AnimationCurve(keysX);
        curveY = new AnimationCurve(keysY);

        clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
        clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);

        anim.AddClip(clip, clip.name);
        anim.Play(clip.name);
    }
}

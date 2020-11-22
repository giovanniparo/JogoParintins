using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScorePlusText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Animator animator;

    private Vector3 initScale;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        animator = GetComponent<Animator>();
        initScale = transform.localScale;
    }

    public void UpdateText(string score)
    {
        text.text = score;
        animator.SetBool("updating", true);
    }

    public void ResetText()
    {
        text.text = "";
        animator.SetBool("updating", false);
        transform.localScale = initScale;
        FeedbackManager.instance.FinishPlusScoreAnimation();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChainText : MonoBehaviour
{
    private ParticleSystem partSys;
    [SerializeField] private int incChainChange;

    private TextMeshProUGUI text;
    private Animator animator;

    private Vector3 initScale;

    private int currChainSize = 0;
    private int prevChainSize = 0;

    private bool change = false;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        animator = GetComponent<Animator>();
        partSys = GetComponent<ParticleSystem>();
        initScale = transform.localScale;
    }
    private void Update()
    {
        currChainSize = int.Parse(text.text);
    }

    private void Reset()
    {
        transform.localScale = initScale;
        animator.SetBool("chain", false);
    }
}

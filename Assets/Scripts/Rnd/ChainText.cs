using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChainText : MonoBehaviour
{
    [SerializeField] private GameObject[] chainFXs;
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
        initScale = transform.localScale;
    }

    public void UpdateFX()
    {
        int flipPos;

        if (currChainSize == 0)
            Reset();
        else
        {
            if (currChainSize <= incChainChange) flipPos = 0;
            else if (currChainSize <= 2 * incChainChange) flipPos = 1;
            else if (currChainSize <= 3 * incChainChange) flipPos = 2;
            else flipPos = 3;

            for (int n = 0; n < chainFXs.Length; n++)
            {
                if (n == flipPos && chainFXs[n].activeSelf == false)
                {
                    chainFXs[n].SetActive(true);
                    chainFXs[n].GetComponent<ParticleSystem>().Play();
                }
                else if(n != flipPos)
                {
                    chainFXs[n].SetActive(false);
                }
            }

            transform.localScale *= 1.25f;
            animator.SetBool("chain", true);
        }
    }

    private void Update()
    {
        currChainSize = int.Parse(text.text);
        
        UpdateFX();
    }

    private void Reset()
    {
        foreach(GameObject fxs in chainFXs)
        {
            fxs.SetActive(false);
        }

        transform.localScale = initScale;
        animator.SetBool("chain", false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeedbackManager : MonoBehaviour
{
    [SerializeField] private Transform feedbackBubblePosition;
    [SerializeField] private GameObject[] feedbackBubblePrefabs;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI chainText;
    [SerializeField] private TextMeshProUGUI scorePlusText;
    [SerializeField] private GameObject startText;

    private Animator scoreTextAnimator;
    private Animator chainTextAnimator;

    public int goodScoreValue;
    public int excelentScoreValue;
    public int chainMultiplier;

    private GameObject currentFeedbackBubble;
    private int chainCounter;
    private int score;
    private int currentScore;

    public static FeedbackManager instance;

    private bool isUpdateScore = false;
    private bool isUpdateChain = false;

    private Vector3 initScale;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Destroy(this);
            Debug.LogError("More than one instance of FeedbackManager Script was found!");
        }

        scoreTextAnimator = scoreText.gameObject.GetComponent<Animator>();
        chainTextAnimator = chainText.gameObject.GetComponent<Animator>();

        initScale = transform.localScale;
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        chainText.text = chainCounter.ToString();
    }

    public void ActivateStartText()
    {
        startText.SetActive(true);
    }


    public void MissFeedback()
    {
        if (currentFeedbackBubble != null)
            Destroy(currentFeedbackBubble);
        currentFeedbackBubble = Instantiate(feedbackBubblePrefabs[0], feedbackBubblePosition.position, Quaternion.identity);
        chainCounter = 0;
    }

    public void SlowFeedback()
    {
        if (currentFeedbackBubble != null)
            Destroy(currentFeedbackBubble);
        currentFeedbackBubble = Instantiate(feedbackBubblePrefabs[1], feedbackBubblePosition.position, Quaternion.identity);
        chainCounter = 0;
    }

    public void FastFeedback()
    {
        if (currentFeedbackBubble != null)
            Destroy(currentFeedbackBubble);
        currentFeedbackBubble = Instantiate(feedbackBubblePrefabs[2], feedbackBubblePosition.position, Quaternion.identity);
        chainCounter = 0;
    }

    public void GoodFeedback()
    {
        if (currentFeedbackBubble != null)
            Destroy(currentFeedbackBubble);
        currentFeedbackBubble = Instantiate(feedbackBubblePrefabs[3], feedbackBubblePosition.position, Quaternion.identity);
        IncChainCounter();
        score += goodScoreValue + chainCounter * chainMultiplier;
        UpdatePlusScoreText((goodScoreValue + chainCounter * chainMultiplier).ToString());
    }

    public void ExcelentFeedback()
    {
        if (currentFeedbackBubble != null)
            Destroy(currentFeedbackBubble);
        currentFeedbackBubble = Instantiate(feedbackBubblePrefabs[4], feedbackBubblePosition.position, Quaternion.identity);
        IncChainCounter();
        score += excelentScoreValue + 2 * chainCounter * chainMultiplier;
        UpdatePlusScoreText((excelentScoreValue + 2 * chainCounter * chainMultiplier).ToString());
    }

    public void IncChainCounter()
    {
        chainCounter++;
    }
    
    public void UpdatePlusScoreText(string text)
    {
        if (scorePlusText.GetComponent<ScorePlusText>() != null)
            scorePlusText.GetComponent<ScorePlusText>().UpdateText(text.ToString());
    }
    
    public void FinishPlusScoreAnimation()
    {
        StartCoroutine("UpdateScoretextCoroutine");
    }

    IEnumerator UpdateScoretextCoroutine()
    {       
        while(currentScore != score)
        {
            scoreTextAnimator.SetBool("isUpdating", true);
            currentScore++;
            scoreText.text = currentScore.ToString();
            yield return null;
            scoreTextAnimator.SetBool("isUpdating", false);
        }
    }
}

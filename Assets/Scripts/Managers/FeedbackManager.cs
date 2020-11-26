using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum uiState
{
    PLAYING,
    PAUSED,
    FEEDBACK
}

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager instance;

    private GetPHP getPHP;

    [SerializeField] private Transform feedbackBubblePosition;
    [SerializeField] private GameObject[] feedbackBubblePrefabs;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI chainText;
    [SerializeField] private TextMeshProUGUI scorePlusText;
    [SerializeField] private GameObject startText;
    [SerializeField] private GameObject[] playingUIElements;
    [SerializeField] private GameObject feedbackUIElements;
    //[SerializeField] private GameObject[] pausedUIElements;
    [SerializeField] private TextMeshProUGUI feedbackWindowMusicName;
    [SerializeField] private TextMeshProUGUI feedbackWindowPlayerName;
    [SerializeField] private TextMeshProUGUI[] feedbackWindowHitCounters;
    [SerializeField] private TextMeshProUGUI[] feedbackWindowScoreCounters;
    [SerializeField] private TextMeshProUGUI feedbackWindowGradeText;
    [SerializeField] private TextMeshProUGUI feedbackWindowGradeSEAL;
    [SerializeField] private Button feedbackWindowNextButton;
    [SerializeField] private Button feedbackWindowSaveButton;

    private Animator scoreTextAnimator;
    private Animator chainTextAnimator;

    public int goodScoreValue;
    public int excelentScoreValue;
    public int chainMultiplier;

    private GameObject currentFeedbackBubble;
    private int chainCounter;
    private int maxChainCounter = 0;
    private int score;
    private int currentScore;
    int currentNumScore = 0;
    int currentNumChain = 0;

    private bool isUpdatingFeedbackScreen = false;

    private Vector3 initScale;
    private LevelStats levelStatsData;
    private uiState state;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Destroy(this);
            Debug.LogError("More than one instance of FeedbackManager Script was found!");
        }

        getPHP = GetComponent<GetPHP>();
        scoreTextAnimator = scoreText.gameObject.GetComponent<Animator>();
        chainTextAnimator = chainText.gameObject.GetComponent<Animator>();

        levelStatsData = new LevelStats(SceneLoader.instance.currentPlayerData.name, SceneLoader.instance.currentPlayerData.music);
        initScale = transform.localScale;
        state = uiState.PLAYING;
    }

    private void Update()
    {
        if (state == uiState.PLAYING)
        {
            UpdateUI();
        }
        else if (state == uiState.PAUSED)
        {
            /*TODO*/
        }
        else if (state == uiState.FEEDBACK && !isUpdatingFeedbackScreen)
        {
            isUpdatingFeedbackScreen = true;

            foreach (GameObject uiElement in playingUIElements)
                uiElement.SetActive(false);

            feedbackUIElements.SetActive(true);

            levelStatsData.UpdateScore(currentScore);
            levelStatsData.UpdateMaxChain(maxChainCounter);
            feedbackWindowMusicName.text = levelStatsData.levelData.music.ToUpper();
            feedbackWindowPlayerName.text = levelStatsData.levelData.name + " : " + System.DateTime.Now.Date.ToString();

            StartCoroutine("ShowFeedbackCoroutine");
        }
        else if (state == uiState.FEEDBACK)
        {
            /*TODO*/
        }
        else Debug.LogWarning("UI in a state not defined");
    }

    private void UpdateUI()
    {
        chainText.text = chainCounter.ToString();
    }

    public void ActivateStartText()
    {
        startText.SetActive(true);
    }

    public void GiveEndofLevelFeedback()
    {
        state = uiState.FEEDBACK;
    }

    public void MissFeedback()
    {
        if (currentFeedbackBubble != null)
            Destroy(currentFeedbackBubble);
        currentFeedbackBubble = Instantiate(feedbackBubblePrefabs[0], feedbackBubblePosition.position, Quaternion.identity);
        chainCounter = 0;
        levelStatsData.IncLevelStats("miss");
    }

    public void SlowFeedback()
    {
        if (currentFeedbackBubble != null)
            Destroy(currentFeedbackBubble);
        currentFeedbackBubble = Instantiate(feedbackBubblePrefabs[1], feedbackBubblePosition.position, Quaternion.identity);
        chainCounter = 0;
        levelStatsData.IncLevelStats("slow");
    }

    public void FastFeedback()
    {
        if (currentFeedbackBubble != null)
            Destroy(currentFeedbackBubble);
        currentFeedbackBubble = Instantiate(feedbackBubblePrefabs[2], feedbackBubblePosition.position, Quaternion.identity);
        chainCounter = 0;
        levelStatsData.IncLevelStats("fast");
    }

    public void GoodFeedback()
    {
        if (currentFeedbackBubble != null)
            Destroy(currentFeedbackBubble);
        currentFeedbackBubble = Instantiate(feedbackBubblePrefabs[3], feedbackBubblePosition.position, Quaternion.identity);
        IncChainCounter();
        score += goodScoreValue + chainCounter * chainMultiplier;
        UpdatePlusScoreText((goodScoreValue + chainCounter * chainMultiplier).ToString());
        levelStatsData.IncLevelStats("good");
    }

    public void ExcelentFeedback()
    {
        if (currentFeedbackBubble != null)
            Destroy(currentFeedbackBubble);
        currentFeedbackBubble = Instantiate(feedbackBubblePrefabs[4], feedbackBubblePosition.position, Quaternion.identity);
        IncChainCounter();
        score += excelentScoreValue + 2 * chainCounter * chainMultiplier;
        UpdatePlusScoreText((excelentScoreValue + 2 * chainCounter * chainMultiplier).ToString());
        levelStatsData.IncLevelStats("exc");
    }

    public void IncChainCounter()
    {
        chainCounter++;
        if (chainCounter > maxChainCounter)
            maxChainCounter = chainCounter;
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

    public void SaveButtonClicked()
    {
        getPHP.SaveMusicData(levelStatsData.levelData);
        getPHP.UpdatePlayerData(levelStatsData.levelData.name, levelStatsData.levelData.music, SceneLoader.instance.currentPlayerData.team);
    }

    public void NextButtonClicked()
    {
        SceneLoader.instance.LoadNextMusic();
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

    IEnumerator ShowFeedbackCoroutine()
    {
        bool showMiss = false;
        bool showSlow = false;
        bool showFast = false;
        bool showGood = false;
        bool showExc = false;
        bool showScore = false;
        bool showChain = false;

        if(!showMiss)
        {
            StartCoroutine("UpdateHitFeedbackScreenCoroutine", "miss");
            showMiss = true;
        }
        yield return new WaitForSeconds(0.5f);

        if (!showSlow)
        {
            StartCoroutine("UpdateHitFeedbackScreenCoroutine", "slow");
            showSlow = true;
        }
        yield return new WaitForSeconds(0.5f);

        if (!showFast)
        {
            StartCoroutine("UpdateHitFeedbackScreenCoroutine", "fast");
            showFast = true;
        }
        yield return new WaitForSeconds(0.5f);

        if (!showGood)
        {
            StartCoroutine("UpdateHitFeedbackScreenCoroutine", "good");
            showGood = true;
        }
        yield return new WaitForSeconds(0.5f);

        if (!showExc)
        {
            StartCoroutine("UpdateHitFeedbackScreenCoroutine", "exc");
            showExc = true;
        }
        yield return new WaitForSeconds(1.0f);

        if (!showScore)
        {
            StartCoroutine("UpdateHitFeedbackScreenCoroutine", "score");
            showScore = true;
        }
        if (!showChain)
        {
            StartCoroutine("UpdateHitFeedbackScreenCoroutine", "chain");
            showChain = true;
        }
        yield return new WaitForSeconds(2.0f);

        feedbackWindowGradeText.text = levelStatsData.GetLevelGrade().ToString();
        feedbackWindowGradeText.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        if (levelStatsData.GetLevelGrade() >= 60) feedbackWindowGradeSEAL.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);
        feedbackWindowNextButton.gameObject.SetActive(true);
        feedbackWindowSaveButton.gameObject.SetActive(true);
    }

    IEnumerator UpdateHitFeedbackScreenCoroutine(string type)
    {
        int currentNumHit = 0;

        while (currentNumHit != levelStatsData.GetLevelStats(type))
        {
            if(type != "score")
                currentNumHit++;
            else
            {
                if (currentNumHit < levelStatsData.GetLevelStats(type))
                    currentNumHit += 1000;
                else
                    currentNumHit--;
            }
            switch(type)
            {
                case "miss":
                    feedbackWindowHitCounters[0].text = currentNumHit.ToString();
                    break;
                case "slow":
                    feedbackWindowHitCounters[1].text = currentNumHit.ToString();
                    break;
                case "fast":
                    feedbackWindowHitCounters[2].text = currentNumHit.ToString();
                    break;
                case "good":
                    feedbackWindowHitCounters[3].text = currentNumHit.ToString();
                    break;
                case "exc":
                    feedbackWindowHitCounters[4].text = currentNumHit.ToString();
                    break;
                case "score":
                    feedbackWindowScoreCounters[0].text = currentNumHit.ToString();
                    break;
                case "chain":
                    feedbackWindowScoreCounters[1].text = currentNumHit.ToString();
                    break;
                default:
                    Debug.Log(type + " not defined");
                    break;
            }

            yield return null;
        }
    }
}

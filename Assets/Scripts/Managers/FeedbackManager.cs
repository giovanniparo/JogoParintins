using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

enum uiState
{
    PLAYING,
    FEEDBACK
}

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager instance;

    private GetPHP getPHP;

    [SerializeField] private Gradient[] colorGradients;
    [SerializeField] private GameObject particleGeneratorDrums;

    [SerializeField] private Transform feedbackBubblePosition;
    [SerializeField] private GameObject[] feedbackBubblePrefabs;
    [SerializeField] private TextMeshProUGUI[] musicDataText;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI chainText;
    [SerializeField] private TextMeshProUGUI scorePlusText;
    [SerializeField] private TextMeshProUGUI[] gradesText;
    [SerializeField] private GameObject startText;
    [SerializeField] private GameObject[] playingUIElements;
    [SerializeField] private GameObject feedbackUIElements;
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
    public int chainCounter;
    private int maxChainCounter = 0;
    private int score;
    private int currentScore;
    int currentNumScore = 0;
    int currentNumChain = 0;

    private bool isUpdatingFeedbackScreen = false;

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

        levelStatsData = new LevelStats(SceneLoader.instance.playingInfo.name,
                                        SceneLoader.instance.playingInfo.music,
                                        SceneLoader.instance.playingInfo.team,
                                        SceneLoader.instance.playingInfo.idSession);
        SetGradeTexts();
        SetMusicData();
        SetParticleSystemColor();
        state = uiState.PLAYING;
    }

    private void Update()
    {
        if (state == uiState.PLAYING)
        {
            UpdateUI();
            UpdateParticleSystems();
        }
        else if (state == uiState.FEEDBACK && !isUpdatingFeedbackScreen)
        {
            isUpdatingFeedbackScreen = true;

            foreach (GameObject uiElement in playingUIElements)
                uiElement.SetActive(false);

            feedbackUIElements.SetActive(true);

            levelStatsData.UpdateScore(currentScore);
            levelStatsData.UpdateMaxChain(maxChainCounter);
            feedbackWindowMusicName.text = musicDataText[0].text;
            feedbackWindowPlayerName.text = levelStatsData.levelData.name + " : " + System.DateTime.Now.Date.ToString();

            StartCoroutine("ShowFeedbackCoroutine");
        }
        else if(state == uiState.FEEDBACK)
        {
            /* TODO*/
        }
        else
            Debug.LogWarning("UI in a state not defined");
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
        getPHP.InsertNewScoreLine(levelStatsData.levelData.name, levelStatsData.levelData.team,
                                  levelStatsData.levelData.music + 1, levelStatsData.levelData.idSession);
        feedbackWindowSaveButton.gameObject.SetActive(false);
    }

    public void NextButtonClicked()
    {
        SceneLoader.instance.playingInfo.grades[levelStatsData.levelData.music] = levelStatsData.levelData.grade;
        SceneLoader.instance.playingInfo.music++;
        SceneLoader.instance.LoadMusic();
    }

    public void SetMusicData()
    {
        string music = "";
        string bandName = "";

        if (levelStatsData.levelData.team == 0)
        {
            switch(levelStatsData.levelData.music)
            {
                case 0:
                    music = "VERMELHO";
                    bandName = "GARANTIDO 1994";
                    break;
                case 1:
                    music = "EU SOU A TOADA";
                    bandName = "GARANTIDO 2016";
                    break;
                case 2:
                    music = "TIC TIC TAC";
                    bandName = "CARRAPICHO";
                    break;
                default:
                    Debug.LogWarning("Music not defined");
                    break;
            }
        }
        else if (levelStatsData.levelData.team == 1)
        {
            switch(levelStatsData.levelData.music)
            {
                case 0:
                    music = "PAIXAO AZUL";
                    bandName = "CAPRICHOSO 2007";
                    break;
                case 1:
                    music = "CANTO DA MATA";
                    bandName = "CANTO ENVOLVENTE";
                    break;
                case 2:
                    music = "EVOLUCAO DAS CORES";
                    bandName = "CAPRICHOSO 1999";
                    break;
                default:
                    Debug.LogWarning("Music not defined");
                    break;
            }
        }

        musicDataText[0].text = music;
        musicDataText[1].text = bandName;
    }

    public void SetGradeTexts()
    {
        if(SceneLoader.instance.playingInfo.grades[0] != -1)
        {
            gradesText[0].text = SceneLoader.instance.playingInfo.grades[0].ToString();
            if (SceneLoader.instance.playingInfo.grades[0] > 60)
                gradesText[0].color = Color.green;
            else
                gradesText[0].color = Color.red;
        }

        if (SceneLoader.instance.playingInfo.grades[1] != -1)
        {
            gradesText[1].text = SceneLoader.instance.playingInfo.grades[1].ToString();
            if (SceneLoader.instance.playingInfo.grades[1] > 60)
                gradesText[1].color = Color.green;
            else
                gradesText[1].color = Color.red;
        }
        if (SceneLoader.instance.playingInfo.grades[2] != -1)
        {
            gradesText[2].text = SceneLoader.instance.playingInfo.grades[2].ToString();
            if (SceneLoader.instance.playingInfo.grades[2] > 60)
                gradesText[2].color = Color.green;
            else
                gradesText[2].color = Color.red;
        }
    }

    public void SetParticleSystemColor() //Call at start
    {
        //Setting Color over Lifetime
        var colorOverLifetimeModule = particleGeneratorDrums.GetComponent<ParticleSystem>().colorOverLifetime;
        colorOverLifetimeModule.color = colorGradients[levelStatsData.levelData.team];
    }

    public void UpdateParticleSystems() //Call every frame
    {
        if(chainCounter == 0)
        {
            particleGeneratorDrums.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmitting); 
        }
        else
        {
            if(particleGeneratorDrums.GetComponent<ParticleSystem>().isStopped)
                particleGeneratorDrums.GetComponent<ParticleSystem>().Play();
            //Setting Emissions
            var emissionModule = particleGeneratorDrums.GetComponent<ParticleSystem>().emission;
            emissionModule.rateOverTime = Mathf.Clamp(chainCounter * 15, 0.0f, 200.0f);
        }
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

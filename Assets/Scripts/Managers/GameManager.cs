using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject circleHitPrefab;
    [SerializeField] private Transform[] chainSpawnPositions;
    [SerializeField] private Sprite[] numberSprites;
    [SerializeField] private Color[] colors;
    [SerializeField] private float minDistToPositionHit;
    [SerializeField] private float minChainHitDistance;
    [SerializeField] private float maxChainHitDistance;
    [SerializeField] private float chainTimeDif;
    [SerializeField] private float xLimitOfBackground;
    [SerializeField] private float yLimitOfBackground;
    [SerializeField] private float timeCorrectionConstant;
    [SerializeField] private int currentHitSortingOrder;

    private StreamReader streamReader;

    private Vector3 mousePos;
    private RaycastHit2D hit;
    private string levelFilePath;
    private Queue<GameObject> hitQueue;
    private GameObject currentHit;
    private Queue<GameObject> liveHitQueue;

    private int chainSpawnCounter = 0;
    private int chainID = 0;
    private float spawnStartTextTime = 10000.0f;
    private float internalTimer = 0.0f;
    private bool startSpawned = false;
    private bool startSet = false;
    private bool spawning = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one instance of GM present");
            Destroy(this);
        }
        else
            instance = this;

        levelFilePath = "Assets/Levels/" + SceneLoader.instance.currentPlayerData.music + ".txt";
        streamReader = new StreamReader(levelFilePath);
        hitQueue = new Queue<GameObject>();
        liveHitQueue = new Queue<GameObject>();
    }

    private void Start()
    {
        if (streamReader != null)
        {
            PopulateHits();
        }
        else
            Debug.LogError("Could not find levelFilePath at " + levelFilePath);

        internalTimer = 0.0f;
        AudioManager.instance.PlayMusic(SceneLoader.instance.currentPlayerData.music);
        FeedbackManager.instance.ActivateStartText();
    }

    private void Update()
    {
        if (!AudioManager.instance.IsMusicStillPlaying()) 
            FeedbackManager.instance.GiveEndofLevelFeedback();

        UpdateTimer();

        /*if (spawnStartTextTime - timeCorrectionConstant - internalTimer < 0.001f && !startSpawned)
        {
            FeedbackManager.instance.ActivateStartText();
            startSpawned = true;
        }*/

        if (hitQueue.Count > 0 && !spawning)
        {
            currentHit = hitQueue.Dequeue();
            spawning = true;
        }
            
        if (spawning && currentHit.GetComponent<Hit>().time - AudioManager.instance.GetMusicTime() <= timeCorrectionConstant)
        {
             SetCurrentHit();
             liveHitQueue.Enqueue(currentHit);
             currentHit.SetActive(true);
             spawning = false;
        }
        
        if (Input.GetMouseButtonDown(0))
            ProcessInput();
    }

    public void UpdateTimer()
    {
        internalTimer = (float)AudioSettings.dspTime; 
    }

    private void ProcessInput()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && liveHitQueue.Count > 0)
        {
            if (hit.collider.gameObject == liveHitQueue.Peek())
            {
                liveHitQueue.Dequeue().GetComponent<Hit>().Interact();
            }
        }
    }

    public void Missed()
    {
        if (liveHitQueue.Count > 0)
        {
            liveHitQueue.Dequeue();
        }
    }

    private void PopulateHits()
    {
        int chainCounter = 0;
        int colorCounter = 0;
        float prevTime = 0.0f;
        float timeDif = 0.0f;

        string currentLineFull = streamReader.ReadLine();
        string[] currentLine = currentLineFull.Split(' ');
        while (currentLineFull != null)
        {
            if (!startSet)
            {
                spawnStartTextTime = float.Parse(currentLine[0]) - 3.0f;
                startSet = true;
            }

            while (chainCounter < numberSprites.Length && currentLineFull != null)
            {
                currentHit = Instantiate(circleHitPrefab, Vector3.zero, Quaternion.identity); //Instantiate all hits at zero
                currentHit.GetComponent<CircleHit>().SetProperties(float.Parse(currentLine[0]), numberSprites[chainCounter], colors[colorCounter], chainID);
                currentHit.SetActive(false);
                hitQueue.Enqueue(currentHit);
                chainCounter++;
                prevTime = float.Parse(currentLine[0]);

                currentLineFull = streamReader.ReadLine();
                if (currentLineFull != null)
                {
                    currentLine = currentLineFull.Split(' ');
                    timeDif = float.Parse(currentLine[0]) - prevTime;
                    if (timeDif >= chainTimeDif) break;
                }
            }

            chainID++;
            chainCounter = 0;
            colorCounter++;
            if (colorCounter >= colors.Length) colorCounter = 0;
        }

        streamReader.Close();
    }

    public Vector2 GetNextRandomPosition(int chainID)
    {
        int nCounter = 0;
        Vector2 randomPos = Vector2.zero;
        float rngConst = 0.0f;
        bool useful = false;
        Vector2 spawnDirection;

        do
        {
            do
            {
                rngConst = Random.Range(minChainHitDistance, maxChainHitDistance);
                spawnDirection = GetRandomDirection();
                if (liveHitQueue.Count > 0)
                {
                    if(liveHitQueue.ElementAt(liveHitQueue.Count - 1).GetComponent<Hit>().chainID == chainID)
                        randomPos = (Vector2)liveHitQueue.ElementAt(liveHitQueue.Count - 1).transform.position + spawnDirection * rngConst;
                    else
                    {
                        randomPos = (Vector2)chainSpawnPositions[chainSpawnCounter % chainSpawnPositions.Length].transform.position + spawnDirection * rngConst;
                        chainSpawnCounter++;
                    }
                }
                else
                    randomPos = spawnDirection * rngConst;
            } while (Mathf.Abs(randomPos.x) >= xLimitOfBackground || Mathf.Abs(randomPos.y) >= yLimitOfBackground);

            foreach (GameObject hit in liveHitQueue)
            {
                if (((Vector2)hit.transform.position - randomPos).magnitude <= minDistToPositionHit * hit.GetComponent<CircleCollider2D>().radius)
                {
                    useful = false;
                    break;
                }
                useful = true;
            }

            nCounter++;
        } while (!useful && nCounter <= 1000);

        if (nCounter >= 1000) Debug.LogWarning("Could not find a suitable position to instantiate next hit");
        return randomPos;
    }

    public Vector2 GetRandomDirection()
    {
        Vector2[] directions = { Vector2.right, Vector2.up, Vector2.left, Vector2.down };
        Vector2 rngDirection = Vector2.zero;

        float rngConst = Random.value;
        if (rngConst <= 0.25f) rngDirection = directions[0];
        else if (rngConst <= 0.5f) rngDirection = directions[1];
        else if (rngConst <= 0.75f) rngDirection = directions[2];
        else rngDirection = directions[3];

        return rngDirection;
    }

    private void SetCurrentHit()
    {
        currentHit.transform.position = GetNextRandomPosition(currentHit.GetComponent<Hit>().chainID);
        currentHit.transform.localScale *= 1.1f;
        currentHit.GetComponent<SpriteRenderer>().sortingOrder = currentHitSortingOrder;
        currentHit.GetComponent<CircleHit>().numberHolder.GetComponent<SpriteRenderer>().sortingOrder = currentHitSortingOrder + 1;
        currentHit.GetComponent<CircleHit>().circleHitMarker.GetComponent<SpriteRenderer>().sortingOrder = currentHitSortingOrder+2;
    }
}

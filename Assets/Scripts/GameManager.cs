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

    [SerializeField] private GameObject lineConnectorPrefab;
    [SerializeField] private GameObject circleHitPrefab;
    [SerializeField] private string levelFilePath;
    [SerializeField] private Sprite[] numberSprites;
    [SerializeField] private Color[] colors;

    private Queue<float> timeQueue;
    private Queue<GameObject> hitQueue;
    private Queue<GameObject> liveHitQueue;
    private Queue<LineConnector> lineQueue;
    private StreamReader streamReader;

    private Vector3 mousePos;
    private RaycastHit2D hit;
    GameObject currentHitObj;
    int chainCounter = 0;
    int currentColor = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one instance of GM present");
        }
        else
            instance = this;
        
        timeQueue = new Queue<float>();
        hitQueue = new Queue<GameObject>();
        liveHitQueue = new Queue<GameObject>();
        lineQueue = new Queue<LineConnector>();
        streamReader = new StreamReader(levelFilePath);
    }

    private void Start()
    {
        if (streamReader != null)
        {
            PopulateQueues();
        }
        else
            Debug.LogError("Could not find levelFilePath at " + levelFilePath);
    }

    private void Update()
    {
        if(timeQueue.Count != 0 && hitQueue.Count != 0 && 
            timeQueue.Peek() - Time.timeSinceLevelLoad <= 0.01f)
        {
            timeQueue.Dequeue();
            currentHitObj = hitQueue.Dequeue();
            currentHitObj.SetActive(true);
            liveHitQueue.Enqueue(currentHitObj);

            if(liveHitQueue.Count > 1)
            {
                CreateLineConnector();
            }
        }

        if (Input.GetMouseButtonDown(0))
            ProcessInput();
    }

    private void ProcessInput()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject == liveHitQueue.Peek())
            {
                liveHitQueue.Dequeue().GetComponent<Hit>().Interact();
                if (lineQueue.Count > 0)
                    lineQueue.Dequeue().DestroyLine();
            }
            else if (liveHitQueue.Contains(hit.collider.gameObject))
            {
                Debug.Log("Wrong Hit!");
            }
            else
            {
                Debug.Log("GameObj isn't on the live queue!");
            } 
        }
    }

    private void CreateLineConnector()
    {
        GameObject CurrentNode;
        GameObject PrevNode;
        LineConnector connectingLine;

        CurrentNode = liveHitQueue.ElementAt(liveHitQueue.Count - 1);
        PrevNode = liveHitQueue.ElementAt(liveHitQueue.Count - 2);
        connectingLine = Instantiate(lineConnectorPrefab, CurrentNode.transform.position, Quaternion.identity)
            .GetComponent<LineConnector>();
        connectingLine.CreateLine(CurrentNode.transform.position, PrevNode.transform.position);
        connectingLine.SetLineColor(CurrentNode.GetComponent<SpriteRenderer>().color,
                                PrevNode.GetComponent<SpriteRenderer>().color);
        lineQueue.Enqueue(connectingLine);
    }

    public void Missed()
    {
        liveHitQueue.Dequeue();
        if (lineQueue.Count > 0) 
            lineQueue.Dequeue().DestroyLine();
        Debug.Log("Missed");
    }

    private void PopulateQueues()
    {
        GameObject currentHit;

        string[] currentLine;
        string currentLineFull = streamReader.ReadLine();

        int currChain = 0;
        int prevChain = 0;

        while(currentLineFull != null)
        {
            currentLine = currentLineFull.Split(' ');

            currChain = int.Parse(currentLine[0]);
            if(currChain != prevChain)
            {
                currentColor++;
                if (currentColor > colors.Length - 1) currentColor = 0;
                chainCounter = 0;
            }
            currentHit = Instantiate(circleHitPrefab,
                             new Vector3(float.Parse(currentLine[2]), float.Parse(currentLine[3]), 0.0f),
                             Quaternion.identity);
            currentHit.SetActive(false);
            currentHit.GetComponent<CircleHit>().SetProperties(numberSprites[chainCounter], colors[currentColor]);
            chainCounter++;
            if (chainCounter > 8) chainCounter = 0;
            timeQueue.Enqueue(float.Parse(currentLine[1]));
            hitQueue.Enqueue(currentHit);
            currentLineFull = streamReader.ReadLine();
            prevChain = currChain;
        }

        streamReader.Close();
    }
}

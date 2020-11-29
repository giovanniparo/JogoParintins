using System.Collections;
using TMPro;
using UnityEngine;

[System.Serializable]
enum ChatStates
{
    NEUTRAL_TURN,
    PLAYER_TURN,
    ENEMY_TURN,
    ENDING
}

public class ChatManager : MonoBehaviour
{
    [SerializeField] private float numCharsPerSecond;

    [SerializeField] private GameObject hero;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject neutralChatBox;
    [SerializeField] private TextMeshProUGUI neutralChatBoxText;
    [SerializeField] private GameObject heroChatBox;
    [SerializeField] private TextMeshProUGUI heroChatBoxText;
    [SerializeField] private Transform heroEmotePos;
    [SerializeField] private GameObject enemyChatBox;
    [SerializeField] private TextMeshProUGUI enemyChatBoxText;
    [SerializeField] private Transform enemyEmotePos;
    [SerializeField] private GameObject heroMouseClickUIanim;
    [SerializeField] private GameObject enemyMouseClickUIanim;

    [SerializeField] private string heroName;
    [SerializeField] private string enemyName;
    [SerializeField] private string[] heroLines;
    [SerializeField] private string[] enemyLines;
    [SerializeField] private string[] neutralLines;
    [SerializeField] private ChatStates[] chatOrder;

    private int heroLinesCounter = 0;
    private int enemyLinesCounter = 0;
    private int neutralLinesCounter = 0;
    private int generalLineCounter = 0;

    private bool clicked = false;
    private bool writing = false;
    private bool endOfLine = false;
    private bool finishedTalking = false;

    private ChatStates state;

    private void Start()
    {
        NextState();
    }

    private void Update()
    {
        if (!writing)
        {
            if (state == ChatStates.NEUTRAL_TURN)
            {
                if (neutralLinesCounter < neutralLines.Length)
                {
                    WriteText(neutralLines[neutralLinesCounter]);
                    neutralLinesCounter++;
                }
            }
            else if (state == ChatStates.PLAYER_TURN)
            {
                if (heroLinesCounter < heroLines.Length)
                {
                    WriteText(heroLines[heroLinesCounter]);
                    heroLinesCounter++;
                }
            }
            else if (state == ChatStates.ENEMY_TURN)
            {
                if (enemyLinesCounter < enemyLines.Length)
                {
                    WriteText(enemyLines[enemyLinesCounter]);
                    enemyLinesCounter++;
                }
            }
            else if (state == ChatStates.ENDING)
            {
                neutralChatBox.SetActive(false);
                heroChatBox.SetActive(false);
                enemyChatBox.SetActive(false);
            }
            else Debug.LogError("Chat Manager is in an ill defined state"); 
        }

        if(endOfLine && Input.GetMouseButtonDown(0))
            clicked = true;

        if (finishedTalking)
        {
            if(state == ChatStates.NEUTRAL_TURN)
            {
                hero.GetComponent<Animator>().SetBool("enter", true);
                enemy.GetComponent<Animator>().SetBool("enter", true);
            }
            NextState();
            finishedTalking = false;
            writing = false;
        }
    }

    private void NextState()
    {
        if (generalLineCounter < chatOrder.Length)
        {
            state = chatOrder[generalLineCounter];
            generalLineCounter++;
        }

        if (state == ChatStates.ENDING)
        {
            neutralChatBox.SetActive(false);
            heroChatBox.SetActive(false);
            enemyChatBox.SetActive(false);
            hero.GetComponent<Animator>().SetBool("exit", true);
            enemy.GetComponent<Animator>().SetBool("exit", true);
        }
    }

    public void WriteText(string text)
    {
        writing = true;

        if (state == ChatStates.NEUTRAL_TURN)
        {
            neutralChatBox.SetActive(true);
            heroChatBox.SetActive(false);
            enemyChatBox.SetActive(false);
            StartCoroutine(WritingNeutralTextCoroutine(text));
        }
        else if (state == ChatStates.PLAYER_TURN)
        {
            neutralChatBox.SetActive(false);
            heroChatBox.SetActive(true);
            enemyChatBox.SetActive(false);
            hero.transform.localScale *= 1.15f;
            hero.GetComponent<SpriteRenderer>().color = Color.white;
            enemy.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            enemy.GetComponent<SpriteRenderer>().color *= (Color.black - new Color(0.5f, 0.5f, 0.5f, 0.0f));
            StartCoroutine(WritingHeroTextCoroutine(text));
        }
        else if (state == ChatStates.ENEMY_TURN)
        {
            neutralChatBox.SetActive(false);
            heroChatBox.SetActive(false);
            enemyChatBox.SetActive(true);
            hero.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            hero.GetComponent<SpriteRenderer>().color *= (Color.black - new Color(0.5f, 0.5f, 0.5f, 0.0f));
            enemy.transform.localScale *= 1.15f;
            enemy.GetComponent<SpriteRenderer>().color = Color.white;
            StartCoroutine(WritingEnemyTextCoroutine(text));
        }
        else
            Debug.LogWarning("Chat Manager is in an ill defined state");
    }

    /*public void InstEmote(int emoteIndex, string whichChar)
    {
        if (emoteIndex < 0 || emoteIndex > emotePrefabs.Length)
        {
            Debug.LogWarning("Couldn't inst emote, pass a valid emoteIndex");
            return;
        }

        GameObject emoteGO = emotePrefabs[emoteIndex];

        switch (whichChar)
        {
            case "hero":
                Instantiate(emoteGO, Camera.main.WorldToScreenPoint(heroEmotePos.position), Quaternion.identity).
                    transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
                break;
            case "enemy":
                Instantiate(emoteGO, Camera.main.WorldToScreenPoint(enemyEmotePos.position), Quaternion.identity).
                    transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
                break;
            default:
                Debug.LogWarning("Couldn't instantiate emote gameObject, choose a valid character");
                break;
        }
    }*/

    IEnumerator WritingEnemyTextCoroutine(string text)
    {
        char[] charArray = text.ToCharArray();
        enemyChatBoxText.overflowMode = TextOverflowModes.Truncate;
        enemyChatBoxText.renderMode = TextRenderFlags.DontRender;
        enemyChatBoxText.text = text;
        enemyChatBoxText.ForceMeshUpdate();

        TMP_TextInfo tMP_TextInfo = enemyChatBoxText.textInfo;
        TMP_LineInfo[] tMP_LineInfos = new TMP_LineInfo[tMP_TextInfo.lineCount];
        tMP_LineInfos = tMP_TextInfo.lineInfo;
        enemyChatBoxText.maxVisibleLines = 2;

        enemyChatBoxText.overflowMode = TextOverflowModes.Overflow;
        enemyChatBoxText.renderMode = TextRenderFlags.Render;
        enemyChatBoxText.text = "";
        enemyChatBoxText.ForceMeshUpdate();

        int totalWritten = 0;
        while (totalWritten < charArray.Length)
        {
            while (enemyChatBoxText.textInfo.lineCount <= enemyChatBoxText.maxVisibleLines &&
                totalWritten < charArray.Length)
            {
                enemyChatBoxText.text += charArray[totalWritten];
                yield return new WaitForSeconds(1.0f / numCharsPerSecond);
                totalWritten++;
            }
            endOfLine = true;
            enemyMouseClickUIanim.SetActive(true);
            while (!clicked)
            {
                yield return null;
            }
            enemyMouseClickUIanim.SetActive(false);
            clicked = false;
            endOfLine = false;
            enemyChatBoxText.text = "";
            enemyChatBoxText.ForceMeshUpdate();
        }

        finishedTalking = true;
        if (totalWritten == charArray.Length)
            Debug.Log("Reached end of char array");
    }

    IEnumerator WritingHeroTextCoroutine(string text)
    {
        char[] charArray = text.ToCharArray();
        heroChatBoxText.overflowMode = TextOverflowModes.Truncate;
        heroChatBoxText.renderMode = TextRenderFlags.DontRender;
        heroChatBoxText.text = text;
        heroChatBoxText.ForceMeshUpdate();

        TMP_TextInfo tMP_TextInfo = heroChatBoxText.textInfo;
        TMP_LineInfo[] tMP_LineInfos = new TMP_LineInfo[tMP_TextInfo.lineCount];
        tMP_LineInfos = tMP_TextInfo.lineInfo;
        heroChatBoxText.maxVisibleLines = 2;

        heroChatBoxText.overflowMode = TextOverflowModes.Overflow;
        heroChatBoxText.renderMode = TextRenderFlags.Render;
        heroChatBoxText.text = "";
        heroChatBoxText.ForceMeshUpdate();

        int totalWritten = 0;
        while (totalWritten < charArray.Length)
        {
            while (heroChatBoxText.textInfo.lineCount <= heroChatBoxText.maxVisibleLines && 
                totalWritten < charArray.Length)
            {
                heroChatBoxText.text += charArray[totalWritten];
                yield return new WaitForSeconds(1.0f / numCharsPerSecond);
                totalWritten++;
            }
            endOfLine = true;
            heroMouseClickUIanim.SetActive(true);
            while (!clicked)
            {
                yield return null;
            }
            heroMouseClickUIanim.SetActive(false);
            clicked = false;
            endOfLine = false;
            heroChatBoxText.text = "";
            heroChatBoxText.ForceMeshUpdate();
        }

        finishedTalking = true;
        if (totalWritten == charArray.Length)
            Debug.Log("Reached end of char array");
    }

    IEnumerator WritingNeutralTextCoroutine(string text)
    {
        char[] charArray = text.ToCharArray();
        neutralChatBoxText.overflowMode = TextOverflowModes.Truncate;
        neutralChatBoxText.renderMode = TextRenderFlags.DontRender;
        neutralChatBoxText.text = text;
        neutralChatBoxText.ForceMeshUpdate();

        TMP_TextInfo tMP_TextInfo = neutralChatBoxText.textInfo;
        TMP_LineInfo[] tMP_LineInfos = new TMP_LineInfo[tMP_TextInfo.lineCount];
        tMP_LineInfos = tMP_TextInfo.lineInfo;
        neutralChatBoxText.maxVisibleLines = 4;

        neutralChatBoxText.overflowMode = TextOverflowModes.Overflow;
        neutralChatBoxText.renderMode = TextRenderFlags.Render;
        neutralChatBoxText.text = "";
        neutralChatBoxText.ForceMeshUpdate();

        int totalWritten = 0;
        while (totalWritten < charArray.Length)
        {
            while (neutralChatBoxText.textInfo.lineCount <= neutralChatBoxText.maxVisibleLines &&
                totalWritten < charArray.Length)
            {
                neutralChatBoxText.text += charArray[totalWritten];
                yield return new WaitForSeconds(1.0f / numCharsPerSecond);
                totalWritten++;
            }

            yield return new WaitForSeconds(0.5f);
            neutralChatBoxText.text = "";
            neutralChatBoxText.ForceMeshUpdate();
        }

        finishedTalking = true;
        if (totalWritten == charArray.Length)
            Debug.Log("Reached end of char array");
    }

}

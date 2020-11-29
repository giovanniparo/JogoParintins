using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public enum loginUIState
{
    LOGIN,
    CREATE_USER,
    CHOOSE_ACTION,
    CHOOSE_TEAM,
    LOADING_SCENE
}

public struct Info
{
    public string name;
    public int music;
    public int team;
    public int idSession;
    public int[] grades;
}

public class LoginManager : MonoBehaviour
{
    public TextMeshProUGUI feedbackTextMesh;

    public static LoginManager instance;

    private GetPHP getPHP;

    [SerializeField] private GameObject logingPanel;
    [SerializeField] private GameObject createUserPanel;
    [SerializeField] private GameObject chooseTeamPanel;
    [SerializeField] private GameObject chooseActionPanel;

    [SerializeField] private TextMeshProUGUI[] chooseActionPanelText;
    [SerializeField] private Button chooseActionContinuarButton;

    [SerializeField] private TMP_InputField loginNameInput;
    [SerializeField] private TMP_InputField loginPassInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField passInput;
    [SerializeField] private TMP_InputField emailInput;

    private loginUIState state;
    public Info loadedInfo;

    private int numTries = 150;
    private bool loggingIn = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("More than one instance of login manager found.");
            Destroy(this);
        }
    }

    private void Start()
    {
        loadedInfo.grades = new int[3];
        getPHP = GetComponent<GetPHP>();
        state = loginUIState.LOGIN;
    }

    public void Update()
    {
        switch (state)
        {
            case loginUIState.LOGIN:
                createUserPanel.SetActive(false);
                logingPanel.SetActive(true);
                chooseActionPanel.SetActive(false);
                chooseTeamPanel.SetActive(false);
                break;
            case loginUIState.CREATE_USER:
                createUserPanel.SetActive(true);
                logingPanel.SetActive(false);
                chooseActionPanel.SetActive(false);
                chooseTeamPanel.SetActive(false);
                break;
            case loginUIState.CHOOSE_ACTION:
                createUserPanel.SetActive(false);
                logingPanel.SetActive(false);
                chooseActionPanel.SetActive(true);
                chooseTeamPanel.SetActive(false);
                break;
            case loginUIState.CHOOSE_TEAM:
                createUserPanel.SetActive(false);
                logingPanel.SetActive(false);
                chooseActionPanel.SetActive(false);
                chooseTeamPanel.SetActive(true);
                break;
            case loginUIState.LOADING_SCENE:
                SceneLoader.instance.playingInfo = loadedInfo;
                SceneLoader.instance.LoadMusic();
                break;
            default:
                Debug.LogWarning("loginUI state not well defined!");
                break;
        }
    }

    public void ChangeInterface()
    {
        if (state == loginUIState.CREATE_USER) //RETURN BUTTON
        {
            loggingIn = false;
            PlaySound("cancel");
            state = loginUIState.LOGIN;
        }
        else if (state == loginUIState.LOGIN) //CREATE USER BUTTON
        {
            state = loginUIState.CREATE_USER;
        }
        else if (state == loginUIState.CHOOSE_ACTION) //RETURN BUTTON AT CHOOSE ACTION
        {
            loggingIn = false;
            PlaySound("cancel");
            LogOff();
            state = loginUIState.LOGIN;
        }
        else if (state == loginUIState.CHOOSE_TEAM) // RETURN BUTTON AT CHOOSE TEAM
        {
            PlaySound("cancel");
            state = loginUIState.CHOOSE_ACTION;
        }
    }

    public void CreateUserButtonClicked()
    {
        StartCoroutine(CreateUserLoginCoroutine());
    }

    public void LoginButtonClicked()
    {
        if (!loggingIn)
        {
            loggingIn = true;
            PlaySound("confirm");
            StartCoroutine(LoginCoroutine());
        }
    }

    public void NewGameButtonClicked()
    {
        PlaySound("confirm");
        state = loginUIState.CHOOSE_TEAM;
    }

    public void ContinueGameButtonClicked()
    {
        UpdateLoadedInfo();
        SceneLoader.instance.playingInfo = loadedInfo;
        PlaySound("confirm");
        SceneLoader.instance.LoadMusic();
    }

    public void ChooseBullButtonClicked(int choosenBull)
    {
         if (choosenBull == 0 || choosenBull == 1)
        {
            loadedInfo.team = choosenBull;
            PlaySound("confirm");
            StartCoroutine(CreateSessionCoroutine());
        }
         else
             Debug.LogError("The specified team index is not defined");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PlaySound(string action)
    {
        MenuAudio.instance.PlayMenuSFX(action);
    }

    public void LogOff()
    {
        loadedInfo = new Info();
        loadedInfo.grades = new int[3];
        getPHP.currentPlayerData.Clear();
        getPHP.currentIdData.Clear();
    }

    IEnumerator LoginCoroutine()
    {
        StartCoroutine(givePlayerTextFeedback("Entrando...", "confirm", null));
        getPHP.FindUser(loginNameInput.text, loginPassInput.text);

        int nCounter = numTries;
        while (getPHP.currentPlayerData.error == true && nCounter > 0)
        {
            yield return null;
            nCounter--;
        }
        if(nCounter == 0)
        {
            StartCoroutine(givePlayerTextFeedback(GetPHP.returnMessage, null, "cancel"));
        }
        else
        {
            StartCoroutine(givePlayerTextFeedback(GetPHP.returnMessage, null, null));
            getPHP.CheckIfSessaoExists(getPHP.currentPlayerData.name);

            nCounter = numTries;
            while (getPHP.currentIdData.error == true && nCounter > 0)
            {
                yield return null;
                nCounter--;
            }
            if (nCounter == 0)
            {
                StartCoroutine(givePlayerTextFeedback(GetPHP.returnMessage, "cancel", null));
            }
            else
            {
                StartCoroutine(givePlayerTextFeedback(GetPHP.returnMessage, null, null));
                UpdateLoadedInfo();
                chooseActionPanelText[0].text = loadedInfo.name;
                if (loadedInfo.idSession != 0 && loadedInfo.team != 2)
                {
                    chooseActionPanelText[1].text = SceneLoader.ConvMusicIndexToName(loadedInfo.team, loadedInfo.music);
                    chooseActionContinuarButton.gameObject.SetActive(true);
                    chooseActionPanelText[1].gameObject.SetActive(true);
                    chooseActionPanelText[2].gameObject.SetActive(true);
                    if (loadedInfo.team == 0)
                        chooseActionPanelText[2].text = "GARANTIDO";
                    else if (loadedInfo.team == 1)
                        chooseActionPanelText[2].text = "CAPRICHOSO";
                }
                else
                {
                    chooseActionContinuarButton.gameObject.SetActive(false);
                    chooseActionPanelText[1].gameObject.SetActive(false);
                    chooseActionPanelText[2].gameObject.SetActive(false);
                }

                state = loginUIState.CHOOSE_ACTION;
            }
        }

        loggingIn = false;
    }

    IEnumerator CreateSessionCoroutine()
    {
        getPHP.CreateSession(loadedInfo.name, loadedInfo.team);

        int nCounter = numTries;
        while(getPHP.currentIdData.error == true && nCounter > 0)
        {
            yield return null;
            nCounter--;
        }

        if(nCounter != 0)
        {
            UpdateLoadedInfo();
            SceneLoader.instance.playingInfo = loadedInfo;
            SceneLoader.instance.LoadMusic();
        }

        Debug.Log(GetPHP.returnMessage);
    }

    IEnumerator CreateUserLoginCoroutine()
    {
        getPHP.CreateUser(nameInput.text, passInput.text, emailInput.text);
        int nCounter = numTries;
        while (!GetPHP.jobIsDone && nCounter > 0)
        {
            yield return null;
            nCounter--;
        }

        GetPHP.jobIsDone = false;
        if (nCounter == 0)
            StartCoroutine(givePlayerTextFeedback(GetPHP.returnMessage, null, "cancel"));
        else
        {
            StartCoroutine(givePlayerTextFeedback(GetPHP.returnMessage, "confirm", null));
            state = loginUIState.LOGIN;
        }
    }

    public void UpdateLoadedInfo()
    {
        loadedInfo.name = getPHP.currentPlayerData.name;
        loadedInfo.music = getPHP.currentIdData.music;
        loadedInfo.team = getPHP.currentIdData.team;
        loadedInfo.idSession = getPHP.currentIdData.id;
        loadedInfo.grades[0] = getPHP.currentIdData.grades[0];
        loadedInfo.grades[1] = getPHP.currentIdData.grades[1];
        loadedInfo.grades[2] = getPHP.currentIdData.grades[2];
    }

    IEnumerator givePlayerTextFeedback(string feedback, string onStartSound, string onEndSound)
    {
        feedbackTextMesh.gameObject.SetActive(true);
        feedbackTextMesh.text += feedback + "\n";
        if (!string.IsNullOrEmpty(onStartSound)) PlaySound(onStartSound);
        yield return new WaitForSeconds(2.0f);
        feedbackTextMesh.text = "";
        feedbackTextMesh.gameObject.SetActive(false);
        if (!string.IsNullOrEmpty(onEndSound)) PlaySound(onEndSound);
    }
}

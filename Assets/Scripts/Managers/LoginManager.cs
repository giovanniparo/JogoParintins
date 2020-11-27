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
    [SerializeField] private TMP_InputField generoInput;

    private loginUIState state;
    public Info loadedInfo;

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
                SceneLoader.playingInfo = loadedInfo;
                SceneLoader.instance.LoadNextMusic();
                break;
            default:
                Debug.LogWarning("loginUI state not well defined!");
                break;
        }

        if (GetPHP.returnMessage != "")
            StartCoroutine(givePlayerTextFeedback());
    }

    public void ChangeInterface()
    {
        if (state == loginUIState.CREATE_USER) //RETURN BUTTON
        {
            loggingIn = false;
            state = loginUIState.LOGIN;
        }
        else if (state == loginUIState.LOGIN) //CREATE USER BUTTON
        {
            state = loginUIState.CREATE_USER;
        }
        else if (state == loginUIState.CHOOSE_ACTION) //RETURN BUTTON AT CHOOSE ACTION
        {
            loggingIn = false;
            state = loginUIState.LOGIN;
        }
        else if (state == loginUIState.CHOOSE_TEAM) // RETURN BUTTON AT CHOOSE TEAM
        {
            state = loginUIState.CHOOSE_ACTION;
        }
    }

    public void CreateUserButtonClicked()
    {
        getPHP.CreateUser(nameInput.text, passInput.text, emailInput.text);
        state = loginUIState.LOGIN;
    }

    public void LoginButtonClicked()
    {
        if (!loggingIn)
        {
            loggingIn = true;
            getPHP.FindUser(loginNameInput.text, loginPassInput.text);
            StartCoroutine(LoginCoroutine());
        }
    }

    public void NewGameButtonClicked()
    {
        state = loginUIState.CHOOSE_TEAM;
    }

    public void ContinueGameButtonClicked()
    {
        UpdateLoadedInfo();
        SceneLoader.playingInfo = loadedInfo;
        SceneLoader.instance.LoadNextMusic();
    }

    public void ChooseBullButtonClicked(int choosenBull)
    {
         if (choosenBull == 0 || choosenBull == 1)
        {
            loadedInfo.team = choosenBull;
            StartCoroutine(CreateSessionCoroutine());
        }
         else
             Debug.LogError("The specified team index is not defined");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator LoginCoroutine()
    {
        while (getPHP.currentPlayerData.error == true)
        {
            yield return null;
        }

        getPHP.CheckIfSessaoExists(getPHP.currentPlayerData.name);

        while (getPHP.currentIdData.error == true)
        {
            yield return null;
        }

        UpdateLoadedInfo();
        if(loadedInfo.idSession != 0 && loadedInfo.team != 2)
        {
            chooseActionPanelText[0].text = loadedInfo.name;
            chooseActionPanelText[1].text = SceneLoader.ConvMusicIndexToName(loadedInfo.music, loadedInfo.team);
            chooseActionContinuarButton.gameObject.SetActive(true);
            chooseActionPanelText[1].gameObject.SetActive(true);
            chooseActionPanelText[2].gameObject.SetActive(true);
            if (loadedInfo.team == 0)
                chooseActionPanelText[2].text = "CAPRICHOSO";
            else if (loadedInfo.team == 1)
                chooseActionPanelText[2].text = "GARANTIDO";
        }
        else
        {
            chooseActionContinuarButton.gameObject.SetActive(false);
            chooseActionPanelText[1].gameObject.SetActive(false);
            chooseActionPanelText[2].gameObject.SetActive(false);
        }

        state = loginUIState.CHOOSE_ACTION;
    }

    IEnumerator CreateSessionCoroutine()
    {
        getPHP.CreateSession(loadedInfo.name, loadedInfo.team);

        while(getPHP.currentIdData.error == true)
        {
            yield return null;
        }

        UpdateLoadedInfo();
        SceneLoader.playingInfo = loadedInfo;
        SceneLoader.instance.LoadNextMusic();
    }

    public void UpdateLoadedInfo()
    {
        loadedInfo.name = getPHP.currentPlayerData.name;
        loadedInfo.music = getPHP.currentIdData.music;
        loadedInfo.team = getPHP.currentIdData.team;
        loadedInfo.idSession = getPHP.currentIdData.id;

    }

    IEnumerator givePlayerTextFeedback()
    {
          feedbackTextMesh.gameObject.SetActive(true);
          feedbackTextMesh.text = GetPHP.returnMessage;
          yield return new WaitForSeconds(2.0f);
          feedbackTextMesh.text = "";
          GetPHP.returnMessage = "";
          feedbackTextMesh.gameObject.SetActive(false);
    }

}

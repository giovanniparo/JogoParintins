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
    CHOOSE_TEAM
}

public class LoginManager : MonoBehaviour
{
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
        getPHP.CreateUser(nameInput.text, passInput.text, emailInput.text, generoInput.text.ToCharArray()[0]);
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

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator LoginCoroutine()
    {
        while(SceneLoader.instance.currentPlayerData.error == true)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (SceneLoader.instance.currentPlayerData.error != true)
        {
            chooseActionPanelText[0].text = SceneLoader.instance.currentPlayerData.name;
            chooseActionPanelText[1].text = SceneLoader.instance.currentPlayerData.music;
            if (chooseActionPanelText[1].text != "NOTDEF")
            {
                chooseActionContinuarButton.gameObject.SetActive(true);
                chooseActionPanelText[1].gameObject.SetActive(true);
                chooseActionPanelText[2].gameObject.SetActive(true);
                if (SceneLoader.instance.currentPlayerData.team == 0)
                    chooseActionPanelText[2].text = "CAPRICHOSO";
                else if (SceneLoader.instance.currentPlayerData.team == 1)
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
    }
}

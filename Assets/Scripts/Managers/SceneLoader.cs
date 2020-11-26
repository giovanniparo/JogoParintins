using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    [SerializeField] private string[] caprichosoScenes;
    [SerializeField] private string[] garantidoScenes;

    private static int sceneCounter = 0;

    public JogadorLoginData currentPlayerData;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("More than one instance of SceneLoader found.");
            Destroy(this);
        }
    }

    public void SetSelectedTeam(int bullIndex)
    {
        if (bullIndex == 0 || bullIndex == 1)
        {
            currentPlayerData.team = bullIndex;
            LoadNextMusic();
        }
        else
            Debug.LogError("The specified team index is not defined");
    }

    public void LoadNextMusic()
    {
        if (currentPlayerData.team == 0 && sceneCounter < caprichosoScenes.Length)
        {
            currentPlayerData.music = caprichosoScenes[sceneCounter];
            sceneCounter++;
            SceneManager.LoadScene("Scenes/MusicScene");
        }
        else if (currentPlayerData.team == 1 && sceneCounter < garantidoScenes.Length)
        {
            currentPlayerData.music = garantidoScenes[sceneCounter];
            sceneCounter++;
            SceneManager.LoadScene("Scenes/MusicScene");
        }
        else
            Debug.LogError("Tried to load a scene not defined by the characters.");
    }
}

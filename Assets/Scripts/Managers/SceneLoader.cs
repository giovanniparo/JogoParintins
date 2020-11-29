using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    [SerializeField] private string[] caprichosoScenes;
    [SerializeField] private string[] garantidoScenes;

    public Info playingInfo;
    public string currentPlayingMusic;

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

        playingInfo.grades = new int[3];
    }

    public void LoadMusic()
    {
        if (playingInfo.team == 0 && playingInfo.music < garantidoScenes.Length) //Garantido
        {
            currentPlayingMusic = garantidoScenes[playingInfo.music];
            SceneManager.LoadScene("Scenes/MusicScene");
        }
        else if (playingInfo.team == 1 && playingInfo.music < caprichosoScenes.Length) //Caprichoso
        {
            currentPlayingMusic = caprichosoScenes[playingInfo.music];
            SceneManager.LoadScene("Scenes/MusicScene");
        }
        else
            Debug.LogError("Tried to load a scene not defined by the characters.");
    }

    public static string ConvMusicIndexToPath(int team, int musicIndex)
    {
        if (team == 0) //Garantido
        {
            switch (musicIndex)
            {
                case 0:
                    return "vermelho";
                case 1:
                    return "eusouatoada";
                case 2:
                    return "tictictac";
                default:
                    Debug.LogWarning("Music not defined");
                    break;
            }
        }
        else if (team == 1) //Caprichoso
        {
            switch (musicIndex)
            {
                case 0:
                    return "paixaoazul";
                case 1:
                    return "cantodamata";
                case 2:
                    return "evolucaodascores";
                default:
                    Debug.LogWarning("Music not defined");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Team not defined");
        }

        return "ERROR_NOTDEF";
    }

    public static string ConvMusicIndexToName(int team, int musicIndex)
    {
        string musicName = "ERROR_NOTDEF";

        if (team == 0) //Garantido
        {
            switch (musicIndex) 
            {
                case 0:
                    musicName = "Vermelho";
                    break;
                case 1:
                    musicName = "Eu Sou a Toada";
                    break;
                case 2:
                    musicName = "TIC TIC TAC";
                    break;
                default:
                    Debug.LogWarning("Music not defined");
                    break;
            }
        }
        else if (team == 1) //Caprichoso
        {
            switch (musicIndex)
            {
                case 0:
                    musicName = "Paixao Azul";
                    break;
                case 1:
                    musicName = "Canto Da Mata";
                    break;
                case 2:
                    musicName = "Evolucao Das Cores";
                    break;
                default:
                    Debug.LogWarning("Music not defined");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Team not defined");
        }

        return musicName;
    }
}

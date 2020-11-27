using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    [SerializeField] private string[] caprichosoScenes;
    [SerializeField] private string[] garantidoScenes;

    public static Info playingInfo;
    public static string currentPlayingMusic;

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

    public void LoadNextMusic()
    {
        if (playingInfo.team == 0 && playingInfo.music < garantidoScenes.Length)
        {
            currentPlayingMusic = garantidoScenes[playingInfo.music];
            playingInfo.music++;
            SceneManager.LoadScene("Scenes/MusicScene");
        }
        else if (playingInfo.team == 1 && playingInfo.music < caprichosoScenes.Length)
        {
            currentPlayingMusic = caprichosoScenes[playingInfo.music];
            playingInfo.music++;
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
                    return "Cantodamata";
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
        if (team == 0) //Garantido
        {
            switch (musicIndex) 
            {
                case 0:
                    return "Vermelho";
                case 1:
                    return "Eu Sou a Toada";
                case 2:
                    return "TIC TIC TAC";
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
                    return "Paixao Azul";
                case 1:
                    return "Canto Da Mata";
                case 2:
                    return "Evolucao Das Cores";
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
}

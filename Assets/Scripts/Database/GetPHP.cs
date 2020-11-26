using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class GetPHP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userFeedback;

    public void CreateUser(string name, string pass, string email, char gender)
    {
        StartCoroutine(CreateUserCoroutine(name, pass, email, gender));
    }

    public void FindUser(string nome, string pass)
    {
        StartCoroutine(FindUserCoroutine(nome, pass));
    }

    public void SaveMusicData(LevelData levelData)
    {
        StartCoroutine(SaveLevelStatsCoroutine(levelData));
    }

    public void UpdatePlayerData(string name, string music, int team)
    {
        StartCoroutine(UpdatePlayerDataCoroutine(name, music, team));
    }

    IEnumerator UpdatePlayerDataCoroutine(string name, string music, int team)
    {
        WWWForm wwwf = new WWWForm();
        wwwf.AddField("name", name);
        wwwf.AddField("music", music);
        wwwf.AddField("team", team);

        using (var w = UnityWebRequest.Post("http://localhost/jogos/updateJogador.php", wwwf))
        {
            yield return w.SendWebRequest();

            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError(w.error);
            }
            else
            {
                StartCoroutine(GiveUserFeedbackCoroutine(w.downloadHandler.text));
            }
        }
    }

    IEnumerator CreateUserCoroutine(string name, string pass, string email, char gender)
    {
        WWWForm wwwf = new WWWForm();
        wwwf.AddField("id", Random.Range(1, 1000000));
        wwwf.AddField("name", name);
        wwwf.AddField("pass", MD5Sum(pass));
        wwwf.AddField("email", email);
        wwwf.AddField("gender", gender);

        using (var w = UnityWebRequest.Post("http://localhost/jogos/inserirJogador.php", wwwf))
        {
            yield return w.SendWebRequest();

            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError(w.error);
            }
            else
            {
                StartCoroutine(GiveUserFeedbackCoroutine(w.downloadHandler.text));
            }
        }
    }

    IEnumerator FindUserCoroutine(string nome, string pass)
    {
        WWWForm wwwf = new WWWForm();
        wwwf.AddField("nome", nome);
        wwwf.AddField("pass", MD5Sum(pass));

        WWW www = new WWW("http://localhost/jogos/encontrarJogador.php", wwwf);
        
        yield return www;

        if (www.error == null)
        {
            SceneLoader.instance.currentPlayerData = JsonUtility.FromJson<JogadorLoginData>(www.text);
            StartCoroutine(GiveUserFeedbackCoroutine(SceneLoader.instance.currentPlayerData.message));
        }
    }

    IEnumerator SaveLevelStatsCoroutine(LevelData levelData)
    {
        WWWForm wwwf = new WWWForm();
        wwwf.AddField("nome", levelData.name);
        wwwf.AddField("score", levelData.score);
        wwwf.AddField("grade", levelData.grade);
        wwwf.AddField("chain", levelData.maxChainSize);
        wwwf.AddField("music", levelData.music);
        wwwf.AddField("numMiss", levelData.numMiss);
        wwwf.AddField("numSlow", levelData.numSlow);
        wwwf.AddField("numFast", levelData.numFast);
        wwwf.AddField("numGood", levelData.numGood);
        wwwf.AddField("numExc", levelData.numExcelent);

        using (var w = UnityWebRequest.Post("http://localhost/jogos/saveMusicData.php", wwwf))
        {
            yield return w.SendWebRequest();

            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError(w.error);
            }
            else
            {
                StartCoroutine(GiveUserFeedbackCoroutine(w.downloadHandler.text));
            }
        }
    }

    IEnumerator GiveUserFeedbackCoroutine(string feedback)
    {
        userFeedback.gameObject.SetActive(true);
        userFeedback.text = feedback;
        Debug.Log(feedback);
        yield return new WaitForSeconds(2.0f);
        userFeedback.text = "";
        userFeedback.gameObject.SetActive(false);
    }

    public string MD5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
}

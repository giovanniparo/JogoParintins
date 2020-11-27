using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class GetPHP : MonoBehaviour
{
    public JogadorLoginData currentPlayerData;
    public IdData currentIdData;
    public static string returnMessage;

    private void Awake()
    {
        currentPlayerData = new JogadorLoginData();
        currentIdData = new IdData();
    }

    public void CreateUser(string name, string pass, string email)
    {
        StartCoroutine(CreateUserCoroutine(name, pass, email));
    }

    public void FindUser(string name, string pass)
    {
        StartCoroutine(FindUserCoroutine(name, pass));
    }

    public void SaveMusicData(LevelData levelData)
    {
        StartCoroutine(SaveLevelStatsCoroutine(levelData));
    }

    public void CheckIfSessaoExists(string name)
    {
        StartCoroutine(CheckIfSessaoExistsCoroutine(name));
    }

    public void CreateSession(string name, int team)
    {
        currentIdData.Clear();
        StartCoroutine(CreateSessionCoroutine(name, team));
    }

    IEnumerator CheckIfSessaoExistsCoroutine(string name)
    {
        WWWForm wwwf = new WWWForm();
        wwwf.AddField("name", name);

        WWW www = new WWW("http://localhost/jogos/checkSectionID.php", wwwf);

        yield return www;

        if (www.error == null)
        {
            Debug.Log(www.text);
            currentIdData = JsonUtility.FromJson<IdData>(www.text);
            returnMessage = currentIdData.message;
        }
    }

    IEnumerator CreateSessionCoroutine(string name, int team)
    {
        WWWForm wwwf = new WWWForm();
        wwwf.AddField("name", name);
        wwwf.AddField("team", team);

        using (var w = UnityWebRequest.Post("http://localhost/jogos/createSession.php", wwwf))
        {
            yield return w.SendWebRequest();

            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError(w.error);
            }
            else
            {
                Debug.Log(w.downloadHandler.text);
                currentIdData = JsonUtility.FromJson<IdData>(w.downloadHandler.text);
                returnMessage = currentIdData.message;
            }
        }
    }

    IEnumerator CreateUserCoroutine(string name, string pass, string email)
    {
        WWWForm wwwf = new WWWForm();
        wwwf.AddField("name", name);
        wwwf.AddField("pass", MD5Sum(pass));
        wwwf.AddField("email", email);

        using (var w = UnityWebRequest.Post("http://localhost/jogos/inserirJogador.php", wwwf))
        {
            yield return w.SendWebRequest();

            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError(w.error);
            }
            else
            {
                Debug.Log(w.downloadHandler.text);
                returnMessage = w.downloadHandler.text;
            }
        }
    }

    IEnumerator FindUserCoroutine(string name, string pass)
    {
        WWWForm wwwf = new WWWForm();
        wwwf.AddField("name", name);
        wwwf.AddField("pass", MD5Sum(pass));

        WWW www = new WWW("http://localhost/jogos/encontrarJogador.php", wwwf);
        
        yield return www;

        if (www.error == null)
        {
            Debug.Log(www.text);
            currentPlayerData = JsonUtility.FromJson<JogadorLoginData>(www.text);
            returnMessage = currentPlayerData.message;
        }
    }

    IEnumerator SaveLevelStatsCoroutine(LevelData levelData)
    {
        WWWForm wwwf = new WWWForm();
        wwwf.AddField("name", levelData.name);
        wwwf.AddField("score", levelData.score);
        wwwf.AddField("grade", levelData.grade);
        wwwf.AddField("chain", levelData.maxChainSize);
        wwwf.AddField("music", levelData.music);
        wwwf.AddField("team", levelData.team);
        wwwf.AddField("numMiss", levelData.numMiss);
        wwwf.AddField("numSlow", levelData.numSlow);
        wwwf.AddField("numFast", levelData.numFast);
        wwwf.AddField("numGood", levelData.numGood);
        wwwf.AddField("numExc", levelData.numExcelent);
        wwwf.AddField("idSession", levelData.idSession);

        using (var w = UnityWebRequest.Post("http://localhost/jogos/saveMusicData.php", wwwf))
        {
            yield return w.SendWebRequest();

            if (w.isNetworkError || w.isHttpError)
            {
                Debug.LogError(w.error);
            }
            else
            {
                Debug.Log(w.downloadHandler.text);
                returnMessage = w.downloadHandler.text;
            }
        }
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

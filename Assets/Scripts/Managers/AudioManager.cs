using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [SerializeField] private AudioClip[] musics;
    [SerializeField] private AudioClip[] sfxs;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource[] hitSfxSources;

    int nCounter = 0;
    int iCounter = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.LogError("More than one instance of AudioManager present");
            Destroy(this);
        }

        foreach(AudioSource source in hitSfxSources)
        {
            source.clip = sfxs[0];
            source.volume = 0.01f;
        }
    }

    public void PlayMusic(string level)
    {
        switch(level)
        {
            case "vermelho":
                musicSource.clip = musics[0];
                break;
            case "eusouatoada":
                musicSource.clip = musics[1];
                break;
            case "tictictac":
                musicSource.clip = musics[2];
                break;
            case "paixaoazul":
                musicSource.clip = musics[3];
                break;
            case "cantodamata":
                musicSource.clip = musics[4];
                break;
            case "evolucaodascores":
                musicSource.clip = musics[5];
                break;
            default:
                Debug.LogWarning("Music not defined");
                break;
        }

        musicSource.volume = 0.02f;
        musicSource.Play();
    }

    public void PlayHitSound()
    {
        hitSfxSources[iCounter % hitSfxSources.Length].Play();
        iCounter++;
    }


    public void ScheduleSFX(string sfx, float timeToPlay)
    {
        switch (sfx)
        {
            case "hit":
                hitSfxSources[nCounter % hitSfxSources.Length].PlayScheduled(timeToPlay);
                break;
            default:
                Debug.LogWarning("SFX not defined");
                break;
        }

        nCounter++;
    }

    public float GetMusicTime()
    {
        return musicSource.time;
    }

    public bool IsMusicStillPlaying()
    {
        return musicSource.isPlaying;
    }
}

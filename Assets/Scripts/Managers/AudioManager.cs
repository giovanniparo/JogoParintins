using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [SerializeField] private AudioClip[] musicsGaratindo;
    [SerializeField] private AudioClip[] musicsCaprichoso;
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

    public void PlayMusic(int music, int team)
    {
        musicSource.volume = 0.02f;
        if (team == 0) //Garantido
        {
            musicSource.clip = musicsGaratindo[music];
            musicSource.Play();
        }
        else if (team == 1) //Caprichoso
        {
            musicSource.clip = musicsCaprichoso[music];
            musicSource.Play();
        }
        else
            Debug.LogError("Failed to load a valid music clip, team not def");
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

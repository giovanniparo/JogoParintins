using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    public static MenuAudio instance;
    
    [SerializeField] private AudioClip confirmClip;
    [SerializeField] private AudioClip cancelClip;
    [SerializeField] private AudioSource[] sfxSource;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Debug.LogWarning("More than one instance of MenuAudio was found!");
            Destroy(this);
        }
    }

    public void PlayMenuSFX(string sfx)
    {
        AudioSource usedAudioSource;
        if (sfxSource[0].isPlaying)
            usedAudioSource = sfxSource[1];
        else
            usedAudioSource = sfxSource[0];

        switch(sfx)
        {
            case "confirm":
                usedAudioSource.volume = 0.1f;
                usedAudioSource.clip = confirmClip;
                usedAudioSource.Play();
                break;
            case "cancel":
                usedAudioSource.volume = 0.1f;
                usedAudioSource.clip = cancelClip;
                usedAudioSource.Play();
                break;
            default:
                Debug.Log("SFX is not defined");
                break;
        }
    }
}

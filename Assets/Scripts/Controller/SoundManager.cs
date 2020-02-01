using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    [SerializeField]
    AudioSource fxSource;
    [SerializeField]
    AudioSource musicSource;
    [SerializeField]
    float lowPitchRange = 0.95f;
    [SerializeField]
    float hightPitchRange = 1.05f;

    #region Singleton
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    public static SoundManager Instance { get { return instance; } }
    #endregion Singleton

    public void playSingle(AudioClip clip)
    {
        fxSource.clip = clip;
        fxSource.Play();
    }
    public void randomizedSfx(AudioClip[] clips)
    {
        int rand = Random.Range(0, clips.Length);
        float randPitch = Random.Range(lowPitchRange, hightPitchRange);
        fxSource.pitch = randPitch;
        fxSource.clip = clips[rand];
        fxSource.Play();
    }
    public void playMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }
}

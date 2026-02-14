using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource voiceSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip correctAnswer;
    [SerializeField] private AudioClip wrongAnswer;
    [SerializeField] private AudioClip catchDessert;
    [SerializeField] private AudioClip dropStrawberry;

    private Dictionary<string, AudioClip> sfxClips;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeSfxDictionary();
    }

    private void InitializeSfxDictionary()
    {
        sfxClips = new Dictionary<string, AudioClip>
        {
            { "button", buttonClick },
            { "correct", correctAnswer },
            { "wrong", wrongAnswer },
            { "catch", catchDessert },
            { "drop", dropStrawberry }
        };
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(string sfxName)
    {
        if (sfxClips.TryGetValue(sfxName, out var clip) && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayVoice(AudioClip clip)
    {
        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    public void SetVolume(float bgm, float sfx, float voice)
    {
        bgmSource.volume = Mathf.Clamp01(bgm);
        sfxSource.volume = Mathf.Clamp01(sfx);
        voiceSource.volume = Mathf.Clamp01(voice);
    }
}

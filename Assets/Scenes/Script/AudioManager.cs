using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;
    public AudioSource sfxSource2;

    #region AudioClip
    [Header("Clips")]
    public AudioClip spinStart;
    public AudioClip reelStop;
    public AudioClip win;
    public AudioClip freeSpin;
    public AudioClip buttonClick;
    #endregion

    private bool isMuted = false;

    public bool IsMuted => isMuted;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void SpinSound(bool play)
    {
        if (isMuted) return;

        if (play && !sfxSource2.isPlaying)
            sfxSource2.Play();
        else if (!play && sfxSource2.isPlaying)
            sfxSource2.Stop();
    }

    public void SetMute(bool mute)
    {
        isMuted = mute;
        sfxSource.mute = mute;
        sfxSource2.mute = mute;
    }
    
}
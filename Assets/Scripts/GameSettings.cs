using UnityEngine;

/// <summary>
/// Match settings: rhythm speed and custom audio clips.
/// Assign music/tick clips in the Inspector on GameController.
/// </summary>
public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [Header("Rhythm speed")]
    public float rhythmBpm = 80f;
    public float minBpm = 60f;
    public float maxBpm = 120f;

    [Header("Custom audio (drag clips here in Inspector)")]
    [Tooltip("Optional song that loops during a match.")]
    public AudioClip backgroundMusic;

    [Tooltip("Optional click on each beat. Uses BeatConductor if empty.")]
    public AudioClip metronomeTick;

    [Header("UI")]
    public Sprite menuLogo;

    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float tickVolume = 0.7f;

    private AudioSource _musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SetupMusicSource();
    }

    public void SetBpm(float bpm)
    {
        rhythmBpm = Mathf.Clamp(bpm, minBpm, maxBpm);

        if (BeatConductor.Instance != null)
        {
            BeatConductor.Instance.SetBpm(rhythmBpm);
        }
    }

    public void ApplyToBeatConductor()
    {
        if (BeatConductor.Instance == null) return;

        BeatConductor.Instance.SetBpm(rhythmBpm);

        if (metronomeTick != null)
        {
            BeatConductor.Instance.tickClip = metronomeTick;
        }

        BeatConductor.Instance.SetTickVolume(tickVolume);
    }

    public void StartMatchAudio()
    {
        ApplyToBeatConductor();
        BeatConductor.Instance.SetRunning(true);
        BeatConductor.Instance.ResetBeatClock();

        PlayBackgroundMusic();
    }

    public void StopMatchAudio()
    {
        if (BeatConductor.Instance != null)
        {
            BeatConductor.Instance.SetRunning(false);
        }

        StopBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (_musicSource == null || backgroundMusic == null) return;

        _musicSource.clip = backgroundMusic;
        _musicSource.volume = musicVolume;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        if (_musicSource == null) return;
        _musicSource.Stop();
    }

    private void SetupMusicSource()
    {
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
    }
}

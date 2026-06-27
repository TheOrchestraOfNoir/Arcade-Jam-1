using UnityEngine;

/// <summary>
/// Match settings: rhythm speed and custom audio clips.
/// </summary>
public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    private const string DefaultMusicResourcePath = "Music/FightMusic";

    [Header("Rhythm speed")]
    [Tooltip("BPM of the background music. Tune in Inspector if rhythm feels early/late.")]
    public float rhythmBpm = 120f;

    [Tooltip("Seconds into the song before the first gameplay beat hits.")]
    public float musicBeatOffset = 0f;

    [Header("Custom audio")]
    [Tooltip("Song that loops during a match. Rhythm timing syncs to this track.")]
    public AudioClip backgroundMusic;

    [Tooltip("Optional click on each beat. Left empty when using full music.")]
    public AudioClip metronomeTick;

    [Header("UI")]
    public Sprite menuLogo;
    public Sprite rhythmCloudSprite;

    [Range(0f, 1f)] public float musicVolume = 0.75f;
    [Range(0f, 1f)] public float tickVolume = 0.35f;

    private AudioSource _musicSource;

    public AudioSource MusicSource => _musicSource;

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

    private void Start()
    {
        if (backgroundMusic == null)
        {
            backgroundMusic = Resources.Load<AudioClip>(DefaultMusicResourcePath);
        }
    }

    public void SetBpm(float bpm)
    {
        rhythmBpm = Mathf.Max(1f, bpm);

        if (BeatConductor.Instance != null)
        {
            BeatConductor.Instance.SetBpm(rhythmBpm);
        }
    }

    public void ApplyToBeatConductor()
    {
        if (BeatConductor.Instance == null) return;

        BeatConductor.Instance.SetBpm(rhythmBpm);
        BeatConductor.Instance.SetMusicSyncSource(_musicSource, musicBeatOffset);

        if (metronomeTick != null)
        {
            BeatConductor.Instance.tickClip = metronomeTick;
        }

        BeatConductor.Instance.SetTickVolume(backgroundMusic != null ? tickVolume * 0.25f : tickVolume);
    }

    public void StartMatchAudio()
    {
        ApplyToBeatConductor();
        EnsureMusicPlaying();

        if (BeatConductor.Instance != null)
        {
            BeatConductor.Instance.ResetBeatClock();
            BeatConductor.Instance.SetRunning(true);
        }
    }

    public void StopMatchAudio()
    {
        if (BeatConductor.Instance != null)
        {
            BeatConductor.Instance.SetRunning(false);
            BeatConductor.Instance.SetMusicSyncSource(null, 0f);
        }

        StopBackgroundMusic();
    }

    public void EnsureMusicPlaying()
    {
        if (_musicSource == null || backgroundMusic == null) return;

        _musicSource.clip = backgroundMusic;
        _musicSource.volume = musicVolume;
        _musicSource.loop = true;
        _musicSource.spatialBlend = 0f;
        _musicSource.priority = 32;

        if (!_musicSource.isPlaying)
        {
            _musicSource.Play();
        }

        if (BeatConductor.Instance != null)
        {
            BeatConductor.Instance.SetMusicSyncSource(_musicSource, musicBeatOffset);
        }
    }

    public void PlayBackgroundMusic()
    {
        if (_musicSource == null || backgroundMusic == null) return;

        _musicSource.clip = backgroundMusic;
        _musicSource.volume = musicVolume;
        _musicSource.loop = true;
        _musicSource.spatialBlend = 0f;
        _musicSource.time = 0f;
        _musicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        if (_musicSource == null) return;
        _musicSource.Stop();
    }

    private void SetupMusicSource()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length >= 2)
        {
            _musicSource = sources[1];
        }
        else
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
        }

        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
        _musicSource.spatialBlend = 0f;
        _musicSource.priority = 32;
    }
}

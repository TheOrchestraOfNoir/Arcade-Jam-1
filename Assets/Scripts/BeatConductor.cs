using System;
using UnityEngine;

/// <summary>
/// Keeps time to the beat — synced to background music when available.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BeatConductor : MonoBehaviour
{
    public enum BeatRating
    {
        Miss,
        Bad,
        Good,
        Perfect
    }

    public static BeatConductor Instance { get; private set; }

    [Header("Timing")]
    public float bpm = 120f;

    [Tooltip("Within this many seconds = Perfect")]
    public float perfectWindowSeconds = 0.08f;

    [Tooltip("Within this many seconds = Good")]
    public float goodWindowSeconds = 0.14f;

    [Tooltip("Within this many seconds = Bad")]
    public float badWindowSeconds = 0.2f;

    [Header("Sound (optional)")]
    public AudioClip tickClip;

    public event Action OnBeat;

    private AudioSource _tickSource;
    private AudioSource _musicSyncSource;
    private float _musicBeatOffset;
    private float _tickVolume = 0.7f;
    private float _secondsPerBeat;
    private float _nextBeatTime;
    private float _lastBeatTime;
    private int _musicBeatIndex = -1;
    private bool _isRunning;

    public float SecondsPerBeat => _secondsPerBeat;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _tickSource = GetComponent<AudioSource>();
        RecalculateBeatLength();
    }

    private void Start()
    {
        _lastBeatTime = -999f;
        _nextBeatTime = Time.time + _secondsPerBeat;
        _isRunning = false;

        if (GameSettings.Instance != null)
        {
            SetBpm(GameSettings.Instance.rhythmBpm);
            if (GameSettings.Instance.metronomeTick != null)
            {
                tickClip = GameSettings.Instance.metronomeTick;
            }
            SetTickVolume(GameSettings.Instance.tickVolume);
        }
    }

    private void Update()
    {
        if (!_isRunning) return;

        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.EnsureMusicPlaying();
        }

        if (IsMusicSyncActive())
        {
            UpdateMusicSyncedBeats();
            return;
        }

        if (Time.time < _nextBeatTime) return;

        _lastBeatTime = _nextBeatTime;
        _nextBeatTime += _secondsPerBeat;

        PlayTick();
        OnBeat?.Invoke();
    }

    public BeatRating RateInput()
    {
        float difference = GetSecondsFromNearestBeat();
        if (difference < 0f) return BeatRating.Miss;

        if (difference <= perfectWindowSeconds) return BeatRating.Perfect;
        if (difference <= goodWindowSeconds) return BeatRating.Good;
        if (difference <= badWindowSeconds) return BeatRating.Bad;
        return BeatRating.Miss;
    }

    public bool IsOnBeat()
    {
        BeatRating rating = RateInput();
        return rating == BeatRating.Perfect || rating == BeatRating.Good;
    }

    public void SetRunning(bool running)
    {
        _isRunning = running;
    }

    public void SetBpm(float newBpm)
    {
        bpm = Mathf.Max(1f, newBpm);
        RecalculateBeatLength();
    }

    public void SetTickVolume(float volume)
    {
        _tickVolume = Mathf.Clamp01(volume);
    }

    public void SetMusicSyncSource(AudioSource musicSource, float beatOffset)
    {
        _musicSyncSource = musicSource;
        _musicBeatOffset = beatOffset;
        _musicBeatIndex = -1;
    }

    public void ResetBeatClock()
    {
        _lastBeatTime = -999f;
        _nextBeatTime = Time.time + _secondsPerBeat;
        _musicBeatIndex = -1;
    }

    private bool IsMusicSyncActive()
    {
        return _musicSyncSource != null && _musicSyncSource.isPlaying && _musicSyncSource.clip != null;
    }

    private float GetSongBeatTime()
    {
        return _musicSyncSource.time - _musicBeatOffset;
    }

    private float GetSecondsFromNearestBeat()
    {
        if (IsMusicSyncActive())
        {
            float songBeatTime = GetSongBeatTime();
            if (songBeatTime < 0f) return -1f;

            float phase = songBeatTime % _secondsPerBeat;
            return Mathf.Min(phase, _secondsPerBeat - phase);
        }

        return Mathf.Abs(Time.time - _lastBeatTime);
    }

    private void UpdateMusicSyncedBeats()
    {
        float songBeatTime = GetSongBeatTime();
        if (songBeatTime < 0f) return;

        int beatIndex = Mathf.FloorToInt(songBeatTime / _secondsPerBeat);
        if (beatIndex <= _musicBeatIndex) return;

        _musicBeatIndex = beatIndex;
        _lastBeatTime = Time.time;

        PlayTick();
        OnBeat?.Invoke();
    }

    private void RecalculateBeatLength()
    {
        _secondsPerBeat = 60f / bpm;
    }

    private void PlayTick()
    {
        if (tickClip == null) return;
        _tickSource.PlayOneShot(tickClip, _tickVolume);
    }
}

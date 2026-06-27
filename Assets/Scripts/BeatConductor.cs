using System;
using UnityEngine;

/// <summary>
/// Keeps time to the beat — like a metronome.
/// Other scripts ask: "Did we just hit a beat?" and "How good was the timing?"
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
    public float perfectWindowSeconds = 0.05f;

    [Tooltip("Within this many seconds = Good")]
    public float goodWindowSeconds = 0.1f;

    [Tooltip("Within this many seconds = Bad")]
    public float badWindowSeconds = 0.15f;

    [Header("Sound (optional)")]
    public AudioClip tickClip;

    public event Action OnBeat;

    private AudioSource _audioSource;
    private float _secondsPerBeat;
    private float _nextBeatTime;
    private float _lastBeatTime;
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
        _audioSource = GetComponent<AudioSource>();
        RecalculateBeatLength();
    }

    private void Start()
    {
        _lastBeatTime = -999f;
        _nextBeatTime = Time.time + _secondsPerBeat;
        _isRunning = true;
    }

    private void Update()
    {
        if (!_isRunning) return;
        if (Time.time < _nextBeatTime) return;

        _lastBeatTime = _nextBeatTime;
        _nextBeatTime += _secondsPerBeat;

        PlayTick();
        OnBeat?.Invoke();
    }

    /// <summary>
    /// How close the current moment is to the last beat.
    /// </summary>
    public BeatRating RateInput()
    {
        float difference = Mathf.Abs(Time.time - _lastBeatTime);

        if (difference <= perfectWindowSeconds) return BeatRating.Perfect;
        if (difference <= goodWindowSeconds) return BeatRating.Good;
        if (difference <= badWindowSeconds) return BeatRating.Bad;
        return BeatRating.Miss;
    }

    /// <summary>
    /// Quick check used by jump/shoot bonuses — Good or Perfect counts as on-beat.
    /// </summary>
    public bool IsOnBeat()
    {
        BeatRating rating = RateInput();
        return rating == BeatRating.Perfect || rating == BeatRating.Good;
    }

    public void SetRunning(bool running)
    {
        _isRunning = running;
    }

    private void RecalculateBeatLength()
    {
        _secondsPerBeat = 60f / bpm;
    }

    private void PlayTick()
    {
        if (tickClip == null) return;
        _audioSource.PlayOneShot(tickClip);
    }
}

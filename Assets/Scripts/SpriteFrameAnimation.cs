using UnityEngine;

/// <summary>
/// Plays a looping or one-shot sprite sequence on a SpriteRenderer.
/// </summary>
public class SpriteFrameAnimation : MonoBehaviour
{
    public Sprite[] frames;
    public float framesPerSecond = 8f;
    public bool playOnEnable = false;
    public bool loop = true;

    private SpriteRenderer _spriteRenderer;
    private int _frameIndex;
    private float _timer;
    private bool _playing;
    private bool _oneShot;

    public bool IsPlaying => _playing;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        if (playOnEnable && frames != null && frames.Length > 0)
        {
            PlayLoop();
        }
    }

    private void Update()
    {
        if (!_playing || frames == null || frames.Length <= 1 || _spriteRenderer == null) return;

        _timer += Time.deltaTime;
        float frameDuration = 1f / Mathf.Max(framesPerSecond, 1f);

        while (_timer >= frameDuration)
        {
            _timer -= frameDuration;
            _frameIndex++;

            if (_frameIndex >= frames.Length)
            {
                if (_oneShot)
                {
                    _playing = false;
                    return;
                }

                _frameIndex = 0;
            }

            _spriteRenderer.sprite = frames[_frameIndex];
        }
    }

    public void SetFrames(Sprite[] newFrames)
    {
        frames = newFrames;
    }

    public void PlayLoop()
    {
        if (frames == null || frames.Length == 0) return;

        _oneShot = false;
        loop = true;
        _playing = true;
        _frameIndex = 0;
        _timer = 0f;
        _spriteRenderer.sprite = frames[0];
    }

    public void PlayOnce(System.Action onComplete = null)
    {
        if (frames == null || frames.Length == 0) return;

        _oneShot = true;
        loop = false;
        _playing = true;
        _frameIndex = 0;
        _timer = 0f;
        _spriteRenderer.sprite = frames[0];

        if (onComplete != null)
        {
            float duration = frames.Length / Mathf.Max(framesPerSecond, 1f);
            Invoke(nameof(InvokeComplete), duration);
            void InvokeComplete() => onComplete?.Invoke();
        }
    }

    public void Stop()
    {
        _playing = false;
    }
}

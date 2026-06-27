using UnityEngine;

/// <summary>
/// Plays GIF-based sprite strips for stand / walk / jump / rhythm / shoot states.
/// Animation strips live in Resources/Animations/.
/// </summary>
public class PlayerCharacterVisual : MonoBehaviour
{
    [Header("Static")]
    public Sprite standSprite;

    [Header("Animation strips (Resources path, no extension)")]
    public string walkAnimationPath = "Animations/Zeus_Walk";
    public string jumpAnimationPath = "Animations/Zeus_Jump";
    public string rhythmAnimationPath = "Animations/Zeus_Dance";
    public string shootAnimationPath = "Animations/Fireball";

    public float walkFps = 8f;
    public float jumpFps = 8f;
    public float rhythmFps = 8f;
    public float shootFps = 12f;
    public float shootAnimDuration = 0.35f;

    private SpriteRenderer _spriteRenderer;
    private SpriteFrameAnimation _animator;
    private Rigidbody2D _rigidbody;

    private Sprite[] _walkFrames;
    private Sprite[] _jumpFrames;
    private Sprite[] _rhythmFrames;
    private Sprite[] _shootFrames;
    private Sprite[] _currentFrames;

    private bool _shooting;
    private float _shootTimer;

    private void Start()
    {
        Transform spriteChild = transform.Find("Sprite");
        if (spriteChild != null)
        {
            _spriteRenderer = spriteChild.GetComponent<SpriteRenderer>();
            _animator = spriteChild.GetComponent<SpriteFrameAnimation>();
            if (_animator == null)
            {
                _animator = spriteChild.gameObject.AddComponent<SpriteFrameAnimation>();
            }
        }

        _rigidbody = GetComponent<Rigidbody2D>();

        _walkFrames = SpriteAnimationLibrary.LoadStrip(walkAnimationPath);
        _jumpFrames = SpriteAnimationLibrary.LoadStrip(jumpAnimationPath);
        _rhythmFrames = SpriteAnimationLibrary.LoadStrip(rhythmAnimationPath);
        _shootFrames = SpriteAnimationLibrary.LoadStrip(shootAnimationPath);

        ShowStand();
    }

    private void Update()
    {
        if (_spriteRenderer == null || _rigidbody == null || GameState.Instance == null) return;

        if (_shooting)
        {
            _shootTimer -= Time.deltaTime;
            if (_shootTimer <= 0f)
            {
                _shooting = false;
            }
            else
            {
                return;
            }
        }

        if (GameState.Instance.gameState == GameState.GameStateEnum.RhythmDuel)
        {
            PlayLoop(_rhythmFrames, rhythmFps);
            return;
        }

        float horizontalSpeed = _rigidbody.velocity.x;
        float verticalSpeed = _rigidbody.velocity.y;

        if (horizontalSpeed > 0.05f) _spriteRenderer.flipX = false;
        else if (horizontalSpeed < -0.05f) _spriteRenderer.flipX = true;

        if (verticalSpeed > 0.15f && _jumpFrames != null && _jumpFrames.Length > 0)
        {
            PlayLoop(_jumpFrames, jumpFps);
        }
        else if (Mathf.Abs(horizontalSpeed) > 0.1f && _walkFrames != null && _walkFrames.Length > 0)
        {
            PlayLoop(_walkFrames, walkFps);
        }
        else
        {
            ShowStand();
        }
    }

    public void PlayShootAnimation()
    {
        if (_shootFrames == null || _shootFrames.Length == 0 || _animator == null) return;

        _shooting = true;
        _shootTimer = shootAnimDuration;
        _animator.framesPerSecond = shootFps;
        _animator.SetFrames(_shootFrames);
        _animator.PlayLoop();
    }

    private void PlayLoop(Sprite[] frames, float fps)
    {
        if (frames == null || frames.Length == 0 || _animator == null) return;

        if (_currentFrames == frames && _animator.IsPlaying) return;

        _currentFrames = frames;
        _animator.framesPerSecond = fps;
        _animator.SetFrames(frames);
        _animator.PlayLoop();
    }

    private void ShowStand()
    {
        _currentFrames = null;

        if (_animator != null)
        {
            _animator.Stop();
        }

        if (_spriteRenderer != null && standSprite != null)
        {
            _spriteRenderer.sprite = standSprite;
            _spriteRenderer.color = Color.white;
        }
    }
}

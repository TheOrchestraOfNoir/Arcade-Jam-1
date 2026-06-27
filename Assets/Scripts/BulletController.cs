using UnityEngine;

public class BulletController : MonoBehaviour {
    private Rigidbody2D _rigidbody2D;
    private Vector2 _direction = Vector2.right;
    private SpriteRenderer _spriteRenderer;
    private SpriteFrameAnimation _spriteAnimation;
    private float _ignoreOwnerUntil;

    public float delayBeforeShrink = 2f;
    public float shrinkDuration = 1f;
    public int speed = 5;

    [Header("Visual")]
    public float visualScale = 0.55f;
    public int sortingOrder = 10;

    [Header("Effects")]
    public string fireballAnimationPath = "Animations/Fireball";
    public string explosionAnimationPath = "Animations/Explosion";
    public float fireballFps = 12f;
    public float explosionFps = 12f;
    public float explosionLifetime = 0.5f;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        SetupVisual();
    }

    private void Start()
    {
        StartCoroutine(ShrinkRoutine());
    }

    private void SetupVisual()
    {
        Transform spriteChild = transform.Find("Sprite");
        if (spriteChild == null) return;

        _spriteRenderer = spriteChild.GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null) return;

        _spriteRenderer.sortingOrder = sortingOrder;
        _spriteRenderer.color = Color.white;
        spriteChild.localScale = Vector3.one * visualScale;

        Sprite[] fireballFrames = SpriteAnimationLibrary.LoadStrip(fireballAnimationPath);
        if (fireballFrames != null && fireballFrames.Length > 0)
        {
            _spriteRenderer.sprite = fireballFrames[0];

            _spriteAnimation = spriteChild.GetComponent<SpriteFrameAnimation>();
            if (_spriteAnimation == null)
            {
                _spriteAnimation = spriteChild.gameObject.AddComponent<SpriteFrameAnimation>();
            }

            _spriteAnimation.framesPerSecond = fireballFps;
            _spriteAnimation.SetFrames(fireballFrames);
            _spriteAnimation.PlayLoop();
            return;
        }

        Sprite fallback = Resources.Load<Sprite>(fireballAnimationPath + "_0");
        if (fallback != null)
        {
            _spriteRenderer.sprite = fallback;
        }
    }

    public void SetOwner(Collider2D ownerCollider)
    {
        if (ownerCollider == null) return;

        Collider2D bulletCollider = GetComponent<Collider2D>();
        if (bulletCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, ownerCollider, true);
        }

        _ignoreOwnerUntil = Time.time + 0.15f;
    }

    public void SetDirection(Vector2 dir) {
        _direction = dir.normalized;
    }

    private void Update() {
        if (_rigidbody2D != null)
        {
            _rigidbody2D.velocity = _direction * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (Time.time < _ignoreOwnerUntil) return;
        if (collision.isTrigger) return;

        SpawnExplosion();
        Destroy(gameObject);
    }

    private void SpawnExplosion()
    {
        Sprite[] frames = SpriteAnimationLibrary.LoadStrip(explosionAnimationPath);
        if (frames == null || frames.Length == 0) return;

        GameObject fx = new GameObject("Explosion");
        fx.transform.position = transform.position;
        fx.transform.localScale = Vector3.one * 1.5f;

        SpriteRenderer renderer = fx.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = sortingOrder + 1;
        renderer.sprite = frames[0];

        SpriteFrameAnimation anim = fx.AddComponent<SpriteFrameAnimation>();
        anim.framesPerSecond = explosionFps;
        anim.SetFrames(frames);
        anim.PlayOnce();

        Destroy(fx, explosionLifetime);
    }

    private System.Collections.IEnumerator ShrinkRoutine()
    {
        yield return new WaitForSeconds(delayBeforeShrink);

        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;
        float elapsedTime = 0f;

        while (elapsedTime < shrinkDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / shrinkDuration);
            yield return null;
        }

        transform.localScale = targetScale;
        Destroy(gameObject);
    }
}

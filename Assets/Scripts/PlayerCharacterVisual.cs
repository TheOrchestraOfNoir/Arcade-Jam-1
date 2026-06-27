using UnityEngine;

/// <summary>
/// Swaps the player sprite for stand / walk / jump art.
/// Assign Zeus sprites on Player 1 and Poseidon sprites on Player 2 in the Inspector.
/// </summary>
public class PlayerCharacterVisual : MonoBehaviour
{
    public Sprite standSprite;
    public Sprite walkSprite;
    public Sprite jumpSprite;
    public Sprite danceSprite;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;

    private void Start()
    {
        Transform spriteChild = transform.Find("Sprite");
        if (spriteChild != null) _spriteRenderer = spriteChild.GetComponent<SpriteRenderer>();

        _rigidbody = GetComponent<Rigidbody2D>();

        if (_spriteRenderer != null && standSprite != null)
        {
            _spriteRenderer.sprite = standSprite;
            _spriteRenderer.color = Color.white;
        }
    }

    private void Update()
    {
        if (_spriteRenderer == null || _rigidbody == null) return;
        if (GameState.Instance == null) return;

        if (GameState.Instance.gameState == GameState.GameStateEnum.RhythmDuel)
        {
            if (danceSprite != null)
            {
                _spriteRenderer.sprite = danceSprite;
            }
            return;
        }

        float horizontalSpeed = _rigidbody.velocity.x;
        float verticalSpeed = _rigidbody.velocity.y;

        if (horizontalSpeed > 0.05f) _spriteRenderer.flipX = false;
        else if (horizontalSpeed < -0.05f) _spriteRenderer.flipX = true;

        if (verticalSpeed > 0.15f && jumpSprite != null)
        {
            _spriteRenderer.sprite = jumpSprite;
        }
        else if (Mathf.Abs(horizontalSpeed) > 0.1f && walkSprite != null)
        {
            _spriteRenderer.sprite = walkSprite;
        }
        else if (standSprite != null)
        {
            _spriteRenderer.sprite = standSprite;
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerActions : MonoBehaviour {
    public string playerCount = "1";
    
    private Vector3 _start;
    private Rigidbody2D _rigidbody;
    private PlayerWeapon _playerWeapon;
    
    public GameObject xObject;
    public Color bulletColor; 
    public LayerMask layersToExclude;
    public float spawnInterval = 2f;
    public float currentTime = 0f;
    
    private bool _canShoot = true;
    
    private void Start()
    {
        playerCount = NormalizePlayerId(playerCount);

        _start = gameObject.transform.position;
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerWeapon = GetComponent<PlayerWeapon>();
    }

    private void Update() {
        if (GameState.Instance == null) return;

        switch (GameState.Instance.gameState) {
            
            case GameState.GameStateEnum.GetReady: {
                if (Input.GetButtonDown(GameState.Instance.jumpButton + playerCount)) {
                    GameState.Instance.SetReady(playerCount);
                }
                break;
            }
            
            case GameState.GameStateEnum.InMatch: {
                if (Input.GetButtonDown(GameState.Instance.actionX + playerCount)) {
                    TryShoot();
                }

                if (Input.GetButtonDown(GameState.Instance.actionY + playerCount)) {
                    if (RhythmDuelManager.Instance != null) {
                        RhythmDuelManager.Instance.StartDuel();
                    }
                }

                if (!_canShoot) {
                    currentTime -= Time.deltaTime;
                    if (currentTime < 0) {
                        _canShoot = true;
                    }
                }
                break;
            }
            
            case GameState.GameStateEnum.RhythmDuel: {
                if (PlayerDirectionInput.TryGetDirectionPress(playerCount, out int horizontal, out int vertical)) {
                    if (RhythmDuelManager.Instance != null) {
                        RhythmDuelManager.Instance.TryRegisterInput(playerCount, horizontal, vertical);
                    }
                }
                break;
            }

            case GameState.GameStateEnum.GameOver: {
                if (Input.GetButtonDown(GameState.Instance.jumpButton + playerCount)) {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                break;
            }
        }
    }

    private void TryShoot() {
        if (!_canShoot) return;
        if (xObject == null || _playerWeapon == null || _playerWeapon.weapon == null) return;

        currentTime = spawnInterval;
        _canShoot = false;

        Transform spawnPoint = _playerWeapon.weapon.transform;
        GameObject newObject = Instantiate(xObject, spawnPoint.position, spawnPoint.rotation);

        Transform sprite = newObject.transform.Find("Sprite");
        if (sprite != null) {
            SpriteRenderer spriteRenderer = sprite.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) spriteRenderer.color = bulletColor;
        }

        Rigidbody2D bulletBody = newObject.GetComponent<Rigidbody2D>();
        if (bulletBody != null) bulletBody.excludeLayers = layersToExclude;

        BulletController bullet = newObject.GetComponent<BulletController>();
        if (bullet != null) {
            bullet.SetDirection(_playerWeapon.direction);

            if (BeatConductor.Instance != null && BeatConductor.Instance.IsOnBeat()) {
                bullet.speed = Mathf.RoundToInt(bullet.speed * 1.5f);
                currentTime = spawnInterval * 0.5f;
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision) { 
        if (GameState.Instance == null) return;

        if (collision.CompareTag("Death")) {
            transform.position = _start;
            _rigidbody.velocity = Vector2.zero;
            GameState.Instance.TakeDamage(playerCount);
        }
    }

    private static string NormalizePlayerId(string rawId) {
        if (string.IsNullOrWhiteSpace(rawId)) return "1";
        rawId = rawId.Trim();
        return rawId == "2" ? "2" : "1";
    }
}

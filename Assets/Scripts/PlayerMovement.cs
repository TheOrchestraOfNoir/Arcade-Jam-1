using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    // Movement speed modifier for the player
    public float speed = 5;
    
    // Reference to the 2D Rigidbody component for physics-based movement
    private Rigidbody2D _rigidbody2D;
    
    // Reference to the player actions script to determine player index/ID
    private PlayerActions _playerActions;
    private PlayerBuffs _playerBuffs;
    
    private void Start() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerActions = GetComponent<PlayerActions>();
        _playerBuffs = GetComponent<PlayerBuffs>();
    }

    // Update is called once per frame
    private void Update() {
        if (GameState.Instance == null || _playerActions == null || _rigidbody2D == null) return;
        if (GameState.Instance.gameState != GameState.GameStateEnum.InMatch) return;
        
        // Get horizontal input value (-1, 0, or 1) using the player's specific control axis
        float horizontal = Input.GetAxisRaw(GameState.Instance.horizontalAxis + _playerActions.playerCount);
            
        // Apply horizontal velocity while preserving the current vertical velocity
        float moveSpeed = speed;
        if (_playerBuffs != null) moveSpeed *= _playerBuffs.speedMultiplier;

        _rigidbody2D.velocity = new Vector2(horizontal * moveSpeed, _rigidbody2D.velocity.y);
    }
}
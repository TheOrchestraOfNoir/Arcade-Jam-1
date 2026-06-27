using UnityEngine;

/// <summary>
/// Reads direction presses for Player 1 and Player 2.
/// Keyboard keys are checked first, then Input Manager axes (also covers gamepads).
/// </summary>
public static class PlayerDirectionInput
{
    private static int _playerOneLastH;
    private static int _playerOneLastV;
    private static int _playerTwoLastH;
    private static int _playerTwoLastV;

    public static bool TryGetDirectionPress(string playerNumber, out int horizontal, out int vertical)
    {
        horizontal = 0;
        vertical = 0;

        playerNumber = playerNumber == "2" ? "2" : "1";

        if (TryGetKeyboardPress(playerNumber, out horizontal, out vertical))
        {
            return true;
        }

        return TryGetAxisPress(playerNumber, out horizontal, out vertical);
    }

    public static void ResetStickMemory()
    {
        _playerOneLastH = 0;
        _playerOneLastV = 0;
        _playerTwoLastH = 0;
        _playerTwoLastV = 0;
    }

    private static bool TryGetKeyboardPress(string playerNumber, out int horizontal, out int vertical)
    {
        horizontal = 0;
        vertical = 0;

        if (playerNumber == "2")
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.I)) { vertical = 1; return true; }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.K)) { vertical = -1; return true; }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.J)) { horizontal = -1; return true; }
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.L)) { horizontal = 1; return true; }
            return false;
        }

        if (Input.GetKeyDown(KeyCode.W)) { vertical = 1; return true; }
        if (Input.GetKeyDown(KeyCode.S)) { vertical = -1; return true; }
        if (Input.GetKeyDown(KeyCode.A)) { horizontal = -1; return true; }
        if (Input.GetKeyDown(KeyCode.D)) { horizontal = 1; return true; }
        return false;
    }

    private static bool TryGetAxisPress(string playerNumber, out int horizontal, out int vertical)
    {
        horizontal = 0;
        vertical = 0;

        if (GameState.Instance == null) return false;

        string horizontalAxis = GameState.Instance.horizontalAxis + playerNumber;
        string verticalAxis = GameState.Instance.verticalAxis + playerNumber;

        float rawH = Input.GetAxisRaw(horizontalAxis);
        float rawV = Input.GetAxisRaw(verticalAxis);

        if (Mathf.Abs(rawH) < 0.5f) rawH = 0f;
        if (Mathf.Abs(rawV) < 0.5f) rawV = 0f;

        horizontal = rawH > 0 ? 1 : rawH < 0 ? -1 : 0;
        vertical = rawV > 0 ? 1 : rawV < 0 ? -1 : 0;

        if (horizontal != 0 && vertical != 0)
        {
            if (Mathf.Abs(horizontal) >= Mathf.Abs(vertical)) vertical = 0;
            else horizontal = 0;
        }

        if (horizontal == 0 && vertical == 0) return false;

        int lastH;
        int lastV;
        if (playerNumber == "2")
        {
            lastH = _playerTwoLastH;
            lastV = _playerTwoLastV;
            _playerTwoLastH = horizontal;
            _playerTwoLastV = vertical;
        }
        else
        {
            lastH = _playerOneLastH;
            lastV = _playerOneLastV;
            _playerOneLastH = horizontal;
            _playerOneLastV = vertical;
        }

        bool hadInputBefore = lastH != 0 || lastV != 0;
        return !hadInputBefore;
    }
}

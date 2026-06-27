using UnityEngine;

/// <summary>
/// Direction input for rhythm duels: keyboard, gamepad D-pad buttons, D-pad axes, and stick.
/// </summary>
public static class PlayerDirectionInput
{
    private static int _playerOneLastStickH;
    private static int _playerOneLastStickV;
    private static int _playerTwoLastStickH;
    private static int _playerTwoLastStickV;

    private static int _playerOneLastDpadH;
    private static int _playerOneLastDpadV;
    private static int _playerTwoLastDpadH;
    private static int _playerTwoLastDpadV;

    // Common Xbox / Switch Pro / generic D-pad button layouts (index = up, down, left, right).
    private static readonly int[][] DpadButtonMaps =
    {
        new[] { 14, 11, 13, 12 },
        new[] { 15, 12, 14, 13 },
        new[] { 8, 9, 10, 11 },
        new[] { 4, 5, 6, 7 },
    };

    public static bool TryGetDirectionPress(string playerNumber, out int horizontal, out int vertical)
    {
        horizontal = 0;
        vertical = 0;

        playerNumber = playerNumber == "2" ? "2" : "1";

        if (TryGetKeyboardPress(playerNumber, out horizontal, out vertical))
        {
            return true;
        }

        if (TryGetRhythmButtonPress(playerNumber, out horizontal, out vertical))
        {
            return true;
        }

        if (TryGetGamepadDpadButtonPress(playerNumber, out horizontal, out vertical))
        {
            return true;
        }

        if (TryGetDpadAxisPress(playerNumber, out horizontal, out vertical))
        {
            return true;
        }

        return TryGetStickPress(playerNumber, out horizontal, out vertical);
    }

    public static void ResetStickMemory()
    {
        _playerOneLastStickH = 0;
        _playerOneLastStickV = 0;
        _playerTwoLastStickH = 0;
        _playerTwoLastStickV = 0;
        _playerOneLastDpadH = 0;
        _playerOneLastDpadV = 0;
        _playerTwoLastDpadH = 0;
        _playerTwoLastDpadV = 0;
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

    private static bool TryGetRhythmButtonPress(string playerNumber, out int horizontal, out int vertical)
    {
        horizontal = 0;
        vertical = 0;

        string suffix = playerNumber;

        if (Input.GetButtonDown("RhythmUp_" + suffix)) { vertical = 1; return true; }
        if (Input.GetButtonDown("RhythmDown_" + suffix)) { vertical = -1; return true; }
        if (Input.GetButtonDown("RhythmLeft_" + suffix)) { horizontal = -1; return true; }
        if (Input.GetButtonDown("RhythmRight_" + suffix)) { horizontal = 1; return true; }

        return false;
    }

    private static bool TryGetGamepadDpadButtonPress(string playerNumber, out int horizontal, out int vertical)
    {
        horizontal = 0;
        vertical = 0;

        int joyIndex = playerNumber == "2" ? 2 : 1;
        KeyCode baseButton = joyIndex == 2 ? KeyCode.Joystick2Button0 : KeyCode.Joystick1Button0;

        foreach (int[] map in DpadButtonMaps)
        {
            if (Input.GetKeyDown(baseButton + map[0])) { vertical = 1; return true; }
            if (Input.GetKeyDown(baseButton + map[1])) { vertical = -1; return true; }
            if (Input.GetKeyDown(baseButton + map[2])) { horizontal = -1; return true; }
            if (Input.GetKeyDown(baseButton + map[3])) { horizontal = 1; return true; }
        }

        return false;
    }

    private static bool TryGetDpadAxisPress(string playerNumber, out int horizontal, out int vertical)
    {
        horizontal = 0;
        vertical = 0;

        float rawH = Input.GetAxisRaw("DpadHorizontal_" + playerNumber);
        float rawV = Input.GetAxisRaw("DpadVertical_" + playerNumber);

        if (Mathf.Abs(rawH) < 0.35f) rawH = 0f;
        if (Mathf.Abs(rawV) < 0.35f) rawV = 0f;

        // Some drivers expose D-pad on 9th/10th axis instead of 7th/8th.
        if (rawH == 0f && rawV == 0f)
        {
            rawH = Input.GetAxisRaw("AltDpadHorizontal_" + playerNumber);
            rawV = Input.GetAxisRaw("AltDpadVertical_" + playerNumber);
            if (Mathf.Abs(rawH) < 0.35f) rawH = 0f;
            if (Mathf.Abs(rawV) < 0.35f) rawV = 0f;
        }

        horizontal = rawH > 0 ? 1 : rawH < 0 ? -1 : 0;
        vertical = rawV > 0 ? 1 : rawV < 0 ? -1 : 0;

        if (horizontal == 0 && vertical == 0) return false;

        if (horizontal != 0 && vertical != 0)
        {
            if (Mathf.Abs(horizontal) >= Mathf.Abs(vertical)) vertical = 0;
            else horizontal = 0;
        }

        return RegisterDpadEdge(playerNumber, horizontal, vertical);
    }

    private static bool RegisterDpadEdge(string playerNumber, int horizontal, int vertical)
    {
        int lastH;
        int lastV;

        if (playerNumber == "2")
        {
            lastH = _playerTwoLastDpadH;
            lastV = _playerTwoLastDpadV;
            _playerTwoLastDpadH = horizontal;
            _playerTwoLastDpadV = vertical;
        }
        else
        {
            lastH = _playerOneLastDpadH;
            lastV = _playerOneLastDpadV;
            _playerOneLastDpadH = horizontal;
            _playerOneLastDpadV = vertical;
        }

        bool wasNeutral = lastH == 0 && lastV == 0;
        return wasNeutral;
    }

    private static bool TryGetStickPress(string playerNumber, out int horizontal, out int vertical)
    {
        horizontal = 0;
        vertical = 0;

        if (GameState.Instance == null) return false;

        float rawH = Input.GetAxisRaw(GameState.Instance.horizontalAxis + playerNumber);
        float rawV = Input.GetAxisRaw(GameState.Instance.verticalAxis + playerNumber);

        if (Mathf.Abs(rawH) < 0.55f) rawH = 0f;
        if (Mathf.Abs(rawV) < 0.55f) rawV = 0f;

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
            lastH = _playerTwoLastStickH;
            lastV = _playerTwoLastStickV;
            _playerTwoLastStickH = horizontal;
            _playerTwoLastStickV = vertical;
        }
        else
        {
            lastH = _playerOneLastStickH;
            lastV = _playerOneLastStickV;
            _playerOneLastStickH = horizontal;
            _playerOneLastStickV = vertical;
        }

        bool wasNeutral = lastH == 0 && lastV == 0;
        return wasNeutral;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyView : MonoBehaviour {
    
    public GameObject startScreen;
    public GameObject inMatchScreen;
    public GameObject gameOverScreen;
    
    public Image backgroundPlayerOne;
    public TextMeshProUGUI readyTextMeshProPlayerOne;
    
    public Image backgroundPlayerTwo;
    public TextMeshProUGUI readyTextMeshProPlayerTwo;
    
    public TextMeshProUGUI healthPlayerOne;
    public TextMeshProUGUI healthPlayerTwo;

    public TextMeshProUGUI playerWins;
    
    public Color backgroundColor = Color.green;

    private GameObject _duelPanel;
    private RectTransform _noteLane;
    private Image _rhythmCloudImage;
    private GameObject _controlsHud;
    private TextMeshProUGUI _duelPromptText;
    private TextMeshProUGUI _duelScorePlayerOne;
    private TextMeshProUGUI _duelScorePlayerTwo;
    private TextMeshProUGUI _duelFeedbackPlayerOne;
    private TextMeshProUGUI _duelFeedbackPlayerTwo;
    private TextMeshProUGUI _duelResultText;

    private const float CloudY = 260f;
    private const float NoteSpawnY = 170f;
    private const float NoteHitY = -50f;

    private MainMenuView _mainMenuView;
    
    private void Start()
    {
        if (startScreen != null) startScreen.SetActive(false);
        if (inMatchScreen != null) inMatchScreen.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);

        BuildDuelUI();
        BuildMainMenu();
        BuildControlsHud();

        if (GameState.Instance != null && GameState.Instance.gameState == GameState.GameStateEnum.MainMenu)
        {
            ShowMainMenu();
        }
    }

    public void ShowMainMenu()
    {
        if (_mainMenuView != null) _mainMenuView.Show();
        if (startScreen != null) startScreen.SetActive(false);
        if (inMatchScreen != null) inMatchScreen.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (_duelPanel != null) _duelPanel.SetActive(false);
        HideControlsHud();
    }

    public void ShowGetReady()
    {
        if (_mainMenuView != null) _mainMenuView.Hide();
        if (startScreen != null) startScreen.SetActive(true);
        if (inMatchScreen != null) inMatchScreen.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        HideControlsHud();
    }

    public void ResetReadyDisplay()
    {
        if (backgroundPlayerOne != null) backgroundPlayerOne.color = Color.white;
        if (backgroundPlayerTwo != null) backgroundPlayerTwo.color = Color.white;
        if (readyTextMeshProPlayerOne != null) readyTextMeshProPlayerOne.text = "Press Jump to ready up";
        if (readyTextMeshProPlayerTwo != null) readyTextMeshProPlayerTwo.text = "Press Jump to ready up";
    }

    public void SetReady(string player) {
        switch (player) {
            case "1": {
                if (backgroundPlayerOne != null) backgroundPlayerOne.color = backgroundColor;
                if (readyTextMeshProPlayerOne != null) readyTextMeshProPlayerOne.text = "Player 1 ready!";
                break;   
            }
            case "2":{
                if (backgroundPlayerTwo != null) backgroundPlayerTwo.color = backgroundColor;
                if (readyTextMeshProPlayerTwo != null) readyTextMeshProPlayerTwo.text = "Player 2 ready!";
                break;   
            }
        }
    }

    public void SetInMatch() {
        if (_mainMenuView != null) _mainMenuView.Hide();
        if (startScreen != null) startScreen.SetActive(false);
        if (inMatchScreen != null) inMatchScreen.SetActive(true);
        ShowControlsHud();
    }

    public void SetInGameOver(string player) {
        if (_mainMenuView != null) _mainMenuView.Hide();
        if (startScreen != null) startScreen.SetActive(false);
        if (inMatchScreen != null) inMatchScreen.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
        if (_duelPanel != null) _duelPanel.SetActive(false);
        HideControlsHud();
        if (playerWins != null) playerWins.text = "Player " + player + " wins!";
    }

    public void UpdatePlayerHealth(string player, int health) {
        switch (player) {
            case "1": {
                if (healthPlayerOne != null) healthPlayerOne.text = health.ToString();
                break;   
            }
            case "2":{
                if (healthPlayerTwo != null) healthPlayerTwo.text = health.ToString();
                break;   
            }
        }
    }

    public void ShowRhythmDuel() {
        if (_duelPanel == null) return;
        _duelPanel.SetActive(true);
        ShowControlsHud();
        _duelResultText.text = "RHYTHM DUEL!";
        ClearLaneNotes();
        ClearFeedback();
    }

    public void HideRhythmDuel() {
        if (_duelPanel == null) return;
        ClearLaneNotes();
        _duelPanel.SetActive(false);
    }

    public void UpdateDuelPrompt(string arrowText) {
        if (_duelPromptText == null) return;
        _duelPromptText.text = arrowText;
    }

    public void UpdateDuelScore(int playerOneScore, int playerTwoScore) {
        if (_duelScorePlayerOne == null || _duelScorePlayerTwo == null) return;
        _duelScorePlayerOne.text = "P1 score: " + playerOneScore;
        _duelScorePlayerTwo.text = "P2 score: " + playerTwoScore;
    }

    public void ShowDuelResult(string message) {
        if (_duelResultText == null) return;
        _duelResultText.text = message;
    }

    public void ShowPlayerFeedback(string player, BeatConductor.BeatRating rating) {
        TextMeshProUGUI target = player == "2" ? _duelFeedbackPlayerTwo : _duelFeedbackPlayerOne;
        if (target == null) return;

        switch (rating) {
            case BeatConductor.BeatRating.Perfect:
                target.text = "PERFECT!";
                target.color = Color.yellow;
                break;
            case BeatConductor.BeatRating.Good:
                target.text = "GOOD";
                target.color = Color.green;
                break;
            case BeatConductor.BeatRating.Bad:
                target.text = "BAD";
                target.color = Color.gray;
                break;
            default:
                target.text = "MISS";
                target.color = Color.red;
                break;
        }
    }

    /// <summary>
    /// Spawns a falling arrow note that reaches the hit line in one beat.
    /// </summary>
    public void SpawnDuelNote(string arrowText) {
        if (_noteLane == null) return;

        float secondsPerBeat = BeatConductor.Instance != null
            ? BeatConductor.Instance.SecondsPerBeat
            : 0.5f;

        float travelDistance = NoteSpawnY - NoteHitY;
        float fallSpeed = travelDistance / secondsPerBeat;

        GameObject noteObject = new GameObject("Rhythm Note");
        noteObject.transform.SetParent(_noteLane, false);

        RectTransform noteRect = noteObject.AddComponent<RectTransform>();
        noteRect.sizeDelta = new Vector2(80f, 80f);
        noteRect.anchoredPosition = new Vector2(0f, NoteSpawnY);

        TextMeshProUGUI noteText = noteObject.AddComponent<TextMeshProUGUI>();
        noteText.text = arrowText;
        noteText.fontSize = 64;
        noteText.alignment = TextAlignmentOptions.Center;
        noteText.color = Color.cyan;
        if (healthPlayerOne != null && healthPlayerOne.font != null) {
            noteText.font = healthPlayerOne.font;
        }

        RhythmNote note = noteObject.AddComponent<RhythmNote>();
        note.arrowText = noteText;
        note.destroyBelowY = NoteHitY - 80f;
        note.Setup(arrowText, fallSpeed);
    }

    private void BuildMainMenu()
    {
        if (startScreen == null) return;

        Transform canvas = startScreen.transform.parent;
        if (canvas == null) return;

        TMP_FontAsset font = healthPlayerOne != null ? healthPlayerOne.font : null;

        _mainMenuView = gameObject.AddComponent<MainMenuView>();
        _mainMenuView.Build(canvas, font);
        _mainMenuView.Hide();
    }

    private void BuildControlsHud()
    {
        if (_controlsHud != null || startScreen == null) return;

        Transform canvas = startScreen.transform.parent;
        if (canvas == null) return;

        TMP_FontAsset font = healthPlayerOne != null ? healthPlayerOne.font : null;

        _controlsHud = new GameObject("Controls HUD");
        _controlsHud.transform.SetParent(canvas, false);

        RectTransform hudRect = _controlsHud.AddComponent<RectTransform>();
        hudRect.anchorMin = new Vector2(0f, 0f);
        hudRect.anchorMax = new Vector2(0f, 0f);
        hudRect.pivot = new Vector2(0f, 0f);
        hudRect.anchoredPosition = new Vector2(12f, 12f);
        hudRect.sizeDelta = new Vector2(360f, 150f);

        Image bg = _controlsHud.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.55f);

        GameObject textObject = new GameObject("Controls Text");
        textObject.transform.SetParent(_controlsHud.transform, false);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(8f, 8f);
        textRect.offsetMax = new Vector2(-8f, -8f);

        TextMeshProUGUI label = textObject.AddComponent<TextMeshProUGUI>();
        label.fontSize = 11;
        label.alignment = TextAlignmentOptions.TopLeft;
        label.color = Color.white;
        label.richText = true;
        if (font != null) label.font = font;
        label.text =
            "<b>CONTROLS</b>\n" +
            "<color=#88CCFF>P1:</color> WASD move | Space jump | R shoot | Y duel | T special\n" +
            "<color=#88CCFF>P2:</color> Arrows/IJKL | Shift jump | N shoot | , duel | M special\n" +
            "<color=#FFEE88>Rhythm:</color> D-pad arrows, left stick, or WASD / IJKL / arrow keys";

        _controlsHud.SetActive(false);
    }

    private void ShowControlsHud()
    {
        if (_controlsHud != null) _controlsHud.SetActive(true);
    }

    private void HideControlsHud()
    {
        if (_controlsHud != null) _controlsHud.SetActive(false);
    }

    private void BuildDuelUI() {
        if (_duelPanel != null || startScreen == null) return;

        Transform canvas = startScreen.transform.parent;

        _duelPanel = new GameObject("Rhythm Duel Panel");
        _duelPanel.transform.SetParent(canvas, false);

        RectTransform panelRect = _duelPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelBackground = _duelPanel.AddComponent<Image>();
        panelBackground.color = new Color(0f, 0f, 0f, 0.65f);

        Sprite cloudSprite = GameSettings.Instance != null ? GameSettings.Instance.rhythmCloudSprite : null;
        if (cloudSprite != null)
        {
            GameObject cloudObject = new GameObject("Rhythm Cloud");
            cloudObject.transform.SetParent(_duelPanel.transform, false);
            RectTransform cloudRect = cloudObject.AddComponent<RectTransform>();
            cloudRect.anchorMin = new Vector2(0.5f, 0.5f);
            cloudRect.anchorMax = new Vector2(0.5f, 0.5f);
            cloudRect.sizeDelta = new Vector2(320f, 160f);
            cloudRect.anchoredPosition = new Vector2(0f, CloudY);

            _rhythmCloudImage = cloudObject.AddComponent<Image>();
            _rhythmCloudImage.sprite = cloudSprite;
            _rhythmCloudImage.preserveAspect = true;
        }

        GameObject laneObject = new GameObject("Note Lane");
        laneObject.transform.SetParent(_duelPanel.transform, false);
        _noteLane = laneObject.AddComponent<RectTransform>();
        _noteLane.anchorMin = new Vector2(0.5f, 0.5f);
        _noteLane.anchorMax = new Vector2(0.5f, 0.5f);
        _noteLane.sizeDelta = new Vector2(120f, 520f);
        _noteLane.anchoredPosition = new Vector2(0f, 20f);

        _duelResultText = CreateDuelText("RHYTHM DUEL!", 32, new Vector2(0f, 320f));
        _duelPromptText = CreateDuelText("?", 72, new Vector2(0f, NoteHitY));
        _duelScorePlayerOne = CreateDuelText("P1 score: 0", 24, new Vector2(-220f, -180f));
        _duelScorePlayerTwo = CreateDuelText("P2 score: 0", 24, new Vector2(220f, -180f));
        _duelFeedbackPlayerOne = CreateDuelText("", 22, new Vector2(-220f, -140f));
        _duelFeedbackPlayerTwo = CreateDuelText("", 22, new Vector2(220f, -140f));

        _duelPanel.SetActive(false);
    }

    private TextMeshProUGUI CreateDuelText(string startingText, float fontSize, Vector2 anchoredPosition) {
        GameObject textObject = new GameObject(startingText + " Text");
        textObject.transform.SetParent(_duelPanel.transform, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400f, 80f);
        rect.anchoredPosition = anchoredPosition;

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = startingText;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        if (healthPlayerOne != null && healthPlayerOne.font != null) {
            text.font = healthPlayerOne.font;
        }

        return text;
    }

    private void ClearLaneNotes() {
        if (_noteLane == null) return;
        for (int i = _noteLane.childCount - 1; i >= 0; i--) {
            Destroy(_noteLane.GetChild(i).gameObject);
        }
    }

    private void ClearFeedback() {
        if (_duelFeedbackPlayerOne != null) _duelFeedbackPlayerOne.text = "";
        if (_duelFeedbackPlayerTwo != null) _duelFeedbackPlayerTwo.text = "";
    }
}

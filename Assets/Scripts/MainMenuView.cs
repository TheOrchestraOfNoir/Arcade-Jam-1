using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime-built title screen: controls, rhythm speed slider, start button.
/// </summary>
public class MainMenuView : MonoBehaviour
{
    private GameObject _menuPanel;
    private Slider _bpmSlider;
    private TextMeshProUGUI _bpmLabel;
    private TextMeshProUGUI _audioHintText;

    public void Build(Transform canvas, TMP_FontAsset font)
    {
        if (_menuPanel != null) return;

        _menuPanel = new GameObject("Main Menu Panel");
        _menuPanel.transform.SetParent(canvas, false);

        RectTransform panelRect = _menuPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image background = _menuPanel.AddComponent<Image>();
        background.color = new Color(0.08f, 0.1f, 0.18f, 0.95f);

        float titleYOffset = 240f;
        Sprite logo = GameSettings.Instance != null ? GameSettings.Instance.menuLogo : null;
        if (logo != null)
        {
            CreateLogoImage(logo, new Vector2(0f, 250f));
            titleYOffset = 170f;
            CreateLabel("Subtitle", "Zeus vs Poseidon — Rhythm Platformer", 20, new Vector2(0f, 190f), font, new Color(0.75f, 0.85f, 1f));
        }
        else
        {
            CreateLabel("Title", "ZEUS vs POSEIDON", 48, new Vector2(0f, titleYOffset), font, Color.white);
            CreateLabel("Subtitle", "Arcade Jam — Rhythm Platformer", 22, new Vector2(0f, 190f), font, new Color(0.75f, 0.85f, 1f));
        }

        CreateLabel("Controls Header", "CONTROLS", 28, new Vector2(0f, 130f), font, Color.yellow);

        string controlsText =
            "<b>Player 1 (Zeus)</b>\n" +
            "Move: WASD  |  Jump: Space  |  Shoot: R  |  Rhythm Duel: Y  |  Special: T\n\n" +
            "<b>Player 2 (Poseidon)</b>\n" +
            "Move: Arrows or IJKL  |  Jump: Right Shift  |  Shoot: N\n" +
            "Rhythm Duel: , (comma)  |  Special: M\n\n" +
            "Gamepads supported (Player 1 / Player 2 axes)";

        CreateLabel("Controls Body", controlsText, 16, new Vector2(0f, 10f), font, Color.white, 720f, 200f);

        float minBpm = GameSettings.Instance != null ? GameSettings.Instance.minBpm : 60f;
        float maxBpm = GameSettings.Instance != null ? GameSettings.Instance.maxBpm : 120f;
        float startBpm = GameSettings.Instance != null ? GameSettings.Instance.rhythmBpm : 80f;

        _bpmLabel = CreateLabel("BPM Label", "Rhythm Speed: " + Mathf.RoundToInt(startBpm) + " BPM", 20,
            new Vector2(0f, -120f), font, Color.cyan);

        _bpmSlider = CreateBpmSlider(new Vector2(0f, -155f), minBpm, maxBpm, startBpm, font);
        _bpmSlider.onValueChanged.AddListener(OnBpmSliderChanged);

        CreateLabel("BPM Hint", "[ and ] keys also adjust speed on this screen", 14,
            new Vector2(0f, -185f), font, new Color(0.7f, 0.7f, 0.7f));

        _audioHintText = CreateLabel("Audio Hint",
            GetAudioHintText(), 14, new Vector2(0f, -215f), font, new Color(0.85f, 0.9f, 0.6f), 700f, 60f);

        CreateStartButton(new Vector2(0f, -280f), font);

        CreateLabel("Start Hint", "Press Space / Enter / either Jump key to start", 16,
            new Vector2(0f, -330f), font, Color.white);
    }

    public void Show()
    {
        if (_menuPanel != null) _menuPanel.SetActive(true);
        RefreshAudioHint();
    }

    public void Hide()
    {
        if (_menuPanel != null) _menuPanel.SetActive(false);
    }

    public void AdjustBpm(float delta)
    {
        if (_bpmSlider == null) return;

        float min = _bpmSlider.minValue;
        float max = _bpmSlider.maxValue;
        _bpmSlider.value = Mathf.Clamp(_bpmSlider.value + delta, min, max);
    }

    public void TryStartGame()
    {
        if (GameState.Instance == null) return;
        if (GameState.Instance.gameState != GameState.GameStateEnum.MainMenu) return;

        GameState.Instance.BeginReadyPhase();
    }

    private void OnBpmSliderChanged(float value)
    {
        int rounded = Mathf.RoundToInt(value);
        if (_bpmLabel != null)
        {
            _bpmLabel.text = "Rhythm Speed: " + rounded + " BPM";
        }

        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetBpm(rounded);
        }
    }

    private void RefreshAudioHint()
    {
        if (_audioHintText == null) return;
        _audioHintText.text = GetAudioHintText();
    }

    private static string GetAudioHintText()
    {
        if (GameSettings.Instance == null)
        {
            return "Custom audio: add GameSettings on GameController, then assign clips in Inspector.";
        }

        bool hasMusic = GameSettings.Instance.backgroundMusic != null;
        bool hasTick = GameSettings.Instance.metronomeTick != null;

        if (hasMusic && hasTick)
        {
            return "Custom audio loaded: \"" + GameSettings.Instance.backgroundMusic.name +
                   "\" + metronome tick \"" + GameSettings.Instance.metronomeTick.name + "\"";
        }

        if (hasMusic)
        {
            return "Custom music loaded: \"" + GameSettings.Instance.backgroundMusic.name +
                   "\". Drop a tick clip on Game Settings for beat clicks.";
        }

        if (hasTick)
        {
            return "Metronome tick loaded. Drop a music clip on Game Settings for a custom song.";
        }

        return "Custom audio: drop .wav/.mp3/.ogg into Assets/Audio, then assign on GameController > Game Settings.";
    }

    private void CreateLogoImage(Sprite logo, Vector2 position)
    {
        GameObject logoObject = new GameObject("Game Logo");
        logoObject.transform.SetParent(_menuPanel.transform, false);

        RectTransform rect = logoObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(420f, 120f);
        rect.anchoredPosition = position;

        Image image = logoObject.AddComponent<Image>();
        image.sprite = logo;
        image.preserveAspect = true;
    }

    private TextMeshProUGUI CreateLabel(string objectName, string text, float fontSize, Vector2 position,
        TMP_FontAsset font, Color color, float width = 640f, float height = 80f)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(_menuPanel.transform, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = position;

        TextMeshProUGUI label = textObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.color = color;
        label.richText = true;
        if (font != null) label.font = font;

        return label;
    }

    private Slider CreateBpmSlider(Vector2 position, float min, float max, float start, TMP_FontAsset font)
    {
        GameObject sliderObject = new GameObject("BPM Slider");
        sliderObject.transform.SetParent(_menuPanel.transform, false);

        RectTransform rect = sliderObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(360f, 30f);
        rect.anchoredPosition = position;

        Slider slider = sliderObject.AddComponent<Slider>();
        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = true;
        slider.value = start;

        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderObject.transform, false);
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.25f);

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObject.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
        fillAreaRect.offsetMin = new Vector2(8f, 0f);
        fillAreaRect.offsetMax = new Vector2(-8f, 0f);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.7f, 1f);

        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObject.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(8f, 0f);
        handleAreaRect.offsetMax = new Vector2(-8f, 0f);

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(24f, 24f);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;

        return slider;
    }

    private void CreateStartButton(Vector2 position, TMP_FontAsset font)
    {
        GameObject buttonObject = new GameObject("Start Button");
        buttonObject.transform.SetParent(_menuPanel.transform, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(220f, 50f);
        rect.anchoredPosition = position;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.15f, 0.55f, 0.25f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(TryStartGame);

        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(buttonObject.transform, false);
        RectTransform labelRect = labelObject.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = "START GAME";
        label.fontSize = 24;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        if (font != null) label.font = font;
    }
}

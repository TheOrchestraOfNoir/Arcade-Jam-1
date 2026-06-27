using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime-built title screen with logo, controls, and start button.
/// </summary>
public class MainMenuView : MonoBehaviour
{
    private GameObject _menuPanel;

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

        Sprite logo = GameSettings.Instance != null ? GameSettings.Instance.menuLogo : null;
        if (logo != null)
        {
            CreateLogoImage(logo, new Vector2(0f, 250f));
            CreateLabel("Subtitle", "Zeus vs Poseidon — Rhythm Platformer", 20, new Vector2(0f, 190f), font, new Color(0.75f, 0.85f, 1f));
        }
        else
        {
            CreateLabel("Title", "ZEUS vs POSEIDON", 48, new Vector2(0f, 240f), font, Color.white);
            CreateLabel("Subtitle", "Arcade Jam — Rhythm Platformer", 22, new Vector2(0f, 190f), font, new Color(0.75f, 0.85f, 1f));
        }

        CreateLabel("Controls Header", "CONTROLS", 28, new Vector2(0f, 130f), font, Color.yellow);

        string controlsText =
            "<b>Player 1 (Zeus)</b>\n" +
            "Move: WASD  |  Jump: Space  |  Shoot: R  |  Start Duel: Y  |  Special: T\n\n" +
            "<b>Player 2 (Poseidon)</b>\n" +
            "Move: Arrows or IJKL  |  Jump: Right Shift  |  Shoot: N\n" +
            "Start Duel: , (comma)  |  Special: M\n\n" +
            "<b>Rhythm duel arrows:</b> WASD / Arrows / IJKL / gamepad D-pad or left stick";

        CreateLabel("Controls Body", controlsText, 16, new Vector2(0f, 20f), font, Color.white, 720f, 220f);

        CreateStartButton(new Vector2(0f, -200f), font);

        CreateLabel("Start Hint", "Press Space / Enter / either Jump key to start", 16,
            new Vector2(0f, -260f), font, Color.white);
    }

    public void Show()
    {
        if (_menuPanel != null) _menuPanel.SetActive(true);
    }

    public void Hide()
    {
        if (_menuPanel != null) _menuPanel.SetActive(false);
    }

    public void TryStartGame()
    {
        if (GameState.Instance == null) return;
        if (GameState.Instance.gameState != GameState.GameStateEnum.MainMenu) return;

        GameState.Instance.BeginReadyPhase();
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

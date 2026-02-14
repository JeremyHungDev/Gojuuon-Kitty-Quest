using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.IO;

/// <summary>
/// Editor script that automatically creates all game scenes, UI elements, and prefabs.
/// Run from menu: Tools > Gojuuon Kitty Quest > Build All Scenes
/// </summary>
public class SceneBuilder : Editor
{
    // Warm theme colors matching reference image
    private static readonly Color WarmCream = new Color(1f, 0.96f, 0.88f, 1f);            // #FFF5E1
    private static readonly Color ButtonCream = new Color(1f, 0.96f, 0.89f, 0.92f);       // Cream button bg
    private static readonly Color LightBlueBackground = new Color(0.714f, 0.843f, 1f, 1f); // #B6D7FF
    private static readonly Color CreamBackground = new Color(1f, 0.973f, 0.863f, 1f);     // #FFF8DC
    private static readonly Color PurpleCard = new Color(0.8f, 0.6f, 0.8f, 1f);            // #CC99CC
    private static readonly Color OverlayDark = new Color(0f, 0f, 0f, 0.6f);
    private static readonly Color DarkBrown = new Color(0.35f, 0.22f, 0.1f, 1f);          // #59381A
    private static readonly Color PanelBrown = new Color(0.45f, 0.3f, 0.15f, 0.9f);

    [MenuItem("Tools/Gojuuon Kitty Quest/Build All Scenes")]
    public static void BuildAllScenes()
    {
        if (!EditorUtility.DisplayDialog(
            "Build All Scenes",
            "This will create:\n- MainMenu scene\n- StageSelect scene\n- CardMatch scene\n- Card prefab\n- LevelButton prefab\n- Build Settings\n\nContinue?",
            "Yes, Build!", "Cancel"))
        {
            return;
        }

        // Ensure directories exist
        EnsureDirectory("Assets/Scenes");
        EnsureDirectory("Assets/Prefabs");

        // Build prefabs first (needed by scenes)
        BuildCardPrefab();
        BuildLevelButtonPrefab();

        // Build scenes
        BuildMainMenuScene();
        BuildStageSelectScene();
        BuildCardMatchScene();

        // Set up build settings
        SetupBuildSettings();

        EditorUtility.DisplayDialog("Complete!",
            "All scenes and prefabs have been created!\n\n" +
            "Next steps:\n" +
            "1. Open MainMenu scene\n" +
            "2. Press Play to test\n\n" +
            "Scenes are in Assets/Scenes/\n" +
            "Prefabs are in Assets/Prefabs/",
            "OK");
    }

    [MenuItem("Tools/Gojuuon Kitty Quest/Build MainMenu Only")]
    public static void BuildMainMenuOnly()
    {
        EnsureDirectory("Assets/Scenes");
        BuildMainMenuScene();
        Debug.Log("MainMenu scene built successfully!");
    }

    [MenuItem("Tools/Gojuuon Kitty Quest/Build StageSelect Only")]
    public static void BuildStageSelectOnly()
    {
        EnsureDirectory("Assets/Scenes");
        EnsureDirectory("Assets/Prefabs");
        BuildLevelButtonPrefab();
        BuildStageSelectScene();
        Debug.Log("StageSelect scene built successfully!");
    }

    [MenuItem("Tools/Gojuuon Kitty Quest/Build CardMatch Only")]
    public static void BuildCardMatchOnly()
    {
        EnsureDirectory("Assets/Scenes");
        EnsureDirectory("Assets/Prefabs");
        BuildCardPrefab();
        BuildCardMatchScene();
        Debug.Log("CardMatch scene built successfully!");
    }

    // ==================== PREFABS ====================

    private static void BuildCardPrefab()
    {
        // Create card button
        GameObject cardObj = new GameObject("Card");

        // Add RectTransform
        var rectTransform = cardObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(150, 200);

        // Add Image (card background)
        var image = cardObj.AddComponent<Image>();
        image.color = PurpleCard;

        // Add Button
        var button = cardObj.AddComponent<Button>();
        var colors = button.colors;
        colors.highlightedColor = new Color(0.9f, 0.7f, 0.9f, 1f);
        button.colors = colors;

        // Create KanaText child
        GameObject textObj = new GameObject("KanaText");
        textObj.transform.SetParent(cardObj.transform, false);

        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var text = textObj.AddComponent<Text>();
        text.text = "?";
        text.fontSize = 48;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Add CardView script
        var cardView = cardObj.AddComponent<CardView>();
        // Use SerializedObject to set private serialized fields
        var so = new SerializedObject(cardView);
        so.FindProperty("kanaText").objectReferenceValue = text;
        so.FindProperty("cardBackground").objectReferenceValue = image;
        so.ApplyModifiedProperties();

        // Save as prefab
        string prefabPath = "Assets/Prefabs/Card.prefab";
        PrefabUtility.SaveAsPrefabAsset(cardObj, prefabPath);
        DestroyImmediate(cardObj);

        Debug.Log("Card prefab created at " + prefabPath);
    }

    private static void BuildLevelButtonPrefab()
    {
        GameObject buttonObj = new GameObject("LevelButton");

        var rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(350, 80);

        var image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.95f, 0.95f, 1f, 1f);

        var button = buttonObj.AddComponent<Button>();

        // Create Text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 5);
        textRect.offsetMax = new Vector2(-10, -5);

        var text = textObj.AddComponent<Text>();
        text.text = "Level";
        text.fontSize = 24;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = DarkBrown;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        string prefabPath = "Assets/Prefabs/LevelButton.prefab";
        PrefabUtility.SaveAsPrefabAsset(buttonObj, prefabPath);
        DestroyImmediate(buttonObj);

        Debug.Log("LevelButton prefab created at " + prefabPath);
    }

    // ==================== MAIN MENU ====================

    private static void BuildMainMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Create managers
        CreateGameManager();
        CreateAudioManager();
        CreateUIManager();

        // Create Canvas
        GameObject canvas = CreateCanvas();

        // === Background (full screen, loads image at runtime) ===
        var bgObj = CreateFullScreenImage(canvas.transform, "Background", WarmCream);
        var bgImage = bgObj.GetComponent<Image>();

        // === Paw-shaped Title Sign ===
        var pawSprite = GeneratePawSignSprite();
        var pawSign = new GameObject("PawSign");
        pawSign.transform.SetParent(canvas.transform, false);
        var pawRect = pawSign.AddComponent<RectTransform>();
        pawRect.anchoredPosition = new Vector2(0, 280);
        pawRect.sizeDelta = new Vector2(360, 280);
        var pawImage = pawSign.AddComponent<Image>();
        pawImage.sprite = pawSprite;
        pawImage.preserveAspect = true;
        pawImage.raycastTarget = false;

        // Title text on paw sign
        CreateText(pawSign.transform, "TitleText", "KITTY\nQUEST", 38, new Color(1, 0.98f, 0.93f, 1),
            new Vector2(0, -20), new Vector2(300, 120));
        CreateText(pawSign.transform, "SubtitleText", "五十音甜點大冒險", 16, new Color(1, 0.97f, 0.9f, 0.9f),
            new Vector2(0, -90), new Vector2(280, 25));

        // === Main Buttons (centered, lower area) ===
        var continueBtn = CreateStyledButton(canvas.transform, "ContinueButton", "CONTINUE",
            new Vector2(0, -80), new Vector2(380, 65), 28);

        var startNewBtn = CreateStyledButton(canvas.transform, "StartNewGameButton", "START NEW GAME",
            new Vector2(0, -170), new Vector2(380, 65), 28);

        // === Bottom-Right Text Buttons (Settings + Exit) ===
        var settingsBtn = CreateTextOnlyButton(canvas.transform, "SettingsButton", "SETTINGS",
            new Vector2(340, -480), new Vector2(120, 35), 16);

        var exitBtn = CreateTextOnlyButton(canvas.transform, "ExitButton", "EXIT GAME",
            new Vector2(480, -480), new Vector2(130, 35), 16);

        // === New Game Panel (overlay, hidden by default) ===
        var newGameOverlay = CreateFullScreenImage(canvas.transform, "NewGamePanel", OverlayDark);
        var newGamePanel = CreatePanel(newGameOverlay.transform, "NewGameContent", new Color(1, 0.97f, 0.92f, 0.95f),
            new Vector2(0, 0), new Vector2(500, 420));

        CreateText(newGamePanel.transform, "NewGameTitle", "NEW GAME", 32, DarkBrown,
            new Vector2(0, 160), new Vector2(400, 45));

        // Mode selection buttons
        var singleBtn = CreateStyledButton(newGamePanel.transform, "SinglePlayerButton", "單人模式",
            new Vector2(-120, 90), new Vector2(200, 50), 22);
        var twoBtn = CreateStyledButton(newGamePanel.transform, "TwoPlayerButton", "雙人模式",
            new Vector2(120, 90), new Vector2(200, 50), 22);

        // Player 1 Input
        CreateText(newGamePanel.transform, "P1Label", "玩家 1 名稱", 18, DarkBrown,
            new Vector2(0, 40), new Vector2(350, 25));
        var p1Input = CreateInputField(newGamePanel.transform, "Player1NameInput", "輸入名稱...",
            new Vector2(0, 10), new Vector2(350, 40));

        // Player 2 Name Group
        var p2Group = new GameObject("Player2NameGroup");
        p2Group.transform.SetParent(newGamePanel.transform, false);
        var p2GroupRect = p2Group.AddComponent<RectTransform>();
        p2GroupRect.sizeDelta = new Vector2(350, 70);
        p2GroupRect.anchoredPosition = new Vector2(0, -50);

        CreateText(p2Group.transform, "P2Label", "玩家 2 名稱", 18, DarkBrown,
            new Vector2(0, 20), new Vector2(350, 25));
        var p2Input = CreateInputField(p2Group.transform, "Player2NameInput", "輸入名稱...",
            new Vector2(0, -10), new Vector2(350, 40));

        // Confirm & Cancel buttons
        var confirmBtn = CreateStyledButton(newGamePanel.transform, "ConfirmStartButton", "開始冒險！",
            new Vector2(0, -130), new Vector2(250, 55), 24);
        var cancelBtn = CreateStyledButton(newGamePanel.transform, "CancelNewGameButton", "取消",
            new Vector2(0, -180), new Vector2(150, 40), 18);

        newGameOverlay.SetActive(false);

        // === Settings Panel (overlay, hidden by default) ===
        var settingsOverlay = CreateFullScreenImage(canvas.transform, "SettingsPanel", OverlayDark);
        var settingsContent = CreatePanel(settingsOverlay.transform, "SettingsContent",
            new Color(1, 0.97f, 0.92f, 0.95f), new Vector2(0, 0), new Vector2(450, 300));

        CreateText(settingsContent.transform, "SettingsTitle", "SETTINGS", 32, DarkBrown,
            new Vector2(0, 100), new Vector2(400, 45));
        CreateText(settingsContent.transform, "SettingsPlaceholder", "設定功能開發中...", 20, DarkBrown,
            new Vector2(0, 20), new Vector2(400, 30));
        var closeSettingsBtn = CreateStyledButton(settingsContent.transform, "CloseSettingsButton", "關閉",
            new Vector2(0, -80), new Vector2(180, 50), 22);

        settingsOverlay.SetActive(false);

        // === Wire up MainMenuController ===
        var controller = canvas.AddComponent<MainMenuController>();
        var so = new SerializedObject(controller);

        // Main buttons
        so.FindProperty("continueButton").objectReferenceValue = continueBtn.GetComponent<Button>();
        so.FindProperty("startNewGameButton").objectReferenceValue = startNewBtn.GetComponent<Button>();
        so.FindProperty("settingsButton").objectReferenceValue = settingsBtn.GetComponent<Button>();
        so.FindProperty("exitButton").objectReferenceValue = exitBtn.GetComponent<Button>();

        // New Game panel
        so.FindProperty("newGamePanel").objectReferenceValue = newGameOverlay;
        so.FindProperty("singlePlayerButton").objectReferenceValue = singleBtn.GetComponent<Button>();
        so.FindProperty("twoPlayerButton").objectReferenceValue = twoBtn.GetComponent<Button>();
        so.FindProperty("player1NameInput").objectReferenceValue = p1Input.GetComponent<InputField>();
        so.FindProperty("player2NameInput").objectReferenceValue = p2Input.GetComponent<InputField>();
        so.FindProperty("player2NameGroup").objectReferenceValue = p2Group;
        so.FindProperty("confirmStartButton").objectReferenceValue = confirmBtn.GetComponent<Button>();
        so.FindProperty("cancelNewGameButton").objectReferenceValue = cancelBtn.GetComponent<Button>();

        // Settings panel
        so.FindProperty("settingsPanel").objectReferenceValue = settingsOverlay;
        so.FindProperty("closeSettingsButton").objectReferenceValue = closeSettingsBtn.GetComponent<Button>();

        // Background
        so.FindProperty("backgroundImage").objectReferenceValue = bgImage;

        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
        Debug.Log("MainMenu scene built successfully!");
    }

    // ==================== STAGE SELECT ====================

    private static void BuildStageSelectScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        GameObject canvas = CreateCanvas();

        // Background
        CreateFullScreenImage(canvas.transform, "Background", LightBlueBackground);

        // Title
        CreateText(canvas.transform, "Title", "選擇關卡", 36, DarkBrown,
            new Vector2(0, 350), new Vector2(400, 60));

        // Level Button Container
        var container = new GameObject("LevelButtonContainer");
        container.transform.SetParent(canvas.transform, false);
        var containerRect = container.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(400, 500);
        containerRect.anchoredPosition = new Vector2(0, 0);

        var layoutGroup = container.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 20;
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        // Back Button
        var backBtn = CreateButton(canvas.transform, "BackButton", "返回",
            new Vector2(0, -400), new Vector2(200, 50));

        // Load LevelButton prefab
        var levelButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/LevelButton.prefab");

        // Add StageSelectController
        var controller = canvas.AddComponent<StageSelectController>();
        var so = new SerializedObject(controller);
        so.FindProperty("levelButtonContainer").objectReferenceValue = container.transform;
        so.FindProperty("levelButtonPrefab").objectReferenceValue = levelButtonPrefab;
        so.FindProperty("backButton").objectReferenceValue = backBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/StageSelect.unity");
        Debug.Log("StageSelect scene built successfully!");
    }

    // ==================== CARD MATCH ====================

    private static void BuildCardMatchScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        GameObject canvas = CreateCanvas();

        // Background
        CreateFullScreenImage(canvas.transform, "Background", CreamBackground);

        // Player Name Text
        CreateText(canvas.transform, "PlayerNameText", "玩家名稱", 24, DarkBrown,
            new Vector2(-300, 450), new Vector2(300, 40));

        // Move Count Text
        var moveText = CreateText(canvas.transform, "MoveCountText", "步數：0", 24, DarkBrown,
            new Vector2(300, 450), new Vector2(300, 40));

        // Card Grid
        var gridObj = new GameObject("CardGrid");
        gridObj.transform.SetParent(canvas.transform, false);
        var gridRect = gridObj.AddComponent<RectTransform>();
        gridRect.sizeDelta = new Vector2(520, 460);
        gridRect.anchoredPosition = new Vector2(0, 0);

        var gridLayout = gridObj.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(150, 200);
        gridLayout.spacing = new Vector2(20, 20);
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 3;

        // Result Panel
        var resultPanel = CreatePanel(canvas.transform, "ResultPanel", OverlayDark,
            Vector2.zero, Vector2.zero);
        // Stretch result panel
        var rpRect = resultPanel.GetComponent<RectTransform>();
        rpRect.anchorMin = Vector2.zero;
        rpRect.anchorMax = Vector2.one;
        rpRect.offsetMin = Vector2.zero;
        rpRect.offsetMax = Vector2.zero;

        var resultText = CreateText(resultPanel.transform, "ResultText", "結果", 32, Color.white,
            new Vector2(0, 50), new Vector2(500, 200));

        var retryBtn = CreateButton(resultPanel.transform, "RetryButton", "再玩一次",
            new Vector2(0, -100), new Vector2(250, 50));

        resultPanel.SetActive(false);

        // Back Button
        var backBtn = CreateButton(canvas.transform, "BackButton", "返回",
            new Vector2(0, -450), new Vector2(200, 50));

        // Load Card prefab
        var cardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Card.prefab");

        // Add CardMatchController
        var controller = canvas.AddComponent<CardMatchController>();
        var so = new SerializedObject(controller);
        so.FindProperty("cardGrid").objectReferenceValue = gridLayout;
        so.FindProperty("cardPrefab").objectReferenceValue = cardPrefab;
        so.FindProperty("moveCountText").objectReferenceValue = moveText.GetComponent<Text>();
        so.FindProperty("playerNameText").objectReferenceValue =
            canvas.transform.Find("PlayerNameText").GetComponent<Text>();
        so.FindProperty("resultPanel").objectReferenceValue = resultPanel;
        so.FindProperty("resultText").objectReferenceValue = resultText.GetComponent<Text>();
        so.FindProperty("backButton").objectReferenceValue = backBtn.GetComponent<Button>();
        so.FindProperty("retryButton").objectReferenceValue = retryBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/CardMatch.unity");
        Debug.Log("CardMatch scene built successfully!");
    }

    // ==================== BUILD SETTINGS ====================

    private static void SetupBuildSettings()
    {
        var scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/StageSelect.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/CardMatch.unity", true)
        };
        EditorBuildSettings.scenes = scenes;
        Debug.Log("Build Settings updated with 3 scenes.");
    }

    // ==================== HELPERS ====================

    private static void EnsureDirectory(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = Path.GetDirectoryName(path).Replace("\\", "/");
            string folder = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, folder);
        }
    }

    private static void CreateGameManager()
    {
        var go = new GameObject("GameManager");
        go.AddComponent<GameManager>();
    }

    private static void CreateAudioManager()
    {
        var go = new GameObject("AudioManager");

        var bgm = go.AddComponent<AudioSource>();
        bgm.playOnAwake = false;

        var sfx = go.AddComponent<AudioSource>();
        sfx.playOnAwake = false;

        var voice = go.AddComponent<AudioSource>();
        voice.playOnAwake = false;

        var manager = go.AddComponent<AudioManager>();

        var so = new SerializedObject(manager);
        // AudioSource components are indexed 0, 1, 2
        var sources = go.GetComponents<AudioSource>();
        so.FindProperty("bgmSource").objectReferenceValue = sources[0];
        so.FindProperty("sfxSource").objectReferenceValue = sources[1];
        so.FindProperty("voiceSource").objectReferenceValue = sources[2];
        so.ApplyModifiedProperties();
    }

    private static void CreateUIManager()
    {
        var go = new GameObject("UIManager");
        go.AddComponent<UIManager>();
    }

    private static GameObject CreateCanvas()
    {
        var canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // EventSystem
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        return canvasObj;
    }

    private static GameObject CreateFullScreenImage(Transform parent, string name, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var image = go.AddComponent<Image>();
        image.color = color;

        return go;
    }

    private static GameObject CreateText(Transform parent, string name, string content,
        int fontSize, Color color, Vector2 position, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var text = go.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        return go;
    }

    private static GameObject CreateButton(Transform parent, string name, string label,
        Vector2 position, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var image = go.AddComponent<Image>();
        image.color = Color.white;

        var button = go.AddComponent<Button>();

        // Button text child
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(go.transform, false);

        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var text = textObj.AddComponent<Text>();
        text.text = label;
        text.fontSize = 24;
        text.color = DarkBrown;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        return go;
    }

    private static GameObject CreateStyledButton(Transform parent, string name, string label,
        Vector2 position, Vector2 size, int fontSize)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var image = go.AddComponent<Image>();
        image.color = ButtonCream;

        var button = go.AddComponent<Button>();
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.93f, 0.82f, 1f);
        colors.pressedColor = new Color(0.95f, 0.88f, 0.75f, 1f);
        colors.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        button.colors = colors;

        // Outline for border effect
        var outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(0.55f, 0.38f, 0.2f, 0.6f);
        outline.effectDistance = new Vector2(2, -2);

        // Button text
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(go.transform, false);

        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var text = textObj.AddComponent<Text>();
        text.text = label;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.color = DarkBrown;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        return go;
    }

    private static GameObject CreateTextOnlyButton(Transform parent, string name, string label,
        Vector2 position, Vector2 size, int fontSize)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        // Transparent background (needed for Button click area)
        var image = go.AddComponent<Image>();
        image.color = Color.clear;

        var button = go.AddComponent<Button>();

        var textObj = new GameObject("Text");
        textObj.transform.SetParent(go.transform, false);

        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var text = textObj.AddComponent<Text>();
        text.text = label;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.color = new Color(0.45f, 0.3f, 0.18f, 1f);
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Shadow effect like reference image
        var shadow = textObj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.3f);
        shadow.effectDistance = new Vector2(1, -1);

        // Use text as target graphic so it highlights on hover
        button.targetGraphic = text;
        var colors = button.colors;
        colors.normalColor = new Color(0.45f, 0.3f, 0.18f, 1f);
        colors.highlightedColor = new Color(0.6f, 0.4f, 0.2f, 1f);
        colors.pressedColor = new Color(0.35f, 0.2f, 0.1f, 1f);
        button.colors = colors;

        return go;
    }

    private static Sprite GeneratePawSignSprite()
    {
        int w = 512, h = 400;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var pixels = new Color[w * h];

        Color clear = Color.clear;
        Color wood = new Color(0.72f, 0.52f, 0.32f, 1f);       // Warm wood brown
        Color woodLight = new Color(0.82f, 0.65f, 0.45f, 1f);  // Lighter wood center
        Color border = new Color(0.45f, 0.3f, 0.15f, 1f);      // Dark border

        // Clear all
        for (int i = 0; i < pixels.Length; i++) pixels[i] = clear;

        // Main body - large ellipse (the palm of the paw)
        FillEllipse(pixels, w, h, w / 2, 110, 200, 105, woodLight, wood, border, 5);

        // Three toe pads on top (cat ear bumps)
        FillCircle(pixels, w, h, w / 2 - 140, 260, 60, woodLight, wood, border, 5);
        FillCircle(pixels, w, h, w / 2, 295, 60, woodLight, wood, border, 5);
        FillCircle(pixels, w, h, w / 2 + 140, 260, 60, woodLight, wood, border, 5);

        tex.SetPixels(pixels);
        tex.Apply();

        // Save as asset with synchronous import
        EnsureDirectory("Assets/Sprites");
        byte[] png = tex.EncodeToPNG();
        string path = "Assets/Sprites/paw_sign.png";
        File.WriteAllBytes(path, png);

        // Force synchronous import so asset is available immediately
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);

        // Set texture type to Sprite
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Bilinear;
            importer.SaveAndReimport();
        }

        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogWarning("Paw sprite not loaded from asset, creating in-memory fallback");
            sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Object.DestroyImmediate(tex);
        }

        return sprite;
    }

    private static void FillEllipse(Color[] pixels, int texW, int texH,
        int cx, int cy, int rx, int ry, Color inner, Color outer, Color border, int borderW)
    {
        for (int x = cx - rx - borderW; x <= cx + rx + borderW; x++)
        {
            for (int y = cy - ry - borderW; y <= cy + ry + borderW; y++)
            {
                if (x < 0 || x >= texW || y < 0 || y >= texH) continue;

                float dx = (float)(x - cx) / rx;
                float dy = (float)(y - cy) / ry;
                float dist = dx * dx + dy * dy;

                float dxB = (float)(x - cx) / (rx + borderW);
                float dyB = (float)(y - cy) / (ry + borderW);
                float distB = dxB * dxB + dyB * dyB;

                if (dist <= 1f)
                {
                    // Gradient from inner (center) to outer (edge)
                    float t = Mathf.Sqrt(dist);
                    pixels[y * texW + x] = Color.Lerp(inner, outer, t * 0.6f);
                }
                else if (distB <= 1f)
                {
                    pixels[y * texW + x] = border;
                }
            }
        }
    }

    private static void FillCircle(Color[] pixels, int texW, int texH,
        int cx, int cy, int r, Color inner, Color outer, Color border, int borderW)
    {
        FillEllipse(pixels, texW, texH, cx, cy, r, r, inner, outer, border, borderW);
    }

    private static GameObject CreatePanel(Transform parent, string name, Color color,
        Vector2 position, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var image = go.AddComponent<Image>();
        image.color = color;

        return go;
    }

    private static GameObject CreateInputField(Transform parent, string name, string placeholder,
        Vector2 position, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var image = go.AddComponent<Image>();
        image.color = new Color(1f, 0.98f, 0.94f, 1f);

        // Text area
        var textArea = new GameObject("Text");
        textArea.transform.SetParent(go.transform, false);
        var textRect = textArea.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = new Vector2(10, 2);
        textRect.offsetMax = new Vector2(-10, -2);
        var textComp = textArea.AddComponent<Text>();
        textComp.fontSize = 20;
        textComp.color = DarkBrown;
        textComp.alignment = TextAnchor.MiddleLeft;
        textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComp.supportRichText = false;

        // Placeholder
        var placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(go.transform, false);
        var phRect = placeholderObj.AddComponent<RectTransform>();
        phRect.anchorMin = new Vector2(0, 0);
        phRect.anchorMax = new Vector2(1, 1);
        phRect.offsetMin = new Vector2(10, 2);
        phRect.offsetMax = new Vector2(-10, -2);
        var phText = placeholderObj.AddComponent<Text>();
        phText.text = placeholder;
        phText.fontSize = 20;
        phText.fontStyle = FontStyle.Italic;
        phText.color = new Color(0.6f, 0.6f, 0.6f, 0.75f);
        phText.alignment = TextAnchor.MiddleLeft;
        phText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var inputField = go.AddComponent<InputField>();
        inputField.textComponent = textComp;
        inputField.placeholder = phText;

        return go;
    }
}

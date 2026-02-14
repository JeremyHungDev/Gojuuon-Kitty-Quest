using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Main Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button startNewGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("New Game Panel")]
    [SerializeField] private GameObject newGamePanel;
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button twoPlayerButton;
    [SerializeField] private InputField player1NameInput;
    [SerializeField] private InputField player2NameInput;
    [SerializeField] private GameObject player2NameGroup;
    [SerializeField] private Button confirmStartButton;
    [SerializeField] private Button cancelNewGameButton;

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button closeSettingsButton;

    [Header("Background")]
    [SerializeField] private Image backgroundImage;

    private bool isTwoPlayer = false;

    private void Start()
    {
        newGamePanel.SetActive(false);
        settingsPanel.SetActive(false);

        // Check if save exists to enable Continue
        var gm = GameManager.Instance;
        bool hasSave = gm != null && gm.SaveSystem.HasSaveFile();
        continueButton.interactable = hasSave;

        // Main buttons
        continueButton.onClick.AddListener(OnContinueClicked);
        startNewGameButton.onClick.AddListener(OnStartNewGameClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        exitButton.onClick.AddListener(OnExitClicked);

        // New Game panel buttons
        singlePlayerButton.onClick.AddListener(OnSinglePlayerClicked);
        twoPlayerButton.onClick.AddListener(OnTwoPlayerClicked);
        confirmStartButton.onClick.AddListener(OnConfirmStartClicked);
        cancelNewGameButton.onClick.AddListener(OnCancelNewGameClicked);

        // Settings panel
        closeSettingsButton.onClick.AddListener(OnCloseSettingsClicked);

        LoadBackground();
    }

    private void LoadBackground()
    {
        if (backgroundImage == null) return;

        var sprite = Resources.Load<Sprite>("Backgrounds/MainMenu");
        if (sprite != null)
        {
            backgroundImage.sprite = sprite;
            backgroundImage.color = Color.white;
            return;
        }

        var tex = Resources.Load<Texture2D>("Backgrounds/MainMenu");
        if (tex != null)
        {
            sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f));
            backgroundImage.sprite = sprite;
            backgroundImage.color = Color.white;
        }
    }

    // === Main Menu ===

    private void OnContinueClicked()
    {
        PlayButtonSFX();
        var gm = GameManager.Instance;
        gm.Load();
        gm.StateManager.ChangeState(GameState.StageSelect);

        if (UIManager.Instance != null)
            UIManager.Instance.LoadSceneWithFade("StageSelect");
        else
            gm.LoadScene("StageSelect");
    }

    private void OnStartNewGameClicked()
    {
        PlayButtonSFX();
        isTwoPlayer = false;
        player2NameGroup.SetActive(false);
        player1NameInput.text = "";
        player2NameInput.text = "";
        newGamePanel.SetActive(true);
    }

    private void OnSettingsClicked()
    {
        PlayButtonSFX();
        settingsPanel.SetActive(true);
    }

    private void OnExitClicked()
    {
        PlayButtonSFX();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // === New Game Panel ===

    private void OnSinglePlayerClicked()
    {
        PlayButtonSFX();
        isTwoPlayer = false;
        player2NameGroup.SetActive(false);
        singlePlayerButton.interactable = false;
        twoPlayerButton.interactable = true;
    }

    private void OnTwoPlayerClicked()
    {
        PlayButtonSFX();
        isTwoPlayer = true;
        player2NameGroup.SetActive(true);
        singlePlayerButton.interactable = true;
        twoPlayerButton.interactable = false;
    }

    private void OnConfirmStartClicked()
    {
        PlayButtonSFX();
        string player1Name = string.IsNullOrEmpty(player1NameInput.text)
            ? "Player 1" : player1NameInput.text;

        var gm = GameManager.Instance;
        gm.PlayerManager.SetPlayerCount(isTwoPlayer ? 2 : 1);

        gm.SaveData.players.Clear();
        gm.SaveData.players.Add(new PlayerData { playerName = player1Name, currentLevel = 1 });

        if (isTwoPlayer)
        {
            string player2Name = string.IsNullOrEmpty(player2NameInput.text)
                ? "Player 2" : player2NameInput.text;
            gm.SaveData.players.Add(new PlayerData { playerName = player2Name, currentLevel = 1 });
        }

        gm.StateManager.ChangeState(GameState.StageSelect);
        gm.Save();

        if (UIManager.Instance != null)
            UIManager.Instance.LoadSceneWithFade("StageSelect");
        else
            gm.LoadScene("StageSelect");
    }

    private void OnCancelNewGameClicked()
    {
        PlayButtonSFX();
        newGamePanel.SetActive(false);
    }

    // === Settings Panel ===

    private void OnCloseSettingsClicked()
    {
        PlayButtonSFX();
        settingsPanel.SetActive(false);
    }

    private void PlayButtonSFX()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("button");
    }
}

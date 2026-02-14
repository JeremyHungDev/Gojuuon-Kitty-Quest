using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button twoPlayerButton;

    [Header("Player Name Input")]
    [SerializeField] private GameObject nameInputPanel;
    [SerializeField] private InputField player1NameInput;
    [SerializeField] private InputField player2NameInput;
    [SerializeField] private GameObject player2NameGroup;
    [SerializeField] private Button startGameButton;

    private bool isTwoPlayer = false;

    private void Start()
    {
        nameInputPanel.SetActive(false);
        singlePlayerButton.onClick.AddListener(OnSinglePlayerClicked);
        twoPlayerButton.onClick.AddListener(OnTwoPlayerClicked);
        startGameButton.onClick.AddListener(OnStartGameClicked);
    }

    private void OnSinglePlayerClicked()
    {
        isTwoPlayer = false;
        player2NameGroup.SetActive(false);
        nameInputPanel.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("button");
    }

    private void OnTwoPlayerClicked()
    {
        isTwoPlayer = true;
        player2NameGroup.SetActive(true);
        nameInputPanel.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("button");
    }

    private void OnStartGameClicked()
    {
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
}

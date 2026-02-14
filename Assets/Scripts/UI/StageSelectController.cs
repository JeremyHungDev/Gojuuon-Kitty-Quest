using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectController : MonoBehaviour
{
    [SerializeField] private Transform levelButtonContainer;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Button backButton;

    private List<LevelConfig> levels;

    private void Start()
    {
        LoadLevels();
        CreateLevelButtons();
        backButton.onClick.AddListener(OnBackClicked);
    }

    private void LoadLevels()
    {
        var jsonFile = Resources.Load<TextAsset>("LevelConfigs/Levels");
        var wrapper = JsonUtility.FromJson<LevelConfigWrapper>(jsonFile.text);
        levels = wrapper.levels;
    }

    private void CreateLevelButtons()
    {
        var playerData = GameManager.Instance.GetActivePlayerData();

        foreach (var level in levels)
        {
            var buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            var buttonText = buttonObj.GetComponentInChildren<Text>();
            var button = buttonObj.GetComponent<Button>();

            bool isUnlocked = IsLevelUnlocked(level, playerData);

            buttonText.text = isUnlocked
                ? level.name + "\n" + level.description
                : level.name + "\n(locked)";

            button.interactable = isUnlocked;

            if (isUnlocked)
            {
                var capturedLevel = level;
                button.onClick.AddListener(() => OnLevelSelected(capturedLevel));
            }
        }
    }

    private bool IsLevelUnlocked(LevelConfig level, PlayerData playerData)
    {
        if (level.unlockCondition == "none") return true;
        int requiredLevel = level.levelId - 1;
        return playerData.currentLevel > requiredLevel;
    }

    private void OnLevelSelected(LevelConfig level)
    {
        GameManager.Instance.StateManager.ChangeState(GameState.Playing);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("button");

        if (UIManager.Instance != null)
            UIManager.Instance.LoadSceneWithFade("CardMatch");
        else
            GameManager.Instance.LoadScene("CardMatch");
    }

    private void OnBackClicked()
    {
        GameManager.Instance.StateManager.ChangeState(GameState.MainMenu);

        if (UIManager.Instance != null)
            UIManager.Instance.LoadSceneWithFade("MainMenu");
        else
            GameManager.Instance.LoadScene("MainMenu");
    }
}

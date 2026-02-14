using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum GameState
{
    MainMenu,
    StageSelect,
    Playing,
    Paused,
    Result
}

public class GameStateManager
{
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
    }
}

public class PlayerManager
{
    public int ActivePlayerIndex { get; private set; } = 0;
    public int PlayerCount { get; private set; } = 1;
    public bool IsTwoPlayer => PlayerCount == 2;

    public void SetPlayerCount(int count)
    {
        PlayerCount = count;
        ActivePlayerIndex = 0;
    }

    public void SwitchPlayer()
    {
        if (!IsTwoPlayer) return;
        ActivePlayerIndex = (ActivePlayerIndex + 1) % PlayerCount;
    }

    public void ResetToPlayer1()
    {
        ActivePlayerIndex = 0;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameStateManager StateManager { get; private set; }
    public PlayerManager PlayerManager { get; private set; }

    private ISaveSystem saveSystem;
    private KanaDatabaseService kanaDatabase;

    public GameSaveData SaveData { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        StateManager = new GameStateManager();
        PlayerManager = new PlayerManager();
        saveSystem = new JsonSaveSystem();

        LoadKanaDatabase();
        LoadSaveData();
    }

    private void LoadKanaDatabase()
    {
        kanaDatabase = new KanaDatabaseService();
        var jsonFile = Resources.Load<TextAsset>("KanaDatabase");
        if (jsonFile != null)
        {
            kanaDatabase.LoadFromJson(jsonFile.text);
        }
    }

    private void LoadSaveData()
    {
        SaveData = saveSystem.LoadGame();
        if (SaveData == null)
        {
            SaveData = new GameSaveData
            {
                players = new List<PlayerData>
                {
                    new PlayerData { playerName = "Player 1", currentLevel = 1 }
                },
                lastPlayDate = System.DateTime.Now.ToString("yyyy-MM-dd")
            };
        }
    }

    public KanaDatabaseService GetKanaDatabase() => kanaDatabase;

    public void Save()
    {
        SaveData.lastPlayDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        saveSystem.SaveGame(SaveData);
    }

    public PlayerData GetActivePlayerData()
    {
        return SaveData.players[PlayerManager.ActivePlayerIndex];
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

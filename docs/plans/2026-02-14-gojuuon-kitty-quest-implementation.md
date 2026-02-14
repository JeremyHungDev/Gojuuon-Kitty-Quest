# Gojuuon Kitty Quest 實作計畫

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** 建立一個 Unity 2D 五十音學習遊戲的 MVP，包含核心系統、主選單、卡牌翻翻看小遊戲和あ行假名資料。

**Architecture:** 模組化架構搭配 Singleton、MVC、Factory、Interface 設計模式。資料驅動設計（JSON 配置假名和關卡）。核心系統（GameManager、SaveSystem、AudioManager、KanaDatabase）透過單例模式管理全域狀態。

**Tech Stack:** Unity 2022 LTS+, C#, Unity UI (uGUI), Unity Test Framework (NUnit), JsonUtility, Newtonsoft.Json

**Design Doc:** `docs/plans/2026-02-14-gojuuon-kitty-quest-design.md`

---

## Phase 1: MVP（最小可玩版本）

---

### Task 1: Unity 專案初始化

**說明：** 用 Unity Hub 建立 2D 專案，設定資料夾結構和 Git 忽略規則。

**Step 1: 建立 Unity 專案**

用 Unity Hub 建立新的 2D (URP) 專案：
- 專案名稱：`GojuuonKittyQuest`
- 位置：`c:\Users\KHUser\source\repos\KittyLanguage\`
- Template: `2D (URP)`
- Unity 版本：2022 LTS 或更新

**Step 2: 新增 .gitignore**

建立 `GojuuonKittyQuest/.gitignore`（Unity 標準 gitignore）：

```gitignore
# Unity generated
/[Ll]ibrary/
/[Tt]emp/
/[Oo]bj/
/[Bb]uild/
/[Bb]uilds/
/[Ll]ogs/
/[Uu]ser[Ss]ettings/

# Visual Studio
.vs/
*.csproj
*.sln
*.suo
*.tmp
*.user
*.userprefs
*.pidb
*.booproj
*.svd
*.pdb
*.mdb
*.opendb
*.VC.db

# Unity3D generated meta files
*.pidb.meta
*.pdb.meta
*.mdb.meta

# OS generated
.DS_Store
.DS_Store?
Thumbs.db

# Builds
*.apk
*.aab
*.unitypackage
*.app
```

**Step 3: 建立專案資料夾結構**

在 Unity 專案的 `Assets/` 下建立以下資料夾：

```
Assets/
├── Scripts/
│   ├── Core/
│   │   └── SaveSystem/
│   ├── Data/
│   ├── Gameplay/
│   │   ├── Stage1_MemoryLearning/
│   │   ├── Stage2_ListeningChallenge/
│   │   └── Stage3_WordQuiz/
│   └── UI/
├── Resources/
│   └── LevelConfigs/
├── Scenes/
├── Sprites/
├── Audio/
├── Prefabs/
└── Tests/
    ├── EditMode/
    └── PlayMode/
```

每個空資料夾放一個 `.gitkeep` 檔案以確保 Git 追蹤。

**Step 4: 設定 Unity Test Framework**

在 Unity Editor 中：
1. Window → Package Manager → 確認 `Test Framework` 已安裝
2. 在 `Assets/Tests/EditMode/` 建立 Assembly Definition：
   - 名稱：`Tests.EditMode`
   - Platforms：勾選 `Editor`
   - References：加入 `UnityEngine.TestRunner`, `UnityEditor.TestRunner`
3. 在 `Assets/Tests/PlayMode/` 建立 Assembly Definition：
   - 名稱：`Tests.PlayMode`
   - 勾選 `Test Assemblies`

**Step 5: 提交**

```bash
git add .
git commit -m "chore: initialize Unity 2D project with folder structure and test framework"
```

---

### Task 2: 資料結構定義（Data Classes）

**Files:**
- Create: `Assets/Scripts/Data/KanaData.cs`
- Create: `Assets/Scripts/Data/GameSaveData.cs`
- Create: `Assets/Scripts/Data/LevelConfig.cs`
- Create: `Assets/Scripts/Data/WordData.cs`
- Test: `Assets/Tests/EditMode/DataTests.cs`

**Step 1: 寫測試 - KanaData 序列化**

```csharp
// Assets/Tests/EditMode/DataTests.cs
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class DataTests
{
    [Test]
    public void KanaData_SerializesToJson()
    {
        var kana = new KanaData
        {
            id = "a",
            hiragana = "あ",
            katakana = "ア",
            romaji = "a",
            origin = "安",
            originMeaning = "安靜",
            audioFile = "audio/kana/a.mp3",
            strokeOrder = new List<int> { 1, 2, 3 },
            group = "a_row",
            level = 1
        };

        string json = JsonUtility.ToJson(kana);
        Assert.IsTrue(json.Contains("\"hiragana\":\"あ\""));
    }

    [Test]
    public void KanaData_DeserializesFromJson()
    {
        string json = "{\"id\":\"a\",\"hiragana\":\"あ\",\"katakana\":\"ア\",\"romaji\":\"a\",\"origin\":\"安\",\"originMeaning\":\"安靜\",\"audioFile\":\"audio/kana/a.mp3\",\"strokeOrder\":[1,2,3],\"group\":\"a_row\",\"level\":1}";

        var kana = JsonUtility.FromJson<KanaData>(json);
        Assert.AreEqual("あ", kana.hiragana);
        Assert.AreEqual("ア", kana.katakana);
        Assert.AreEqual("a_row", kana.group);
    }

    [Test]
    public void GameSaveData_SupportsMultiplePlayers()
    {
        var save = new GameSaveData();
        save.players = new List<PlayerData>
        {
            new PlayerData { playerName = "小明", currentLevel = 1 },
            new PlayerData { playerName = "小華", currentLevel = 2 }
        };

        string json = JsonUtility.ToJson(save);
        var loaded = JsonUtility.FromJson<GameSaveData>(json);

        Assert.AreEqual(2, loaded.players.Count);
        Assert.AreEqual("小明", loaded.players[0].playerName);
        Assert.AreEqual("小華", loaded.players[1].playerName);
    }

    [Test]
    public void KanaDatabaseWrapper_DeserializesKanaList()
    {
        var wrapper = new KanaDatabaseWrapper();
        wrapper.kanaList = new List<KanaData>
        {
            new KanaData { id = "a", hiragana = "あ" },
            new KanaData { id = "i", hiragana = "い" }
        };

        string json = JsonUtility.ToJson(wrapper);
        var loaded = JsonUtility.FromJson<KanaDatabaseWrapper>(json);

        Assert.AreEqual(2, loaded.kanaList.Count);
        Assert.AreEqual("い", loaded.kanaList[1].hiragana);
    }
}
```

**Step 2: 執行測試確認失敗**

在 Unity Editor 中：Window → General → Test Runner → EditMode → Run All
預期結果：FAIL（KanaData 等 class 不存在）

**Step 3: 實作資料結構**

```csharp
// Assets/Scripts/Data/KanaData.cs
using System;
using System.Collections.Generic;

[Serializable]
public class KanaData
{
    public string id;
    public string hiragana;
    public string katakana;
    public string romaji;
    public string origin;
    public string originMeaning;
    public string audioFile;
    public List<int> strokeOrder;
    public string group;
    public int level;
}

[Serializable]
public class KanaDatabaseWrapper
{
    public List<KanaData> kanaList;
}
```

```csharp
// Assets/Scripts/Data/GameSaveData.cs
using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public List<PlayerData> players;
    public string lastPlayDate;
    public int totalPlayTimeSeconds;
}

[Serializable]
public class PlayerData
{
    public string playerName;
    public int currentLevel;
    public List<string> learnedKana;
    public List<string> collectedRewards;
    public List<MinigameScore> scores;

    public PlayerData()
    {
        learnedKana = new List<string>();
        collectedRewards = new List<string>();
        scores = new List<MinigameScore>();
    }
}

[Serializable]
public class MinigameScore
{
    public string minigameId;
    public string kanaId;
    public int bestScore;
    public int attempts;
}
```

```csharp
// Assets/Scripts/Data/LevelConfig.cs
using System;
using System.Collections.Generic;

[Serializable]
public class LevelConfig
{
    public int levelId;
    public string name;
    public string sceneName;
    public string description;
    public List<string> kanaGroups;
    public string reward;
    public string rewardSprite;
    public string unlockCondition;
    public List<string> minigames;
}

[Serializable]
public class LevelConfigWrapper
{
    public List<LevelConfig> levels;
}
```

```csharp
// Assets/Scripts/Data/WordData.cs
using System;
using System.Collections.Generic;

[Serializable]
public class WordData
{
    public string id;
    public string kana;
    public string katakana;
    public string chinese;
    public string category;
    public int difficulty;
    public string sprite;
}

[Serializable]
public class WordDatabaseWrapper
{
    public List<WordData> words;
}
```

**Step 4: 執行測試確認通過**

在 Unity Editor 中：Window → General → Test Runner → EditMode → Run All
預期結果：全部 PASS

**Step 5: 提交**

```bash
git add Assets/Scripts/Data/ Assets/Tests/EditMode/DataTests.cs
git commit -m "feat: add data structures for kana, save, level config, and words"
```

---

### Task 3: 五十音 JSON 資料庫（あ行）

**Files:**
- Create: `Assets/Resources/KanaDatabase.json`
- Test: 在 `DataTests.cs` 新增測試

**Step 1: 寫測試 - JSON 檔案載入**

在 `DataTests.cs` 新增：

```csharp
[Test]
public void KanaDatabase_JsonFile_LoadsCorrectly()
{
    TextAsset jsonFile = Resources.Load<TextAsset>("KanaDatabase");
    Assert.IsNotNull(jsonFile, "KanaDatabase.json not found in Resources");

    var db = JsonUtility.FromJson<KanaDatabaseWrapper>(jsonFile.text);
    Assert.IsNotNull(db.kanaList);
    Assert.GreaterOrEqual(db.kanaList.Count, 5, "Should have at least あ行 (5 kana)");

    // 驗證あ行完整性
    var aRow = db.kanaList.FindAll(k => k.group == "a_row");
    Assert.AreEqual(5, aRow.Count, "あ行 should have exactly 5 kana");
}
```

**Step 2: 執行測試確認失敗**

預期結果：FAIL（JSON 檔案不存在）

**Step 3: 建立 KanaDatabase.json**

```json
// Assets/Resources/KanaDatabase.json
{
  "kanaList": [
    {
      "id": "a",
      "hiragana": "あ",
      "katakana": "ア",
      "romaji": "a",
      "origin": "安",
      "originMeaning": "安靜",
      "audioFile": "audio/kana/a.mp3",
      "strokeOrder": [1, 2, 3],
      "group": "a_row",
      "level": 1
    },
    {
      "id": "i",
      "hiragana": "い",
      "katakana": "イ",
      "romaji": "i",
      "origin": "以",
      "originMeaning": "以前",
      "audioFile": "audio/kana/i.mp3",
      "strokeOrder": [1, 2],
      "group": "a_row",
      "level": 1
    },
    {
      "id": "u",
      "hiragana": "う",
      "katakana": "ウ",
      "romaji": "u",
      "origin": "宇",
      "originMeaning": "宇宙",
      "audioFile": "audio/kana/u.mp3",
      "strokeOrder": [1, 2],
      "group": "a_row",
      "level": 1
    },
    {
      "id": "e",
      "hiragana": "え",
      "katakana": "エ",
      "romaji": "e",
      "origin": "衣",
      "originMeaning": "衣服",
      "audioFile": "audio/kana/e.mp3",
      "strokeOrder": [1, 2],
      "group": "a_row",
      "level": 1
    },
    {
      "id": "o",
      "hiragana": "お",
      "katakana": "オ",
      "romaji": "o",
      "origin": "於",
      "originMeaning": "於是",
      "audioFile": "audio/kana/o.mp3",
      "strokeOrder": [1, 2, 3],
      "group": "a_row",
      "level": 1
    }
  ]
}
```

**Step 4: 執行測試確認通過**

預期結果：PASS

**Step 5: 提交**

```bash
git add Assets/Resources/KanaDatabase.json Assets/Tests/EditMode/DataTests.cs
git commit -m "feat: add kana database JSON with あ行 (a-row) data"
```

---

### Task 4: SaveSystem（存檔系統）

**Files:**
- Create: `Assets/Scripts/Core/SaveSystem/ISaveSystem.cs`
- Create: `Assets/Scripts/Core/SaveSystem/JsonSaveSystem.cs`
- Test: `Assets/Tests/EditMode/SaveSystemTests.cs`

**Step 1: 寫測試**

```csharp
// Assets/Tests/EditMode/SaveSystemTests.cs
using NUnit.Framework;
using System.IO;
using System.Collections.Generic;

public class SaveSystemTests
{
    private JsonSaveSystem saveSystem;
    private string testSavePath;

    [SetUp]
    public void SetUp()
    {
        testSavePath = Path.Combine(Path.GetTempPath(), "test_save.json");
        saveSystem = new JsonSaveSystem(testSavePath);
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(testSavePath))
            File.Delete(testSavePath);
    }

    [Test]
    public void HasSaveFile_ReturnsFalse_WhenNoFile()
    {
        Assert.IsFalse(saveSystem.HasSaveFile());
    }

    [Test]
    public void SaveAndLoad_RoundTrip_PreservesData()
    {
        var data = new GameSaveData
        {
            players = new List<PlayerData>
            {
                new PlayerData
                {
                    playerName = "テスト",
                    currentLevel = 2,
                    learnedKana = new List<string> { "a", "i", "u" }
                }
            },
            lastPlayDate = "2026-02-14",
            totalPlayTimeSeconds = 120
        };

        saveSystem.SaveGame(data);
        Assert.IsTrue(saveSystem.HasSaveFile());

        var loaded = saveSystem.LoadGame();
        Assert.AreEqual("テスト", loaded.players[0].playerName);
        Assert.AreEqual(2, loaded.players[0].currentLevel);
        Assert.AreEqual(3, loaded.players[0].learnedKana.Count);
        Assert.AreEqual(120, loaded.totalPlayTimeSeconds);
    }

    [Test]
    public void DeleteSave_RemovesFile()
    {
        var data = new GameSaveData
        {
            players = new List<PlayerData>(),
            lastPlayDate = "2026-02-14"
        };

        saveSystem.SaveGame(data);
        Assert.IsTrue(saveSystem.HasSaveFile());

        saveSystem.DeleteSave();
        Assert.IsFalse(saveSystem.HasSaveFile());
    }

    [Test]
    public void LoadGame_ReturnsNull_WhenNoFile()
    {
        var loaded = saveSystem.LoadGame();
        Assert.IsNull(loaded);
    }
}
```

**Step 2: 執行測試確認失敗**

預期結果：FAIL（ISaveSystem, JsonSaveSystem 不存在）

**Step 3: 實作介面和 JSON 存檔**

```csharp
// Assets/Scripts/Core/SaveSystem/ISaveSystem.cs
public interface ISaveSystem
{
    void SaveGame(GameSaveData data);
    GameSaveData LoadGame();
    bool HasSaveFile();
    void DeleteSave();
}
```

```csharp
// Assets/Scripts/Core/SaveSystem/JsonSaveSystem.cs
using System.IO;
using UnityEngine;

public class JsonSaveSystem : ISaveSystem
{
    private readonly string savePath;

    public JsonSaveSystem(string path = null)
    {
        savePath = path ?? Path.Combine(Application.persistentDataPath, "save.json");
    }

    public void SaveGame(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public GameSaveData LoadGame()
    {
        if (!HasSaveFile()) return null;

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public bool HasSaveFile()
    {
        return File.Exists(savePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
            File.Delete(savePath);
    }
}
```

**Step 4: 執行測試確認通過**

預期結果：全部 PASS

**Step 5: 提交**

```bash
git add Assets/Scripts/Core/SaveSystem/ Assets/Tests/EditMode/SaveSystemTests.cs
git commit -m "feat: add ISaveSystem interface and JsonSaveSystem implementation"
```

---

### Task 5: KanaDatabase（假名資料庫服務）

**Files:**
- Create: `Assets/Scripts/Core/KanaDatabase.cs`
- Test: `Assets/Tests/EditMode/KanaDatabaseTests.cs`

**Step 1: 寫測試**

```csharp
// Assets/Tests/EditMode/KanaDatabaseTests.cs
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class KanaDatabaseTests
{
    private KanaDatabaseService database;

    [SetUp]
    public void SetUp()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("KanaDatabase");
        database = new KanaDatabaseService();
        database.LoadFromJson(jsonFile.text);
    }

    [Test]
    public void GetKanaById_ReturnsCorrectKana()
    {
        var kana = database.GetKanaById("a");
        Assert.IsNotNull(kana);
        Assert.AreEqual("あ", kana.hiragana);
        Assert.AreEqual("ア", kana.katakana);
    }

    [Test]
    public void GetKanaById_ReturnsNull_WhenNotFound()
    {
        var kana = database.GetKanaById("nonexistent");
        Assert.IsNull(kana);
    }

    [Test]
    public void GetKanaByGroup_ReturnsAllInGroup()
    {
        var aRow = database.GetKanaByGroup("a_row");
        Assert.AreEqual(5, aRow.Count);
    }

    [Test]
    public void GetKanaByLevel_ReturnsCorrectLevel()
    {
        var level1 = database.GetKanaByLevel(1);
        Assert.GreaterOrEqual(level1.Count, 5);
        foreach (var kana in level1)
        {
            Assert.AreEqual(1, kana.level);
        }
    }

    [Test]
    public void GetRandomKana_ReturnsKanaFromCorrectLevel()
    {
        var kana = database.GetRandomKana(1);
        Assert.IsNotNull(kana);
        Assert.AreEqual(1, kana.level);
    }

    [Test]
    public void GetAllKana_ReturnsAll()
    {
        var all = database.GetAllKana();
        Assert.GreaterOrEqual(all.Count, 5);
    }
}
```

**Step 2: 執行測試確認失敗**

預期結果：FAIL（KanaDatabaseService 不存在）

**Step 3: 實作 KanaDatabaseService**

```csharp
// Assets/Scripts/Core/KanaDatabase.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KanaDatabaseService
{
    private List<KanaData> kanaList = new List<KanaData>();
    private Dictionary<string, KanaData> kanaById = new Dictionary<string, KanaData>();

    public void LoadFromJson(string json)
    {
        var wrapper = JsonUtility.FromJson<KanaDatabaseWrapper>(json);
        kanaList = wrapper.kanaList;
        kanaById.Clear();
        foreach (var kana in kanaList)
        {
            kanaById[kana.id] = kana;
        }
    }

    public KanaData GetKanaById(string id)
    {
        kanaById.TryGetValue(id, out var kana);
        return kana;
    }

    public List<KanaData> GetKanaByGroup(string group)
    {
        return kanaList.Where(k => k.group == group).ToList();
    }

    public List<KanaData> GetKanaByLevel(int level)
    {
        return kanaList.Where(k => k.level == level).ToList();
    }

    public KanaData GetRandomKana(int level)
    {
        var candidates = GetKanaByLevel(level);
        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }

    public List<KanaData> GetAllKana()
    {
        return new List<KanaData>(kanaList);
    }
}
```

**Step 4: 執行測試確認通過**

預期結果：全部 PASS

**Step 5: 提交**

```bash
git add Assets/Scripts/Core/KanaDatabase.cs Assets/Tests/EditMode/KanaDatabaseTests.cs
git commit -m "feat: add KanaDatabaseService with query methods"
```

---

### Task 6: GameManager（遊戲管理器）

**Files:**
- Create: `Assets/Scripts/Core/GameManager.cs`
- Test: `Assets/Tests/EditMode/GameManagerTests.cs`

**Step 1: 寫測試**

```csharp
// Assets/Tests/EditMode/GameManagerTests.cs
using NUnit.Framework;

public class GameManagerTests
{
    [Test]
    public void GameState_DefaultsToMainMenu()
    {
        var state = new GameStateManager();
        Assert.AreEqual(GameState.MainMenu, state.CurrentState);
    }

    [Test]
    public void GameState_TransitionsCorrectly()
    {
        var state = new GameStateManager();
        state.ChangeState(GameState.StageSelect);
        Assert.AreEqual(GameState.StageSelect, state.CurrentState);

        state.ChangeState(GameState.Playing);
        Assert.AreEqual(GameState.Playing, state.CurrentState);
    }

    [Test]
    public void PlayerManager_DefaultsToPlayer1()
    {
        var pm = new PlayerManager();
        Assert.AreEqual(0, pm.ActivePlayerIndex);
    }

    [Test]
    public void PlayerManager_SwitchPlayer_TogglesIndex()
    {
        var pm = new PlayerManager();
        pm.SetPlayerCount(2);
        pm.SwitchPlayer();
        Assert.AreEqual(1, pm.ActivePlayerIndex);
        pm.SwitchPlayer();
        Assert.AreEqual(0, pm.ActivePlayerIndex);
    }

    [Test]
    public void PlayerManager_SinglePlayer_DoesNotSwitch()
    {
        var pm = new PlayerManager();
        pm.SetPlayerCount(1);
        pm.SwitchPlayer();
        Assert.AreEqual(0, pm.ActivePlayerIndex);
    }

    [Test]
    public void PlayerManager_IsTwoPlayer_ReturnsCorrectly()
    {
        var pm = new PlayerManager();
        Assert.IsFalse(pm.IsTwoPlayer);

        pm.SetPlayerCount(2);
        Assert.IsTrue(pm.IsTwoPlayer);
    }
}
```

**Step 2: 執行測試確認失敗**

預期結果：FAIL

**Step 3: 實作 GameState 和 PlayerManager**

```csharp
// Assets/Scripts/Core/GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    StageSelect,
    Playing,
    Paused,
    Result
}

// 純邏輯類別（不依賴 MonoBehaviour，方便測試）
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

// MonoBehaviour 包裝器（在場景中使用）
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
                players = new System.Collections.Generic.List<PlayerData>
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
```

**Step 4: 執行測試確認通過**

預期結果：全部 PASS

**Step 5: 提交**

```bash
git add Assets/Scripts/Core/GameManager.cs Assets/Tests/EditMode/GameManagerTests.cs
git commit -m "feat: add GameManager with GameStateManager and PlayerManager"
```

---

### Task 7: AudioManager（音效管理器）

**Files:**
- Create: `Assets/Scripts/Core/AudioManager.cs`

**Step 1: 實作 AudioManager**

此模組依賴 Unity AudioSource，主要在 PlayMode 測試。先建立程式碼，PlayMode 測試留待場景建立後補充。

```csharp
// Assets/Scripts/Core/AudioManager.cs
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource voiceSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip correctAnswer;
    [SerializeField] private AudioClip wrongAnswer;
    [SerializeField] private AudioClip catchDessert;
    [SerializeField] private AudioClip dropStrawberry;

    private Dictionary<string, AudioClip> sfxClips;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeSfxDictionary();
    }

    private void InitializeSfxDictionary()
    {
        sfxClips = new Dictionary<string, AudioClip>
        {
            { "button", buttonClick },
            { "correct", correctAnswer },
            { "wrong", wrongAnswer },
            { "catch", catchDessert },
            { "drop", dropStrawberry }
        };
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(string sfxName)
    {
        if (sfxClips.TryGetValue(sfxName, out var clip) && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayVoice(AudioClip clip)
    {
        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    public void SetVolume(float bgm, float sfx, float voice)
    {
        bgmSource.volume = Mathf.Clamp01(bgm);
        sfxSource.volume = Mathf.Clamp01(sfx);
        voiceSource.volume = Mathf.Clamp01(voice);
    }
}
```

**Step 2: 提交**

```bash
git add Assets/Scripts/Core/AudioManager.cs
git commit -m "feat: add AudioManager with BGM, SFX, and voice channels"
```

---

### Task 8: UIManager（介面管理器）

**Files:**
- Create: `Assets/Scripts/Core/UIManager.cs`

**Step 1: 實作 UIManager**

```csharp
// Assets/Scripts/Core/UIManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private CanvasGroup fadePanel;

    private Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterPanel(string panelName, GameObject panel)
    {
        panels[panelName] = panel;
    }

    public void ShowPanel(string panelName)
    {
        if (panels.TryGetValue(panelName, out var panel))
        {
            panel.SetActive(true);
        }
    }

    public void HidePanel(string panelName)
    {
        if (panels.TryGetValue(panelName, out var panel))
        {
            panel.SetActive(false);
        }
    }

    public void HideAllPanels()
    {
        foreach (var panel in panels.Values)
        {
            panel.SetActive(false);
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        if (fadePanel != null)
        {
            yield return StartCoroutine(Fade(0f, 1f, 0.3f));
        }

        SceneManager.LoadScene(sceneName);

        if (fadePanel != null)
        {
            yield return StartCoroutine(Fade(1f, 0f, 0.3f));
        }
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        fadePanel.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        fadePanel.alpha = to;
        if (to == 0f) fadePanel.gameObject.SetActive(false);
    }
}
```

**Step 2: 提交**

```bash
git add Assets/Scripts/Core/UIManager.cs
git commit -m "feat: add UIManager with panel management and scene fade transitions"
```

---

### Task 9: 主選單場景和 UI

**Files:**
- Create: `Assets/Scripts/UI/MainMenuController.cs`
- Create: `Assets/Scenes/MainMenu.unity`（Unity Editor 中建立）

**Step 1: 實作主選單控制器**

```csharp
// Assets/Scripts/UI/MainMenuController.cs
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
```

**Step 2: 在 Unity Editor 建立 MainMenu 場景**

1. File → New Scene → 儲存為 `Assets/Scenes/MainMenu.unity`
2. 建立 Canvas：
   - 加入 Image（背景，用 placeholder 顏色 #FFB6C1 粉紅色）
   - 加入 Text：「五十音甜點大冒險」（標題）
   - 加入 Button：「單人遊戲」
   - 加入 Button：「雙人遊戲」
   - 加入 Panel（nameInputPanel）：
     - InputField：玩家1名稱
     - InputField + 容器（player2NameGroup）：玩家2名稱
     - Button：「開始遊戲」
3. 建立空 GameObject `GameManager`，掛上 `GameManager.cs`
4. 建立空 GameObject `AudioManager`，掛上 `AudioManager.cs`，加 3 個 AudioSource
5. 建立空 GameObject `UIManager`，掛上 `UIManager.cs`
6. 將按鈕和 InputField 拖曳連接到 `MainMenuController`
7. File → Build Settings → 把 MainMenu 加入 Scenes in Build（index 0）

**Step 3: 提交**

```bash
git add Assets/Scripts/UI/MainMenuController.cs Assets/Scenes/MainMenu.unity
git commit -m "feat: add MainMenu scene with single/two player selection"
```

---

### Task 10: 關卡選擇場景

**Files:**
- Create: `Assets/Scripts/UI/StageSelectController.cs`
- Create: `Assets/Scenes/StageSelect.unity`（Unity Editor 中建立）
- Create: `Assets/Resources/LevelConfigs/Levels.json`

**Step 1: 建立關卡配置 JSON**

```json
// Assets/Resources/LevelConfigs/Levels.json
{
  "levels": [
    {
      "levelId": 1,
      "name": "入門",
      "sceneName": "KittyLivingRoom",
      "description": "溫馨客廳",
      "kanaGroups": ["a_row", "ka_row", "sa_row", "ta_row", "na_row"],
      "reward": "蝴蝶結茶杯",
      "rewardSprite": "sprites/rewards/bow_cup",
      "unlockCondition": "none",
      "minigames": ["stroke_practice", "dessert_catch", "card_match", "order_challenge"]
    },
    {
      "levelId": 2,
      "name": "進階",
      "sceneName": "AppleForest",
      "description": "蘋果森林花園",
      "kanaGroups": ["ha_row", "ma_row", "ya_row", "ra_row", "wa_row"],
      "reward": "蕾絲桌巾",
      "rewardSprite": "sprites/rewards/lace_cloth",
      "unlockCondition": "level_1_complete",
      "minigames": ["stroke_practice", "dessert_catch", "card_match", "order_challenge"]
    },
    {
      "levelId": 3,
      "name": "大師",
      "sceneName": "LondonStreet",
      "description": "倫敦繁華街角",
      "kanaGroups": ["dakuon", "handakuon"],
      "reward": "五層大蛋糕",
      "rewardSprite": "sprites/rewards/big_cake",
      "unlockCondition": "level_2_complete",
      "minigames": ["stroke_practice", "dessert_catch", "card_match", "order_challenge"]
    },
    {
      "levelId": 4,
      "name": "傳說",
      "sceneName": "FlagshipStore",
      "description": "華麗甜點旗艦店",
      "kanaGroups": ["youon", "sokuon"],
      "reward": "50音達人獎章",
      "rewardSprite": "sprites/rewards/master_badge",
      "unlockCondition": "level_3_complete",
      "minigames": ["stroke_practice", "dessert_catch", "card_match", "order_challenge"]
    }
  ]
}
```

**Step 2: 實作關卡選擇控制器**

```csharp
// Assets/Scripts/UI/StageSelectController.cs
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
                ? $"{level.name}\n{level.description}"
                : $"{level.name}\n🔒";

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

        // 檢查前一關是否完成（簡易判定：前一關的假名是否都學會）
        int requiredLevel = level.levelId - 1;
        return playerData.currentLevel > requiredLevel;
    }

    private void OnLevelSelected(LevelConfig level)
    {
        GameManager.Instance.StateManager.ChangeState(GameState.Playing);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("button");

        // MVP: 直接進入卡牌翻翻看
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
```

**Step 3: 在 Unity Editor 建立場景**

1. 建立 `Assets/Scenes/StageSelect.unity`
2. Canvas 加入 4 個 Button（關卡按鈕）和返回按鈕
3. 建立 LevelButton Prefab（Text + Button）存到 `Assets/Prefabs/`
4. File → Build Settings → 加入 StageSelect 場景

**Step 4: 提交**

```bash
git add Assets/Scripts/UI/StageSelectController.cs Assets/Resources/LevelConfigs/Levels.json Assets/Scenes/StageSelect.unity Assets/Prefabs/
git commit -m "feat: add StageSelect scene with level unlocking system"
```

---

### Task 11: 卡牌翻翻看 - 核心邏輯

**Files:**
- Create: `Assets/Scripts/Gameplay/Stage2_ListeningChallenge/CardMatchLogic.cs`
- Test: `Assets/Tests/EditMode/CardMatchLogicTests.cs`

**Step 1: 寫測試**

```csharp
// Assets/Tests/EditMode/CardMatchLogicTests.cs
using NUnit.Framework;
using System.Collections.Generic;

public class CardMatchLogicTests
{
    private CardMatchLogic logic;

    [SetUp]
    public void SetUp()
    {
        var kanaList = new List<KanaData>
        {
            new KanaData { id = "a", hiragana = "あ", katakana = "ア" },
            new KanaData { id = "i", hiragana = "い", katakana = "イ" },
            new KanaData { id = "u", hiragana = "う", katakana = "ウ" }
        };
        logic = new CardMatchLogic();
        logic.Initialize(kanaList, 3, 2); // 3x2 grid = 6 cards = 3 pairs
    }

    [Test]
    public void Initialize_CreatesCorrectNumberOfCards()
    {
        Assert.AreEqual(6, logic.Cards.Count);
    }

    [Test]
    public void Initialize_HasMatchingPairs()
    {
        // 每個假名應有平假名和片假名各一張
        int hiraganaCount = 0;
        int katakanaCount = 0;
        foreach (var card in logic.Cards)
        {
            if (card.isKatakana) katakanaCount++;
            else hiraganaCount++;
        }
        Assert.AreEqual(3, hiraganaCount);
        Assert.AreEqual(3, katakanaCount);
    }

    [Test]
    public void FlipCard_FirstCard_SetsFirstFlipped()
    {
        var result = logic.FlipCard(0);
        Assert.AreEqual(FlipResult.FirstCard, result);
        Assert.AreEqual(1, logic.MoveCount);
    }

    [Test]
    public void FlipCard_MatchingPair_ReturnsMatch()
    {
        // 找到配對的兩張卡
        int firstIndex = -1;
        int secondIndex = -1;
        for (int i = 0; i < logic.Cards.Count; i++)
        {
            for (int j = i + 1; j < logic.Cards.Count; j++)
            {
                if (logic.Cards[i].kanaId == logic.Cards[j].kanaId)
                {
                    firstIndex = i;
                    secondIndex = j;
                    break;
                }
            }
            if (firstIndex >= 0) break;
        }

        logic.FlipCard(firstIndex);
        var result = logic.FlipCard(secondIndex);
        Assert.AreEqual(FlipResult.Match, result);
    }

    [Test]
    public void FlipCard_NonMatchingPair_ReturnsMismatch()
    {
        // 找到不配對的兩張卡
        int firstIndex = 0;
        int secondIndex = -1;
        for (int i = 1; i < logic.Cards.Count; i++)
        {
            if (logic.Cards[i].kanaId != logic.Cards[firstIndex].kanaId)
            {
                secondIndex = i;
                break;
            }
        }

        logic.FlipCard(firstIndex);
        var result = logic.FlipCard(secondIndex);
        Assert.AreEqual(FlipResult.Mismatch, result);
    }

    [Test]
    public void IsComplete_FalseAtStart()
    {
        Assert.IsFalse(logic.IsComplete);
    }

    [Test]
    public void MoveCount_StartsAtZero()
    {
        Assert.AreEqual(0, logic.MoveCount);
    }
}
```

**Step 2: 執行測試確認失敗**

預期結果：FAIL

**Step 3: 實作卡牌翻翻看邏輯**

```csharp
// Assets/Scripts/Gameplay/Stage2_ListeningChallenge/CardMatchLogic.cs
using System.Collections.Generic;

public enum FlipResult
{
    FirstCard,
    Match,
    Mismatch
}

public class CardInfo
{
    public string kanaId;
    public string displayText;
    public bool isKatakana;
    public bool isMatched;
    public bool isFaceUp;
}

public class CardMatchLogic
{
    public List<CardInfo> Cards { get; private set; } = new List<CardInfo>();
    public int MoveCount { get; private set; } = 0;
    public bool IsComplete { get; private set; } = false;
    public int MatchedPairs { get; private set; } = 0;

    private int firstFlippedIndex = -1;
    private int totalPairs;

    public void Initialize(List<KanaData> kanaList, int cols, int rows)
    {
        Cards.Clear();
        MoveCount = 0;
        MatchedPairs = 0;
        IsComplete = false;
        firstFlippedIndex = -1;

        totalPairs = (cols * rows) / 2;

        // 取得需要的假名數量
        var selectedKana = new List<KanaData>();
        for (int i = 0; i < totalPairs && i < kanaList.Count; i++)
        {
            selectedKana.Add(kanaList[i]);
        }

        // 每個假名建立平假名和片假名兩張卡
        foreach (var kana in selectedKana)
        {
            Cards.Add(new CardInfo
            {
                kanaId = kana.id,
                displayText = kana.hiragana,
                isKatakana = false,
                isMatched = false,
                isFaceUp = false
            });

            Cards.Add(new CardInfo
            {
                kanaId = kana.id,
                displayText = kana.katakana,
                isKatakana = true,
                isMatched = false,
                isFaceUp = false
            });
        }

        // 洗牌
        Shuffle(Cards);
    }

    public FlipResult FlipCard(int index)
    {
        if (index < 0 || index >= Cards.Count) return FlipResult.Mismatch;
        if (Cards[index].isMatched || Cards[index].isFaceUp) return FlipResult.Mismatch;

        Cards[index].isFaceUp = true;

        if (firstFlippedIndex == -1)
        {
            firstFlippedIndex = index;
            MoveCount++;
            return FlipResult.FirstCard;
        }

        MoveCount++;
        var firstCard = Cards[firstFlippedIndex];
        var secondCard = Cards[index];

        if (firstCard.kanaId == secondCard.kanaId)
        {
            firstCard.isMatched = true;
            secondCard.isMatched = true;
            MatchedPairs++;
            firstFlippedIndex = -1;

            if (MatchedPairs >= totalPairs)
            {
                IsComplete = true;
            }

            return FlipResult.Match;
        }

        // 不配對：翻回去
        firstCard.isFaceUp = false;
        secondCard.isFaceUp = false;
        firstFlippedIndex = -1;

        return FlipResult.Mismatch;
    }

    public string GetMatchedKanaId()
    {
        // 回傳最近配對成功的假名 ID（用於播放音效）
        if (MatchedPairs == 0) return null;
        foreach (var card in Cards)
        {
            if (card.isMatched) return card.kanaId;
        }
        return null;
    }

    private void Shuffle<T>(List<T> list)
    {
        var rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }
}
```

**Step 4: 執行測試確認通過**

預期結果：全部 PASS

**Step 5: 提交**

```bash
git add Assets/Scripts/Gameplay/Stage2_ListeningChallenge/CardMatchLogic.cs Assets/Tests/EditMode/CardMatchLogicTests.cs
git commit -m "feat: add CardMatchLogic with pair matching and scoring"
```

---

### Task 12: 卡牌翻翻看 - Unity 場景和 UI

**Files:**
- Create: `Assets/Scripts/Gameplay/Stage2_ListeningChallenge/CardMatchController.cs`
- Create: `Assets/Scripts/Gameplay/Stage2_ListeningChallenge/CardView.cs`
- Create: `Assets/Scenes/CardMatch.unity`（Unity Editor 中建立）

**Step 1: 實作卡片視覺元件**

```csharp
// Assets/Scripts/Gameplay/Stage2_ListeningChallenge/CardView.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardView : MonoBehaviour
{
    [SerializeField] private Text kanaText;
    [SerializeField] private Image cardBackground;
    [SerializeField] private Color faceDownColor = new Color(0.8f, 0.6f, 0.8f);
    [SerializeField] private Color faceUpColor = Color.white;
    [SerializeField] private Color matchedColor = new Color(0.7f, 1f, 0.7f);

    private Button button;
    private int cardIndex;
    private System.Action<int> onClickCallback;

    public void Initialize(int index, System.Action<int> onClick)
    {
        cardIndex = index;
        onClickCallback = onClick;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClicked);
        ShowFaceDown();
    }

    public void ShowFaceUp(string displayText)
    {
        kanaText.text = displayText;
        kanaText.gameObject.SetActive(true);
        cardBackground.color = faceUpColor;
    }

    public void ShowFaceDown()
    {
        kanaText.text = "?";
        cardBackground.color = faceDownColor;
    }

    public void ShowMatched(string displayText)
    {
        kanaText.text = displayText;
        kanaText.gameObject.SetActive(true);
        cardBackground.color = matchedColor;
        button.interactable = false;
    }

    private void OnClicked()
    {
        onClickCallback?.Invoke(cardIndex);
    }
}
```

**Step 2: 實作卡牌翻翻看場景控制器**

```csharp
// Assets/Scripts/Gameplay/Stage2_ListeningChallenge/CardMatchController.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardMatchController : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridCols = 3;
    [SerializeField] private int gridRows = 2;

    [Header("UI References")]
    [SerializeField] private GridLayoutGroup cardGrid;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Text moveCountText;
    [SerializeField] private Text playerNameText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button retryButton;

    private CardMatchLogic logic;
    private List<CardView> cardViews = new List<CardView>();
    private bool isProcessing = false;

    private void Start()
    {
        resultPanel.SetActive(false);

        var gm = GameManager.Instance;
        var kanaDb = gm.GetKanaDatabase();
        var playerData = gm.GetActivePlayerData();

        playerNameText.text = playerData.playerName;

        // 取得目前關卡的假名
        var kanaList = kanaDb.GetKanaByLevel(playerData.currentLevel);
        if (kanaList.Count == 0)
        {
            kanaList = kanaDb.GetKanaByGroup("a_row");
        }

        logic = new CardMatchLogic();
        logic.Initialize(kanaList, gridCols, gridRows);

        CreateCardViews();
        UpdateUI();

        backButton.onClick.AddListener(OnBackClicked);
        retryButton.onClick.AddListener(OnRetryClicked);
    }

    private void CreateCardViews()
    {
        for (int i = 0; i < logic.Cards.Count; i++)
        {
            var cardObj = Instantiate(cardPrefab, cardGrid.transform);
            var cardView = cardObj.GetComponent<CardView>();
            cardView.Initialize(i, OnCardClicked);
            cardViews.Add(cardView);
        }
    }

    private void OnCardClicked(int index)
    {
        if (isProcessing) return;

        var result = logic.FlipCard(index);
        UpdateUI();

        switch (result)
        {
            case FlipResult.FirstCard:
                cardViews[index].ShowFaceUp(logic.Cards[index].displayText);
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("button");
                break;

            case FlipResult.Match:
                // 找到配對的兩張卡，顯示配對成功
                for (int i = 0; i < logic.Cards.Count; i++)
                {
                    if (logic.Cards[i].isMatched)
                    {
                        cardViews[i].ShowMatched(logic.Cards[i].displayText);
                    }
                }

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("correct");

                if (logic.IsComplete)
                {
                    StartCoroutine(ShowResult());
                }
                break;

            case FlipResult.Mismatch:
                isProcessing = true;
                // 先顯示第二張卡，然後延遲翻回
                cardViews[index].ShowFaceUp(logic.Cards[index].displayText);
                StartCoroutine(HideMismatchedCards());

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX("wrong");
                break;
        }
    }

    private IEnumerator HideMismatchedCards()
    {
        yield return new WaitForSeconds(0.8f);

        for (int i = 0; i < logic.Cards.Count; i++)
        {
            if (!logic.Cards[i].isMatched && !logic.Cards[i].isFaceUp)
            {
                cardViews[i].ShowFaceDown();
            }
        }
        isProcessing = false;
    }

    private IEnumerator ShowResult()
    {
        yield return new WaitForSeconds(0.5f);

        var gm = GameManager.Instance;
        var playerData = gm.GetActivePlayerData();

        // 計算星級
        int stars = CalculateStars(logic.MoveCount, logic.Cards.Count / 2);

        // 儲存分數
        playerData.scores.Add(new MinigameScore
        {
            minigameId = "card_match",
            bestScore = logic.MoveCount,
            attempts = 1
        });
        gm.Save();

        // 顯示結果
        string starDisplay = new string('★', stars) + new string('☆', 3 - stars);
        resultText.text = $"{playerData.playerName}\n步數：{logic.MoveCount}\n{starDisplay}";
        resultPanel.SetActive(true);

        // 雙人模式：檢查是否需要換人
        if (gm.PlayerManager.IsTwoPlayer && gm.PlayerManager.ActivePlayerIndex == 0)
        {
            resultText.text += "\n\n接下來換下一位玩家！";
        }
    }

    private int CalculateStars(int moves, int pairs)
    {
        if (moves <= pairs) return 3;         // 完美
        if (moves <= pairs * 2) return 2;     // 不錯
        return 1;                              // 完成
    }

    private void UpdateUI()
    {
        moveCountText.text = $"步數：{logic.MoveCount}";
    }

    private void OnBackClicked()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.LoadSceneWithFade("StageSelect");
        else
            GameManager.Instance.LoadScene("StageSelect");
    }

    private void OnRetryClicked()
    {
        // 雙人模式切換玩家
        var gm = GameManager.Instance;
        if (gm.PlayerManager.IsTwoPlayer)
        {
            gm.PlayerManager.SwitchPlayer();
        }

        // 重新載入場景
        if (UIManager.Instance != null)
            UIManager.Instance.LoadSceneWithFade("CardMatch");
        else
            gm.LoadScene("CardMatch");
    }
}
```

**Step 3: 在 Unity Editor 建立場景**

1. 建立 `Assets/Scenes/CardMatch.unity`
2. Canvas 加入：
   - `GridLayoutGroup`（CardGrid，放卡片的網格）
   - `Text`（moveCountText，顯示步數）
   - `Text`（playerNameText，顯示玩家名稱）
   - `Panel`（resultPanel，結算畫面）
   - `Button`（backButton，返回）
   - `Button`（retryButton，重玩/換人）
3. 建立 Card Prefab：
   - 在 `Assets/Prefabs/` 建立 `Card.prefab`
   - 結構：Button > Image（cardBackground） + Text（kanaText）
   - 掛上 `CardView.cs`
4. File → Build Settings → 加入 CardMatch 場景

**Step 4: 提交**

```bash
git add Assets/Scripts/Gameplay/Stage2_ListeningChallenge/CardMatchController.cs Assets/Scripts/Gameplay/Stage2_ListeningChallenge/CardView.cs Assets/Scenes/CardMatch.unity Assets/Prefabs/
git commit -m "feat: add CardMatch scene with UI, grid layout, and result screen"
```

---

### Task 13: 整合測試和 MVP 驗收

**Step 1: 在 Build Settings 確認場景順序**

```
0: Scenes/MainMenu
1: Scenes/StageSelect
2: Scenes/CardMatch
```

**Step 2: 手動測試流程**

在 Unity Editor 中按 Play，驗證以下流程：

1. ✅ 主選單顯示「單人遊戲」和「雙人遊戲」
2. ✅ 點擊「單人遊戲」→ 顯示名稱輸入
3. ✅ 輸入名稱 → 點擊「開始遊戲」→ 進入關卡選擇
4. ✅ 入門關卡可選，其他關卡鎖定
5. ✅ 選擇入門 → 進入卡牌翻翻看
6. ✅ 翻卡 → 配對判定正確
7. ✅ 全部配對完成 → 顯示結果和星級
8. ✅ 返回按鈕正常
9. ✅ 存檔正常（關閉重開後進度保留）

**Step 3: 驗證雙人模式**

1. ✅ 選擇「雙人遊戲」→ 顯示兩個名稱輸入
2. ✅ 玩家1完成後提示換人
3. ✅ 玩家2完成後顯示比較結果

**Step 4: 修復發現的問題**

記錄並修復測試中發現的 bug。

**Step 5: 提交**

```bash
git add -A
git commit -m "fix: integration fixes from MVP testing"
```

---

## Phase 2~4 概要（後續實作計畫）

### Phase 2: 核心玩法完整

- **Task 14:** 甜點接接樂邏輯（FallingDessertLogic + 測試）
- **Task 15:** 甜點接接樂場景和 UI
- **Task 16:** 點餐大作戰邏輯（OrderChallengeLogic + 測試）
- **Task 17:** 點餐大作戰場景和 UI
- **Task 18:** 字源聯想記憶畫面
- **Task 19:** 筆順練習（StrokePractice + 路徑判定）
- **Task 20:** 擴充假名資料（か行~わ行全部清音）
- **Task 21:** 星級評價系統

### Phase 3: 完整內容

- **Task 22:** 濁音、半濁音假名資料
- **Task 23:** 拗音、促音假名資料
- **Task 24:** 4個關卡場景和背景
- **Task 25:** 收集品系統 + 甜點屋展示
- **Task 26:** N5 單字資料庫完整填入
- **Task 27:** 雙人模式完善（結果比較畫面優化）

### Phase 4: 打磨發布

- **Task 28:** AI 生成角色和場景美術
- **Task 29:** AI 語音生成（50 音發音檔案）
- **Task 30:** 免費音效庫整合（BGM + SFX）
- **Task 31:** WebGL 建置和測試
- **Task 32:** Windows 建置和測試

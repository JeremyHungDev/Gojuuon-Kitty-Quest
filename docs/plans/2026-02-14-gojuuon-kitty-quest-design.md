# Gojuuon Kitty Quest - 五十音甜點大冒險 設計文件

## 概述

一款以可愛貓咪角色為主題的日語五十音學習遊戲。玩家協助角色在倫敦開設日式甜點屋，透過三階段漸進式教學（字源記憶 → 聽力辨識 → 單字應用）學習平假名與片假名。

## 故事背景

可愛的貓咪角色準備在倫敦開一家全新的日式甜點屋，但製作甜點的「文字配方」不小心被調皮的家族成員藏起來了！玩家需要協助角色前往不同的地圖場景，找回散落的平假名與片假名，完成甜點製作。

## 技術決策

| 項目 | 決策 | 原因 |
|------|------|------|
| 遊戲引擎 | Unity (C#) | 跨平台、2D支援好、用戶熟悉C# |
| 架構模式 | 模組化架構 + 設計模式 | 易維護、易擴展、支援雙人模式 |
| 存檔系統 | 本地 JSON 序列化 | 輕量、簡單、資料量小 (<10KB) |
| 美術資源 | 先用 placeholder，之後 AI 生成替換 | 快速原型開發 |
| 音效配音 | 免費音效庫 + AI 語音生成 | 成本低、品質中等 |
| 發布平台 | 優先 Windows + WebGL，之後 Android/iOS | 漸進式發布 |
| 難度上限 | JLPT N5 等級 | 符合五十音學習定位 |

## 整體架構

### 專案結構

```
Gojuuon-Kitty-Quest/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/                    # 核心系統
│   │   │   ├── GameManager.cs
│   │   │   ├── SaveSystem/
│   │   │   │   ├── ISaveSystem.cs
│   │   │   │   └── JsonSaveSystem.cs
│   │   │   ├── AudioManager.cs
│   │   │   └── UIManager.cs
│   │   ├── Data/                    # 資料結構
│   │   │   ├── KanaData.cs
│   │   │   ├── GameSaveData.cs
│   │   │   └── LevelConfig.cs
│   │   ├── Gameplay/                # 遊戲玩法
│   │   │   ├── Stage1_MemoryLearning/
│   │   │   ├── Stage2_ListeningChallenge/
│   │   │   └── Stage3_WordQuiz/
│   │   └── UI/                      # UI 控制器
│   ├── Resources/
│   │   ├── KanaDatabase.json        # 50音資料庫
│   │   └── LevelConfigs/            # 關卡配置
│   ├── Scenes/
│   │   ├── MainMenu.unity
│   │   ├── Stage1_Memory.unity
│   │   ├── Stage2_Listening.unity
│   │   └── Stage3_WordQuiz.unity
│   ├── Sprites/                     # 圖片素材
│   ├── Audio/                       # 音效素材
│   └── Prefabs/                     # 預製體（可重用物件模板）
```

### 設計模式

- **Singleton Pattern**：GameManager, AudioManager, SaveSystem 等全域系統
- **MVC Pattern**：UI 與邏輯分離
- **Factory Pattern**：動態生成假名卡片、甜點物件
- **Strategy Pattern**：不同小遊戲的玩法邏輯
- **Interface**：SaveSystem 用介面設計，未來可擴展

### 系統關係圖

```
           ┌──────────────┐
           │ GameManager  │ ← 總指揮
           └──────┬───────┘
        ┌─────────┼─────────┐
        ▼         ▼         ▼
 ┌────────┐ ┌──────────┐ ┌──────────┐
 │SaveSys │ │AudioMgr  │ │ UIMgr    │
 └────────┘ └──────────┘ └──────────┘
                  ▲
                  │
           ┌──────┴───────┐
           │ KanaDatabase │ ← 提供資料
           └──────────────┘
                  ▲
                  │
        ┌─────────┼─────────┐
        ▼         ▼         ▼
  ┌─────────┐┌─────────┐┌─────────┐
  │Stage1   ││Stage2   ││Stage3   │
  │筆順練習 ││甜點接接 ││點餐大戰 │
  └─────────┘└─────────┘└─────────┘
```

## 資料結構設計

### 五十音資料庫（KanaDatabase.json）

```json
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
    }
  ]
}
```

### 關卡配置

```json
{
  "levels": [
    {
      "levelId": 1,
      "name": "入門",
      "sceneName": "KittyLivingRoom",
      "description": "溫馨客廳",
      "kanaGroups": ["a_row", "ka_row", "sa_row", "ta_row", "na_row"],
      "reward": "蝴蝶結茶杯",
      "unlockCondition": "none",
      "minigames": ["stroke_practice", "dessert_catch", "card_match"]
    },
    {
      "levelId": 2,
      "name": "進階",
      "sceneName": "AppleForest",
      "description": "蘋果森林花園",
      "kanaGroups": ["ha_row", "ma_row", "ya_row", "ra_row", "wa_row"],
      "reward": "蕾絲桌巾",
      "unlockCondition": "level_1_complete"
    },
    {
      "levelId": 3,
      "name": "大師",
      "sceneName": "LondonStreet",
      "description": "倫敦繁華街角",
      "kanaGroups": ["dakuon", "handakuon"],
      "reward": "五層大蛋糕",
      "unlockCondition": "level_2_complete"
    },
    {
      "levelId": 4,
      "name": "傳說",
      "sceneName": "FlagshipStore",
      "description": "華麗甜點旗艦店",
      "kanaGroups": ["youon", "sokuon"],
      "reward": "50音達人獎章",
      "unlockCondition": "level_3_complete"
    }
  ]
}
```

### 玩家存檔資料

```csharp
[System.Serializable]
public class GameSaveData
{
    public List<PlayerData> players;
    public string lastPlayDate;
    public int totalPlayTimeSeconds;
}

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int currentLevel;
    public List<string> learnedKana;
    public List<string> collectedRewards;
    public List<MinigameScore> scores;
}

[System.Serializable]
public class MinigameScore
{
    public string minigameId;
    public string kanaId;
    public int bestScore;
    public int attempts;
}
```

### N5 單字資料庫

```json
{
  "words": [
    {
      "id": "ringo",
      "kana": "りんご",
      "katakana": "リンゴ",
      "chinese": "蘋果",
      "category": "fruit",
      "difficulty": 1,
      "sprite": "sprites/foods/apple.png"
    }
  ]
}
```

單字分類（N5 範圍，甜點屋主題）：
- 水果：りんご、いちご、バナナ、みかん、ぶどう
- 甜點：ケーキ、プリン、チョコ、クッキー、パフェ
- 飲料：おちゃ、みず、ジュース、コーヒー、ミルク
- 食器/場景：おさら、コップ、おみせ

## 核心系統模組

### GameManager

負責遊戲整體流程控制：
- 管理遊戲狀態切換（MainMenu → StageSelect → Playing → Paused → Result）
- 管理雙人模式的玩家切換
- 協調各子系統

### SaveSystem

介面設計，未來可擴展：
- `ISaveSystem` 介面定義存檔行為
- `JsonSaveSystem` 實作 JSON 本地存檔
- 自動存檔（每完成一個小遊戲即存）

### AudioManager

三個獨立音訊通道：
- `bgmSource`：背景音樂（持續播放）
- `sfxSource`：音效（按鈕、答對、答錯等不同音效檔案）
- `voiceSource`：語音（假名發音）
- 三者同時播放、互不干擾、各自控制音量

### KanaDatabase

啟動時從 JSON 載入所有假名資料：
- `GetKanaById(string id)`
- `GetKanaByGroup(string group)`
- `GetKanaByLevel(int level)`
- `GetRandomKana(int level)`

### UIManager

管理 UI 面板和場景切換：
- 面板顯示/隱藏
- 場景切換含過場動畫
- 雙人模式玩家切換提示

## 三階段遊戲玩法

### 第一階段：字源聯想記憶

1. 顯示假名「あ」
2. 顯示字源「安」+ 動畫演變過程（安 → あ）
3. 角色做出相關動作
4. 筆順練習：沿著軌跡描紅（滑鼠/觸控）
5. 正確完成 → 草莓掉落特效
6. 記錄為「已學會」

筆順判定：玩家描繪路徑與目標路徑的誤差在容許範圍內即通過。

### 第二階段：聽力與辨識挑戰

#### 甜點接接樂
- 畫面上方掉落帶有假名的甜點
- 語音播放目標假名發音
- 玩家移動籃子接住正確假名的甜點
- 接對加分、接錯扣分、漏接不扣分

#### 卡牌翻翻看
- 翻開卡牌，將平假名（あ）與片假名（ア）配對
- 配對成功 → 卡牌消失 + 發音播放
- 計算完成步數

### 第三階段：單字大考驗

#### 點餐大作戰
- 角色走進甜點屋點餐
- 對話框顯示目標單字（如「リンゴ」）
- 玩家從假名碎片中依序選出正確的假名
- 計時模式，比誰拼得快

## 難度曲線

| 關卡 | 甜點接接樂 | 卡牌翻翻看 | 點餐大作戰 |
|------|-----------|-----------|-----------|
| 入門 | 慢速、4個甜點、清音 | 3x2 網格 | 2字、無時間限制 |
| 進階 | 中速、6個甜點、混合片假名 | 4x3 網格 | 3字、寬鬆計時 |
| 大師 | 快速、8個甜點、含濁音 | 4x4 網格 | 3~4字、普通計時 |
| 傳說 | 極速、10個甜點、含拗音促音 | 5x4 網格 | 4~5字、嚴格計時 |

最高難度對應 JLPT N5 等級。

## 雙人模式

### 模式：本地輪流制

同一台裝置，兩位玩家輪流挑戰同一關卡，比較分數。

流程：
1. 主選單選擇「雙人模式」
2. 輸入玩家1和玩家2名稱
3. 選擇關卡和小遊戲
4. 玩家1完成 → 記錄分數
5. 切換提示 → 玩家2完成 → 記錄分數
6. 結果比較畫面

### 各小遊戲計分方式

| 小遊戲 | 計分標準 | 比較方式 |
|--------|---------|---------|
| 筆順練習 | 準確度 % | 誰更精確 |
| 甜點接接樂 | 接到正確數量 | 誰接得多 |
| 卡牌翻翻看 | 完成步數 | 誰步數少 |
| 點餐大作戰 | 完成時間 | 誰拼得快 |

## 進度與獎勵系統

### 星級評價（單人）
- ⭐ 完成挑戰
- ⭐⭐ 達到基本分數門檻
- ⭐⭐⭐ 達到高分門檻

### 關卡解鎖
每個關卡的所有假名至少獲得 ⭐ 即可解鎖下一關。

### 收集品：甜點屋展示
- 入門完成 → 蝴蝶結茶杯
- 進階完成 → 蕾絲桌巾
- 大師完成 → 五層大蛋糕
- 傳說完成 → 50音達人獎章

收集品展示在「甜點屋」場景中，全部收集完觸發大完成動畫。

## 開發優先順序

### MVP（第一階段）
1. Unity 2D 專案初始化
2. 核心系統（GameManager、SaveSystem、AudioManager、KanaDatabase）
3. 主選單 + 關卡選擇畫面
4. 卡牌翻翻看（最容易實作的小遊戲）
5. 入門關卡あ行（あいうえお）5個假名
6. 單人模式
7. 基本 UI（placeholder 素材）

### 第二階段：核心玩法完整
8. 甜點接接樂
9. 點餐大作戰
10. 字源聯想記憶教學畫面
11. 筆順練習
12. 所有清音假名資料（あ行~わ行）
13. 星級評價系統

### 第三階段：完整內容
14. 濁音、半濁音、拗音、促音資料
15. 4個關卡場景全部完成
16. 收集品系統 + 甜點屋展示
17. 雙人模式
18. N5 單字資料庫

### 第四階段：打磨發布
19. AI 生成美術替換 placeholder
20. AI 語音生成（假名發音）
21. 免費音效庫整合（BGM + 音效）
22. WebGL 發布測試
23. Windows 發布

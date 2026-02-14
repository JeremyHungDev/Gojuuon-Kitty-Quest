# Unity 操作指南 - Gojuuon Kitty Quest

本指南假設你完全沒有使用過 Unity，從安裝到執行遊戲的完整步驟。

---

## 第一部分：安裝 Unity

### Step 1：下載 Unity Hub

1. 前往 https://unity.com/download
2. 點擊「Download Unity Hub」
3. 執行安裝程式，按照指示安裝

### Step 2：安裝 Unity Editor

1. 打開 Unity Hub
2. 左側選單點擊「Installs」
3. 點擊右上角「Install Editor」
4. 選擇 **Unity 2022 LTS**（或更新的 LTS 版本）
5. 在模組選擇頁面，勾選：
   - **WebGL Build Support**（用於網頁版發布）
   - **Windows Build Support (IL2CPP)**（通常已預設勾選）
6. 點擊「Install」，等待下載完成（約 2-5 GB）

---

## 第二部分：開啟專案

### Step 3：用 Unity Hub 開啟專案

1. 打開 Unity Hub
2. 左側選單點擊「Projects」
3. 點擊右上角「Open」>「Add project from disk」
4. 選擇資料夾：`c:\Users\KHUser\source\repos\KittyLanguage`
5. Unity 會偵測到 `Assets/` 資料夾，自動識別為 Unity 專案
6. 如果 Unity 提示「No Unity version」，選擇你安裝的版本
7. 點擊專案名稱開啟（第一次開啟需要幾分鐘匯入）

> **注意：** 如果 Unity 無法識別為專案，改用以下方式：
> 1. Unity Hub → Projects → 「New Project」
> 2. Template 選「2D (URP)」
> 3. Project name 輸入 `GojuuonKittyQuest`
> 4. Location 選一個暫時位置
> 5. 建立後，關閉 Unity
> 6. 把我們的 `Assets/` 資料夾複製到新專案中覆蓋
> 7. 重新用 Unity Hub 開啟

### Step 4：認識 Unity 介面

開啟後你會看到以下面板：

```
┌────────────────────────────────────────────────┐
│  Scene（場景檢視）        │  Game（遊戲預覽）   │
│  ← 在這裡拖放物件         │  ← 按 Play 後在這看│
├────────────────────────────────────────────────│
│  Hierarchy（層級面板）     │  Inspector（屬性）  │
│  ← 場景中所有物件的列表    │  ← 選中物件的屬性   │
├────────────────────────────────────────────────│
│  Project（專案檔案）                            │
│  ← 所有檔案，類似檔案總管                        │
└────────────────────────────────────────────────┘
```

**各面板的位置可能不同，如果找不到：** 上方選單 → Window → 選擇要開啟的面板

---

## 第三部分：建立場景

我們需要建立 3 個場景：MainMenu、StageSelect、CardMatch。

### Step 5：建立 MainMenu 場景

1. **建立新場景：**
   - 上方選單 → File → New Scene
   - 選擇「Basic (URP)」模板 → Create
   - File → Save As → 存到 `Assets/Scenes/MainMenu.unity`

2. **建立核心管理物件（只在 MainMenu 建立，因為使用 DontDestroyOnLoad）：**

   在 Hierarchy 面板（左邊）空白處右鍵：

   a. **建立 GameManager：**
      - 右鍵 → Create Empty
      - 在 Inspector（右邊）把名稱改為 `GameManager`
      - 點擊 Inspector 最下方「Add Component」
      - 搜尋並選擇「GameManager」（我們寫的腳本）

   b. **建立 AudioManager：**
      - 右鍵 → Create Empty，命名為 `AudioManager`
      - Add Component → 搜尋「AudioManager」（我們的腳本）
      - Add Component → 搜尋「Audio Source」→ 加入 **3 次**（共 3 個 AudioSource）
      - 在 Inspector 中，找到 AudioManager 腳本區塊：
        - 把第 1 個 Audio Source 拖到「Bgm Source」欄位
        - 把第 2 個 Audio Source 拖到「Sfx Source」欄位
        - 把第 3 個 Audio Source 拖到「Voice Source」欄位

   c. **建立 UIManager：**
      - 右鍵 → Create Empty，命名為 `UIManager`
      - Add Component → 搜尋「UIManager」（我們的腳本）
      - （Fade Panel 之後再設定）

3. **建立主選單 UI：**

   a. **建立 Canvas：**
      - Hierarchy 右鍵 → UI → Canvas
      - 會自動建立 Canvas 和 EventSystem

   b. **設定 Canvas：**
      - 選中 Canvas
      - Inspector 中找到 Canvas Scaler
      - UI Scale Mode 改為「Scale With Screen Size」
      - Reference Resolution 設為 1920 x 1080

   c. **建立背景：**
      - 在 Canvas 上右鍵 → UI → Image
      - 命名為「Background」
      - Inspector 中：
        - Rect Transform → 點擊左邊的方框圖示（Anchor Presets）
        - 按住 Alt 鍵，點擊右下角的「Stretch-Stretch」（撐滿整個畫面）
        - Color 點擊，改為粉紅色 (#FFB6C1)

   d. **建立標題文字：**
      - Canvas 右鍵 → UI → Text
      - 命名為「TitleText」
      - Inspector 中：
        - Text 欄位輸入：「五十音甜點大冒險」
        - Font Size：48
        - Alignment：置中
        - Color：深咖啡色
        - Rect Transform → Pos Y：200（往上移）
        - Width：600, Height：80

   e. **建立「單人遊戲」按鈕：**
      - Canvas 右鍵 → UI → Button
      - 命名為「SinglePlayerButton」
      - 展開按鈕，選中裡面的 Text，改文字為「單人遊戲」
      - 選中按鈕本體，Rect Transform：
        - Pos Y：50
        - Width：300, Height：60

   f. **建立「雙人遊戲」按鈕：**
      - Canvas 右鍵 → UI → Button
      - 命名為「TwoPlayerButton」
      - 裡面的 Text 改為「雙人遊戲」
      - Rect Transform：
        - Pos Y：-30
        - Width：300, Height：60

   g. **建立名稱輸入面板：**
      - Canvas 右鍵 → UI → Panel
      - 命名為「NameInputPanel」
      - Color 改為半透明白色 (255,255,255,200)
      - Rect Transform：Width：500, Height：300, Pos Y：-50

      在 NameInputPanel 裡面建立：

      - 右鍵 → UI → Input Field
        - 命名為「Player1NameInput」
        - Placeholder Text 改為「玩家1名稱」
        - Pos Y：60, Width：350, Height：40

      - 建立一個 Empty GameObject 命名為「Player2NameGroup」
        - 在裡面右鍵 → UI → Input Field
        - 命名為「Player2NameInput」
        - Placeholder Text 改為「玩家2名稱」
        - Player2NameGroup 的 Pos Y：0, Width：350, Height：40

      - 右鍵 → UI → Button
        - 命名為「StartGameButton」
        - Text 改為「開始遊戲」
        - Pos Y：-60, Width：200, Height：50

4. **掛上主選單控制器：**
   - 在 Canvas 上 Add Component → 搜尋「MainMenuController」
   - 在 Inspector 中，把以下物件拖曳到對應欄位：
     - Single Player Button → `SinglePlayerButton`
     - Two Player Button → `TwoPlayerButton`
     - Name Input Panel → `NameInputPanel`
     - Player1 Name Input → `Player1NameInput`
     - Player2 Name Input → `Player2NameInput`
     - Player2 Name Group → `Player2NameGroup`
     - Start Game Button → `StartGameButton`

5. **儲存場景：** Ctrl + S

---

### Step 6：建立 StageSelect 場景

1. File → New Scene → Basic (URP) → Create
2. File → Save As → `Assets/Scenes/StageSelect.unity`

3. **建立 Canvas：**（和 MainMenu 一樣的設定）
   - Hierarchy 右鍵 → UI → Canvas
   - Canvas Scaler → Scale With Screen Size → 1920 x 1080

4. **建立背景：**
   - Canvas 右鍵 → UI → Image → 命名「Background」
   - 撐滿畫面，顏色設為淺藍色 (#B6D7FF)

5. **建立標題：**
   - Canvas 右鍵 → UI → Text → 命名「Title」
   - Text：「選擇關卡」
   - Font Size：36, Pos Y：350

6. **建立關卡按鈕容器：**
   - Canvas 右鍵 → Create Empty → 命名「LevelButtonContainer」
   - Add Component → Vertical Layout Group
   - Spacing：20
   - Rect Transform：Width：400, Height：500, Pos Y：0

7. **建立 LevelButton Prefab：**（非常重要！）
   - Canvas 右鍵 → UI → Button → 命名「LevelButton」
   - 裡面的 Text 改為「關卡名稱」
   - Width：350, Height：80
   - **把這個 LevelButton 從 Hierarchy 拖曳到 Project 面板的 `Assets/Prefabs/` 資料夾**
   - 拖曳完後，Hierarchy 中的 LevelButton 可以刪除（因為已存成 Prefab）

8. **建立返回按鈕：**
   - Canvas 右鍵 → UI → Button → 命名「BackButton」
   - Text：「返回」
   - Pos Y：-400, Width：200, Height：50

9. **掛上控制器：**
   - 在 Canvas 上 Add Component → 「StageSelectController」
   - Inspector 中拖曳：
     - Level Button Container → `LevelButtonContainer`
     - Level Button Prefab → 從 Project 面板的 `Assets/Prefabs/LevelButton` 拖曳
     - Back Button → `BackButton`

10. Ctrl + S 儲存

---

### Step 7：建立 CardMatch 場景

1. File → New Scene → Basic (URP) → Create
2. File → Save As → `Assets/Scenes/CardMatch.unity`

3. **建立 Canvas：**
   - Hierarchy 右鍵 → UI → Canvas
   - Canvas Scaler → Scale With Screen Size → 1920 x 1080

4. **建立背景：**
   - Canvas 右鍵 → UI → Image → 「Background」
   - 撐滿畫面，顏色：淡黃色 (#FFF8DC)

5. **建立頂部資訊列：**
   - Canvas 右鍵 → UI → Text → 命名「PlayerNameText」
     - Text：「玩家名稱」, Font Size：24, Pos Y：450, Pos X：-300

   - Canvas 右鍵 → UI → Text → 命名「MoveCountText」
     - Text：「步數：0」, Font Size：24, Pos Y：450, Pos X：300

6. **建立卡片網格：**
   - Canvas 右鍵 → Create Empty → 命名「CardGrid」
   - Add Component → Grid Layout Group
   - 設定：
     - Cell Size：X=150, Y=200
     - Spacing：X=20, Y=20
     - Start Corner：Upper Left
     - Start Axis：Horizontal
     - Child Alignment：Middle Center
     - Constraint：Fixed Column Count → 3
   - Rect Transform：Width：500, Height：450, Pos Y：0

7. **建立 Card Prefab：**

   a. Canvas 右鍵 → UI → Button → 命名「Card」
   b. 設定按鈕：
      - Width：150, Height：200
      - Image 顏色改為紫色 (#CC99CC)

   c. 在 Card 裡面右鍵 → UI → Text → 命名「KanaText」
      - Text：「?」
      - Font Size：48
      - Alignment：置中
      - Color：白色
      - 撐滿按鈕（Anchor Presets → Stretch-Stretch）

   d. 在 Card 上 Add Component → 搜尋「CardView」
   e. Inspector 中：
      - Kana Text → 拖入 `KanaText`
      - Card Background → 拖入 Card 本身的 Image 元件

   f. **把 Card 從 Hierarchy 拖到 `Assets/Prefabs/` 資料夾**
   g. Hierarchy 中的 Card 刪除

8. **建立結算面板：**
   - Canvas 右鍵 → UI → Panel → 命名「ResultPanel」
   - Color：半透明黑色 (0,0,0,180)
   - 撐滿畫面

   在 ResultPanel 裡面：
   - 右鍵 → UI → Text → 命名「ResultText」
     - Font Size：32, Color：白色, Alignment：置中
     - Pos Y：50, Width：500, Height：200

   - 右鍵 → UI → Button → 命名「RetryButton」
     - Text：「再玩一次 / 換人」
     - Pos Y：-100, Width：250, Height：50

9. **建立返回按鈕：**
   - Canvas 右鍵 → UI → Button → 命名「BackButton」
   - Text：「返回」
   - Pos Y：-450, Width：200, Height：50

10. **掛上控制器：**
    - Canvas 上 Add Component → 「CardMatchController」
    - Inspector 拖曳：
      - Card Grid → `CardGrid`（的 GridLayoutGroup）
      - Card Prefab → 從 `Assets/Prefabs/Card` 拖入
      - Move Count Text → `MoveCountText`
      - Player Name Text → `PlayerNameText`
      - Result Panel → `ResultPanel`
      - Result Text → `ResultText`
      - Back Button → `BackButton`
      - Retry Button → `RetryButton`

11. Ctrl + S 儲存

---

## 第四部分：設定 Build Settings

### Step 8：加入場景到 Build Settings

1. 上方選單 → File → Build Settings
2. 點擊「Add Open Scenes」或直接從 Project 面板拖曳場景：
   - `Scenes/MainMenu` → 順序 0（第一個載入的場景）
   - `Scenes/StageSelect` → 順序 1
   - `Scenes/CardMatch` → 順序 2
3. Platform 確認選擇 **Windows, Mac, Linux**（或 WebGL）
4. 關閉 Build Settings

---

## 第五部分：執行測試

### Step 9：執行單元測試

1. 上方選單 → Window → General → Test Runner
2. 會開啟 Test Runner 視窗
3. 點擊「EditMode」標籤
4. 你應該看到所有測試：
   - DataTests（4 個）
   - SaveSystemTests（4 個）
   - KanaDatabaseTests（6 個）
   - GameManagerTests（6 個）
   - CardMatchLogicTests（7 個）
5. 點擊「Run All」
6. 全部應該顯示綠色 ✅

> **如果測試沒有出現：**
> - 確認 `Assets/Tests/EditMode/Tests.EditMode.asmdef` 檔案存在
> - 上方選單 → Assets → Reimport All
> - 等待 Unity 重新編譯

---

## 第六部分：試玩遊戲

### Step 10：開啟主選單場景並按 Play

1. 在 Project 面板中，雙擊 `Assets/Scenes/MainMenu.unity` 開啟場景
2. 確認 Hierarchy 中有 GameManager、AudioManager、UIManager
3. 點擊上方的 **▶ Play 按鈕**
4. 遊戲應該在 Game 視窗中啟動
5. 測試流程：
   - 點擊「單人遊戲」→ 顯示名稱輸入
   - 輸入名稱 → 點擊「開始遊戲」→ 進入關卡選擇
   - 選擇「入門」→ 進入卡牌翻翻看
   - 翻卡配對 → 完成後顯示結果

> **如果出現錯誤：**
> - 在 Console 面板（Window → General → Console）查看錯誤訊息
> - 常見問題：Inspector 中的引用欄位顯示「None」→ 需要重新拖曳連接

---

## 常見操作速查

| 操作 | 方法 |
|------|------|
| 建立空物件 | Hierarchy 右鍵 → Create Empty |
| 建立 UI 按鈕 | Hierarchy 右鍵 → UI → Button |
| 加入腳本 | Inspector → Add Component → 搜尋腳本名稱 |
| 儲存場景 | Ctrl + S |
| 播放遊戲 | 點擊上方 ▶ 按鈕（或 Ctrl + P） |
| 停止遊戲 | 再次點擊 ▶ 按鈕（或 Ctrl + P） |
| 建立 Prefab | 把 Hierarchy 的物件拖到 Project 的資料夾 |
| 執行測試 | Window → General → Test Runner |
| 查看錯誤 | Window → General → Console |
| 撐滿畫面 | Anchor Presets 中按住 Alt 點右下角 |

---

## 拖曳連接（Inspector 引用）重點說明

Unity 中最重要的概念之一是「拖曳連接」：

```
Inspector 面板中：
┌──────────────────────────────┐
│ MainMenuController (Script)  │
│                              │
│ Single Player Button: [None] │ ← 把 Hierarchy 中的
│                       ↑      │    SinglePlayerButton
│                 拖曳到這裡     │    拖到這個欄位
└──────────────────────────────┘
```

**步驟：**
1. 在 Hierarchy 中找到要連接的物件（例如 `SinglePlayerButton`）
2. 用滑鼠拖曳它
3. 放到 Inspector 中對應的欄位上
4. 欄位會從「None」變成物件名稱

**如果欄位不接受拖曳：** 確認物件類型正確（例如 Button 欄位只接受帶有 Button 元件的物件）

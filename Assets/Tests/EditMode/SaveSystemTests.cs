using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

public class SaveSystemTests
{
    private JsonSaveSystem saveSystem;
    private string testSavePath;

    [SetUp]
    public void SetUp()
    {
        testSavePath = Path.Combine(Path.GetTempPath(), "test_save.json");
        saveSystem = new JsonSaveSystem(testSavePath);

        // Ensure clean state
        if (File.Exists(testSavePath))
            File.Delete(testSavePath);
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
        // Arrange
        var player = new PlayerData
        {
            playerName = "\u30C6\u30B9\u30C8",
            currentLevel = 2
        };
        player.learnedKana.Add("\u3042");
        player.learnedKana.Add("\u3044");
        player.learnedKana.Add("\u3046");

        var saveData = new GameSaveData
        {
            players = new List<PlayerData> { player },
            lastPlayDate = "2026-02-14",
            totalPlayTimeSeconds = 120
        };

        // Act
        saveSystem.SaveGame(saveData);
        var loaded = saveSystem.LoadGame();

        // Assert
        Assert.IsNotNull(loaded);
        Assert.AreEqual(1, loaded.players.Count);
        Assert.AreEqual("\u30C6\u30B9\u30C8", loaded.players[0].playerName);
        Assert.AreEqual(2, loaded.players[0].currentLevel);
        Assert.AreEqual(3, loaded.players[0].learnedKana.Count);
        Assert.AreEqual("\u3042", loaded.players[0].learnedKana[0]);
        Assert.AreEqual("\u3044", loaded.players[0].learnedKana[1]);
        Assert.AreEqual("\u3046", loaded.players[0].learnedKana[2]);
        Assert.AreEqual("2026-02-14", loaded.lastPlayDate);
        Assert.AreEqual(120, loaded.totalPlayTimeSeconds);
    }

    [Test]
    public void DeleteSave_RemovesFile()
    {
        // Arrange
        var saveData = new GameSaveData
        {
            players = new List<PlayerData>(),
            lastPlayDate = "2026-02-14",
            totalPlayTimeSeconds = 0
        };
        saveSystem.SaveGame(saveData);
        Assert.IsTrue(saveSystem.HasSaveFile());

        // Act
        saveSystem.DeleteSave();

        // Assert
        Assert.IsFalse(saveSystem.HasSaveFile());
    }

    [Test]
    public void LoadGame_ReturnsNull_WhenNoFile()
    {
        var result = saveSystem.LoadGame();
        Assert.IsNull(result);
    }
}

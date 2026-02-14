using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class DataTests
{
    [Test]
    public void KanaData_SerializesToJsonAndBack()
    {
        // Arrange
        var kana = new KanaData
        {
            id = "a",
            hiragana = "a_hira",
            katakana = "a_kata",
            romaji = "a",
            origin = "origin_a",
            originMeaning = "meaning_a",
            audioFile = "a.mp3",
            strokeOrder = new List<int> { 1, 2, 3 },
            group = "vowel",
            level = 1
        };

        // Act
        string json = JsonUtility.ToJson(kana);
        var deserialized = JsonUtility.FromJson<KanaData>(json);

        // Assert
        Assert.AreEqual(kana.id, deserialized.id);
        Assert.AreEqual(kana.hiragana, deserialized.hiragana);
        Assert.AreEqual(kana.katakana, deserialized.katakana);
        Assert.AreEqual(kana.romaji, deserialized.romaji);
        Assert.AreEqual(kana.origin, deserialized.origin);
        Assert.AreEqual(kana.originMeaning, deserialized.originMeaning);
        Assert.AreEqual(kana.audioFile, deserialized.audioFile);
        Assert.AreEqual(kana.strokeOrder.Count, deserialized.strokeOrder.Count);
        Assert.AreEqual(kana.group, deserialized.group);
        Assert.AreEqual(kana.level, deserialized.level);
    }

    [Test]
    public void KanaData_DeserializesFromJsonWithJapaneseCharacters()
    {
        // Arrange
        string json = "{\"id\":\"a\",\"hiragana\":\"\u3042\",\"katakana\":\"\u30A2\",\"romaji\":\"a\",\"origin\":\"\u5B89\",\"originMeaning\":\"\u5E73\u5B89\",\"audioFile\":\"a.mp3\",\"strokeOrder\":[1,2,3],\"group\":\"vowel\",\"level\":1}";

        // Act
        var kana = JsonUtility.FromJson<KanaData>(json);

        // Assert
        Assert.AreEqual("a", kana.id);
        Assert.AreEqual("\u3042", kana.hiragana);
        Assert.AreEqual("\u30A2", kana.katakana);
        Assert.AreEqual("a", kana.romaji);
        Assert.AreEqual("\u5B89", kana.origin);
        Assert.AreEqual("\u5E73\u5B89", kana.originMeaning);
        Assert.AreEqual(3, kana.strokeOrder.Count);
        Assert.AreEqual("vowel", kana.group);
        Assert.AreEqual(1, kana.level);
    }

    [Test]
    public void GameSaveData_SupportsMultiplePlayers()
    {
        // Arrange
        var saveData = new GameSaveData
        {
            players = new List<PlayerData>(),
            lastPlayDate = "2026-02-14",
            totalPlayTimeSeconds = 3600
        };

        var player1 = new PlayerData { playerName = "Kitty1", currentLevel = 3 };
        player1.learnedKana.Add("a");
        player1.learnedKana.Add("i");

        var player2 = new PlayerData { playerName = "Kitty2", currentLevel = 1 };
        player2.learnedKana.Add("u");

        saveData.players.Add(player1);
        saveData.players.Add(player2);

        // Act
        string json = JsonUtility.ToJson(saveData);
        var deserialized = JsonUtility.FromJson<GameSaveData>(json);

        // Assert
        Assert.AreEqual(2, deserialized.players.Count);
        Assert.AreEqual("Kitty1", deserialized.players[0].playerName);
        Assert.AreEqual("Kitty2", deserialized.players[1].playerName);
        Assert.AreEqual(3, deserialized.players[0].currentLevel);
        Assert.AreEqual(1, deserialized.players[1].currentLevel);
        Assert.AreEqual(2, deserialized.players[0].learnedKana.Count);
        Assert.AreEqual(1, deserialized.players[1].learnedKana.Count);
    }

    [Test]
    public void KanaDatabaseWrapper_DeserializesListOfKana()
    {
        // Arrange
        string json = "{\"kanaList\":[" +
            "{\"id\":\"a\",\"hiragana\":\"\u3042\",\"katakana\":\"\u30A2\",\"romaji\":\"a\",\"origin\":\"\",\"originMeaning\":\"\",\"audioFile\":\"\",\"strokeOrder\":[],\"group\":\"vowel\",\"level\":1}," +
            "{\"id\":\"i\",\"hiragana\":\"\u3044\",\"katakana\":\"\u30A4\",\"romaji\":\"i\",\"origin\":\"\",\"originMeaning\":\"\",\"audioFile\":\"\",\"strokeOrder\":[],\"group\":\"vowel\",\"level\":1}," +
            "{\"id\":\"u\",\"hiragana\":\"\u3046\",\"katakana\":\"\u30A6\",\"romaji\":\"u\",\"origin\":\"\",\"originMeaning\":\"\",\"audioFile\":\"\",\"strokeOrder\":[],\"group\":\"vowel\",\"level\":1}" +
            "]}";

        // Act
        var database = JsonUtility.FromJson<KanaDatabaseWrapper>(json);

        // Assert
        Assert.IsNotNull(database);
        Assert.IsNotNull(database.kanaList);
        Assert.AreEqual(3, database.kanaList.Count);
        Assert.AreEqual("a", database.kanaList[0].id);
        Assert.AreEqual("\u3042", database.kanaList[0].hiragana);
        Assert.AreEqual("\u30A2", database.kanaList[0].katakana);
        Assert.AreEqual("i", database.kanaList[1].id);
        Assert.AreEqual("\u3044", database.kanaList[1].hiragana);
        Assert.AreEqual("u", database.kanaList[2].id);
        Assert.AreEqual("\u3046", database.kanaList[2].hiragana);
    }
}

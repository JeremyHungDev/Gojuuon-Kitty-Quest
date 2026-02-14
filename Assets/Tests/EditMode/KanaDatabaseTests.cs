using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class KanaDatabaseTests
{
    private KanaDatabaseService database;

    [SetUp]
    public void SetUp()
    {
        database = new KanaDatabaseService();
        var jsonFile = Resources.Load<TextAsset>("KanaDatabase");
        Assert.IsNotNull(jsonFile, "KanaDatabase.json not found in Resources");
        database.LoadFromJson(jsonFile.text);
    }

    [Test]
    public void GetKanaById_ReturnsCorrectKana()
    {
        var kana = database.GetKanaById("a");

        Assert.IsNotNull(kana);
        Assert.AreEqual("\u3042", kana.hiragana);
        Assert.AreEqual("\u30A2", kana.katakana);
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
        var group = database.GetKanaByGroup("a_row");

        Assert.AreEqual(5, group.Count);
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

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
            new KanaData { id = "a", hiragana = "\u3042", katakana = "\u30A2" },
            new KanaData { id = "i", hiragana = "\u3044", katakana = "\u30A4" },
            new KanaData { id = "u", hiragana = "\u3046", katakana = "\u30A6" }
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
        // Find a matching pair
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

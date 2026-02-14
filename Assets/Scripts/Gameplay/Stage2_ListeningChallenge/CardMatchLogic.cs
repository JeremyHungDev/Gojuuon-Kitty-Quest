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

        var selectedKana = new List<KanaData>();
        for (int i = 0; i < totalPairs && i < kanaList.Count; i++)
        {
            selectedKana.Add(kanaList[i]);
        }

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

        firstCard.isFaceUp = false;
        secondCard.isFaceUp = false;
        firstFlippedIndex = -1;

        return FlipResult.Mismatch;
    }

    public string GetMatchedKanaId()
    {
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

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

        int stars = CalculateStars(logic.MoveCount, logic.Cards.Count / 2);

        playerData.scores.Add(new MinigameScore
        {
            minigameId = "card_match",
            bestScore = logic.MoveCount,
            attempts = 1
        });
        gm.Save();

        string starDisplay = new string('\u2605', stars) + new string('\u2606', 3 - stars);
        resultText.text = playerData.playerName + "\n" + "Steps: " + logic.MoveCount + "\n" + starDisplay;
        resultPanel.SetActive(true);

        if (gm.PlayerManager.IsTwoPlayer && gm.PlayerManager.ActivePlayerIndex == 0)
        {
            resultText.text += "\n\nNext player's turn!";
        }
    }

    private int CalculateStars(int moves, int pairs)
    {
        if (moves <= pairs) return 3;
        if (moves <= pairs * 2) return 2;
        return 1;
    }

    private void UpdateUI()
    {
        moveCountText.text = "Steps: " + logic.MoveCount;
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
        var gm = GameManager.Instance;
        if (gm.PlayerManager.IsTwoPlayer)
        {
            gm.PlayerManager.SwitchPlayer();
        }

        if (UIManager.Instance != null)
            UIManager.Instance.LoadSceneWithFade("CardMatch");
        else
            gm.LoadScene("CardMatch");
    }
}

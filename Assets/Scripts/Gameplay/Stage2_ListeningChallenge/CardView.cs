using UnityEngine;
using UnityEngine.UI;
using System;

public class CardView : MonoBehaviour
{
    [SerializeField] private Text kanaText;
    [SerializeField] private Image cardBackground;
    [SerializeField] private Color faceDownColor = new Color(0.8f, 0.6f, 0.8f);
    [SerializeField] private Color faceUpColor = Color.white;
    [SerializeField] private Color matchedColor = new Color(0.7f, 1f, 0.7f);

    private Button button;
    private int cardIndex;
    private Action<int> onClickCallback;

    public void Initialize(int index, Action<int> onClick)
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

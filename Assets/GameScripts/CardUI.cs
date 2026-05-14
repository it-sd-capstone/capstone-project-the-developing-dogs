using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public TMP_Text cardNameText;
    public Image artworkImage;
    public Image backgroundImage;

    private PlayerCard card;
    private bool isSelected = false;

    private Color normalColor;
    public Color selectedColor = Color.yellow;

    private void Awake()
    {
        if (backgroundImage != null)
        {
            normalColor = backgroundImage.color;
        }
    }

    public void Setup(PlayerCard playerCard)
    {
        card = playerCard;

        if (card.City != null)
        {
            cardNameText.text = card.City.cityName;
        }
        else
        {
            cardNameText.text = card.GetType().Name;
        }
    }

    public void Setup(string cardName, Sprite sprite)
    {
        cardNameText.text = cardName;

        if (artworkImage != null)
        {
            artworkImage.sprite = sprite;
        }
    }

    public void OnCardClicked()
    {
        isSelected = !isSelected;

        if (backgroundImage != null)
        {
            backgroundImage.color = isSelected ? selectedColor : normalColor;
        }

        Debug.Log("Clicked card: " + cardNameText.text);
    }
}
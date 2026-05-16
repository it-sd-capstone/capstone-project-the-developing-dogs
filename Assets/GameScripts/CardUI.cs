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
    public Color selectedColor = Color.green;

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
            switch (card.City.diseaseColor)
            {
                case DiseaseColor.Red:
                    backgroundImage.color = Color.red;
                    normalColor = Color.red;
                    break;
                case DiseaseColor.Blue:
                    backgroundImage.color = Color.blue;
                    normalColor = Color.blue;
                    break;
                case DiseaseColor.Yellow:
                    backgroundImage.color = Color.yellow;
                    normalColor = Color.yellow;
                    break;
                default:
                    backgroundImage.color = Color.black;
                    normalColor = Color.black;
                    break;

            }
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
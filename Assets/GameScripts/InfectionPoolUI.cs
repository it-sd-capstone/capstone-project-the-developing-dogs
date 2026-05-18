using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfectionPoolUI : MonoBehaviour
{
    [Header("Infection Pool Texts")]
    public TextMeshProUGUI blueCount;
    public TextMeshProUGUI yellowCount;
    public TextMeshProUGUI blackCount;
    public TextMeshProUGUI redCount;

    [Header("Cure Pool Images")]
    public Image blueCure;
    public Image yellowCure;
    public Image blackCure;
    public Image redCure;

    private void Start()
    {
        // Start all cures as NOT cured (dimmed)
        SetCureStatus(DiseaseColor.Blue, false);
        SetCureStatus(DiseaseColor.Yellow, false);
        SetCureStatus(DiseaseColor.Black, false);
        SetCureStatus(DiseaseColor.Red, false);
    }

    public void UpdateInfectionCounts(int blue, int yellow, int black, int red)
    {
        blueCount.text = blue.ToString();
        yellowCount.text = yellow.ToString();
        blackCount.text = black.ToString();
        redCount.text = red.ToString();
    }

    public void SetCureStatus(DiseaseColor color, bool cured)
    {
        Image target = null;

        switch (color)
        {
            case DiseaseColor.Blue:
                target = blueCure;
                break;
            case DiseaseColor.Yellow:
                target = yellowCure;
                break;
            case DiseaseColor.Black:
                target = blackCure;
                break;
            case DiseaseColor.Red:
                target = redCure;
                break;
        }

        if (target != null)
        {
            Color c = target.color;
            c.a = cured ? 1f : 0.35f;   // bright when cured, dim when not
            target.color = c;
        }
    }
}

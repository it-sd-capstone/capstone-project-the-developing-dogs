using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityCard : MonoBehaviour
{
    [Header("Card elements")]
    [SerializeField] private TextMeshProUGUI cityName;
    [SerializeField] private Image station;
    [SerializeField] private TextMeshProUGUI red;
    [SerializeField] private TextMeshProUGUI blue;
    [SerializeField] private TextMeshProUGUI yellow;
    [SerializeField] private TextMeshProUGUI black;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OnClick(City city)
    {
        cityName.text = city.cityName;
        if(city.hasResearchStation) station.enabled = true;
        else station.enabled = false;

        red.text = city.GetDiseaseCount(DiseaseColor.Red).ToString();
        blue.text = city.GetDiseaseCount(DiseaseColor.Blue).ToString();
        yellow.text = city.GetDiseaseCount(DiseaseColor.Yellow).ToString();
        black.text = city.GetDiseaseCount(DiseaseColor.Black).ToString();

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

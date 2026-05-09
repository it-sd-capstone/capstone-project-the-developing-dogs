using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CityCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cityName;
    [SerializeField] private Image hasResearchStation;
    [SerializeField] private TextMeshProUGUI redCount;
    [SerializeField] private TextMeshProUGUI blueCount;
    [SerializeField] private TextMeshProUGUI yellowCount;
    [SerializeField] private TextMeshProUGUI blackCount;

    private City currentCity;
    private GameBoard gameBoard;

    public void Start()
    {
        gameBoard = FindAnyObjectByType<GameBoard>();
        if (gameBoard == null)
            Debug.LogError("GameBoard not found!");

        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0)) gameObject.SetActive(false);
    }

    public void ShowCityCard(City city)
    {
        currentCity = city;
        UpdateCard();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentCity = null;
    }

    public void UpdateCard()
    {
        if (currentCity == null) return;

        cityName.text = currentCity.cityName;

        redCount.text = currentCity.GetDiseaseCount(DiseaseColor.Red).ToString();
        blueCount.text = currentCity.GetDiseaseCount(DiseaseColor.Blue).ToString();
        yellowCount.text = currentCity.GetDiseaseCount(DiseaseColor.Yellow).ToString();
        blackCount.text = currentCity.GetDiseaseCount(DiseaseColor.Black).ToString();

        if(currentCity.hasResearchStation) hasResearchStation.enabled = true;
        else hasResearchStation.enabled = false;
    }

    public void RefreshIfActive(City updatedCity)
    {
        if (currentCity == updatedCity && gameObject.activeSelf)
            UpdateCard();
    }
}

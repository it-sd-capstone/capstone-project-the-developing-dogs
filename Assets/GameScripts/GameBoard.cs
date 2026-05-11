using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameBoard : MonoBehaviour
{
    public List<City> cities = new List<City>();
    public Dictionary<string, City> cityLookup = new Dictionary<string, City>();
    public Dictionary<DiseaseColor, int> cubePool = new Dictionary<DiseaseColor, int>();
    public Dictionary<DiseaseColor, bool> curePool = new Dictionary<DiseaseColor, bool>();

    public int outbreakCounter = 0;
    public int maxOutbreaks = 8;
    public int researchStationCount = 6;

    //For Infection Pool UI
    public TextMeshProUGUI blueText;
    public TextMeshProUGUI yellowText;
    public TextMeshProUGUI blackText;
    public TextMeshProUGUI redText;

    [Header("City database")]
    [SerializeField] public CityDB citiesDB;

    [Header("Board visuals")]
    [SerializeField] private Image background;
    [SerializeField] private RectTransform boardContainer;
    [SerializeField] private GameObject cityMarkerPrefab;

    private CityCard cc;
    private GameManager gm;
    private PlayerAction pa;

    private Dictionary<City, GameObject> cityMarkers = new Dictionary<City, GameObject>();
    private Dictionary<Player, GameObject> playerMarkers = new Dictionary<Player, GameObject>();

    private void Awake()
    {
        gm = FindAnyObjectByType<GameManager>();
        pa = FindAnyObjectByType<PlayerAction>();
        cc = FindAnyObjectByType<CityCard>();

        foreach (DiseaseColor color in Enum.GetValues(typeof(DiseaseColor)))
        {
            cubePool[color] = 24;
            curePool[color] = false;
        }
        GenerateCities();
        DrawCities();
    }

    private void GenerateCities()
    {
        foreach(CityData data in citiesDB.cities)
        {
            City city = new City();
            city.Init(data);
            cities.Add(city);
            if (city.cityName == "Atlanta")
            {
                city.hasResearchStation = true;
                researchStationCount--;
            }
        }

        BuildCityLookup();

        foreach(CityData cityData in citiesDB.cities)
        {
            City city = cityLookup[cityData.cityName];
            foreach (var neighbor in cityData.neighbors)
            {
                if (cityLookup.TryGetValue(neighbor.cityName, out City n))
                {
                    city.neighbors.Add(cityLookup[n.cityName]);
                }
            }
        }
    }

    private void BuildCityLookup()
    {
        cityLookup.Clear();
        foreach (var city in cities)
        {
            cityLookup[city.cityName] = city;
        }
    }

    private void DrawCities()
    {
        foreach (CityData data in citiesDB.cities)
        {
            City city = cityLookup[data.cityName];
            GameObject marker = Instantiate(cityMarkerPrefab, boardContainer);
            RectTransform rect = marker.GetComponent<RectTransform>();
            rect.anchoredPosition = data.position;

            // Set colour based on the city's disease type
            Image img = marker.GetComponent<Image>();
            if (img != null)
                img.color = GetDisplayColor(data.diseaseColor);

            cityMarkers[city] = marker;

            Button btn = marker.GetComponent<Button>();
            btn.onClick.AddListener(() => OnCityClick(city));
        }
    }

    public void InfectionPhase(List<City> infections)
    {
        foreach(var city in infections)
        {
            InfectCity(city, city.diseaseColor);
        }
    }

    public void InfectCity(City city, DiseaseColor disease, HashSet<City> outbreakChain = null)
    {
        if(IsEradicated(disease)) return;

        if (cubePool[disease] <= 0)
        {
            Debug.Log("No more disease cubes. You lose.");
            return; //TODO: put game end here instead of return.
        }

        int currentCubes = city.GetDiseaseCount(disease);

        if (currentCubes >= 3)
        {
            TriggerOutbreak(city, disease, outbreakChain);
        } else
        {
            city.addCube(disease);
            cubePool[disease]--;
        }

    }

    private void TriggerOutbreak(City city, DiseaseColor groundZeroColor, HashSet<City> outbreakChain)
    {
        if(outbreakChain == null) outbreakChain = new HashSet<City>();

        if(outbreakChain.Contains(city)) return;
        outbreakChain.Add(city);

        outbreakCounter++;
        // OnOutbreak?.Invoke();
        
        if(outbreakCounter >= maxOutbreaks)
        {
            Debug.Log("Too many outbreaks. You lose.");
            //loss code here
            return;
        }

        foreach(var neighbor in city.neighbors)
            InfectCity(neighbor, groundZeroColor, outbreakChain);
    }

    public void RemoveDisease(City city, DiseaseColor color)
    {
        if(city.GetDiseaseCount(color) > 0)
        {
            city.removeCube(color);
            cubePool[color]++;
        }
        UpdateCount();
    }

    public void CureDisease(DiseaseColor color)
    {
        if(curePool[color]) return;
        curePool[color] = true;

        UpdateCount();
    }

    public bool canMove(City from, City to) => from.IsConnectedTo(to);
     
    public bool IsEradicated(DiseaseColor color)
    {
        if(!curePool[color]) return false;
        int totalCubes = cities.Sum(c => c.GetDiseaseCount(color));
        return totalCubes == 0;
    }

    public void BuildResearchStation(City city)
    {
        if (city.hasResearchStation || researchStationCount < 0) return;
        city.BuildResearchStation();
        researchStationCount -= 1;
    }

    public bool CheckWin()
    { 
        foreach(DiseaseColor color in Enum.GetValues(typeof(DiseaseColor)))
        {
            if(!curePool[color]) return false;
        }

        return true;
    }

    private void UpdateCount()
    {
        if (blueText != null)
        {
            blueText.text = cubePool[DiseaseColor.Blue].ToString();
            yellowText.text = cubePool[DiseaseColor.Yellow].ToString();
            blackText.text = cubePool[DiseaseColor.Black].ToString();
            redText.text = cubePool[DiseaseColor.Red].ToString();
        }
    }

    private Color GetDisplayColor(DiseaseColor disease)
    {
        switch (disease)
        {
            case DiseaseColor.Red: return Color.red;
            case DiseaseColor.Blue: return Color.blue;
            case DiseaseColor.Yellow: return Color.yellow;
            case DiseaseColor.Black: return Color.black;
            default: return Color.gray;
        }
    }

    private void OnCityClick(City city)
    {
        // show city card
        cc.OnClick(city);
    
        // Handle action if any
        if (pa != null)
        {
            pa.OnCitySelected(city);
        }
    }

    public void Highlight(City city)
    {
        var outline = cityMarkers[city].GetComponent<Outline>();
        outline.enabled = !outline.enabled;
    }

    // Method to clear highlight from a specific city
    public void ClearHighlight(City city)
    {
        if (cityMarkers.TryGetValue(city, out GameObject marker))
        {
            Outline outline = marker.GetComponent<Outline>();
            if (outline != null)
                outline.enabled = false;
        }
    }

    // Method to clear all highlights
    public void ClearAllHighlights()
    {
        foreach (var marker in cityMarkers.Values)
        {
            Outline outline = marker.GetComponent<Outline>();
            if (outline != null)
                outline.enabled = false;
        }
    }

    // Update player position visual
    public void UpdatePlayerPosition(Player player)
    {
        // If you have visual markers for players, update their positions here
        if (playerMarkers.TryGetValue(player, out GameObject marker))
        {
            // Move the marker to the new city's position
            if (cityMarkers.TryGetValue(player.CurrentCity, out GameObject cityMarker))
            {
                marker.transform.position = cityMarker.transform.position;
            }
        }
    }

    // Update research station visual
    public void UpdateResearchStationVisual(City city)
    {
        if (cityMarkers.TryGetValue(city, out GameObject marker))
        {
            // Enable research station visual on the city marker
            Image stationImage = marker.transform.Find("StationIcon")?.GetComponent<Image>();
            if (stationImage != null)
                stationImage.enabled = true;
        }
    }

    // Check if a disease is cured
    public bool IsCured(DiseaseColor color)
    {
        return curePool.ContainsKey(color) && curePool[color];
    }
}

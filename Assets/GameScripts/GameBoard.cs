using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("City database")]
    [SerializeField] public CityDB citiesDB;

    [Header("Board visuals")]
    [SerializeField] private Image background;
    [SerializeField] public RectTransform boardContainer;
    [SerializeField] private GameObject cityMarkerPrefab;
    private Image hasDisease;

    private CityCard cc;
    private GameManager gm;
    public PlayerAction pa;

    private Dictionary<City, GameObject> cityMarkers = new Dictionary<City, GameObject>();
    public Dictionary<Player, GameObject> playerMarkers = new Dictionary<Player, GameObject>();
    private Dictionary<City, Player[]> citySlots = new Dictionary<City, Player[]>();

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
        foreach (CityData data in citiesDB.cities)
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

        foreach (CityData cityData in citiesDB.cities)
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
        foreach (var city in infections)
        {
            InfectCity(city, city.diseaseColor);
        }
    }

    public void InfectCity(City city, DiseaseColor disease, HashSet<City> outbreakChain = null)
    {
        if (IsEradicated(disease)) return;

        foreach (Player p in gm.players)
        {
            if (p.Role is QuarantineSpecialistRole qs)
            {
                if (qs.PreventsInfectionIn(city))
                {
                    return;
                }
            }
        }

        if (cubePool[disease] <= 0)
        {
            Debug.Log("No more disease cubes. You lose.");
            gm.LoseGame("You let the disease run too rampant");
            return;
        }

        int currentCubes = city.GetDiseaseCount(disease);

        if (currentCubes >= 3)
        {
            TriggerOutbreak(city, disease, outbreakChain);
        }
        else
        {
            city.addCube(disease);
            cubePool[disease]--;
        }
        cc.UpdateCC(city);
        UpdateCount();
    }

    private void TriggerOutbreak(City city, DiseaseColor groundZeroColor, HashSet<City> outbreakChain)
    {
        if (outbreakChain == null) outbreakChain = new HashSet<City>();

        if (outbreakChain.Contains(city)) return;
        outbreakChain.Add(city);

        outbreakCounter++;

        if (outbreakCounter >= maxOutbreaks)
        {
            Debug.Log("Too many outbreaks. You lose.");
            gm.LoseGame("You suffered too many outbreaks");
            return;
        }

        foreach (var neighbor in city.neighbors)
            InfectCity(neighbor, groundZeroColor, outbreakChain);
    }

    public void RemoveDisease(City city, DiseaseColor color)
    {
        if (city.GetDiseaseCount(color) > 0)
        {
            city.removeCube(color);
            cubePool[color]++;
        }
        cc.UpdateCC(city);
        UpdateCount();
    }

    public void CureDisease(DiseaseColor color)
    {
        if (curePool[color]) return;
        curePool[color] = true;
        InfectionPoolUI pool = FindAnyObjectByType<InfectionPoolUI>();
        pool.SetCureStatus(color, true);

        UpdateCount();
        CheckWin();
    }

    public bool canMove(City from, City to) => from.IsConnectedTo(to);

    public bool IsEradicated(DiseaseColor color)
    {
        if (!curePool[color]) return false;
        int totalCubes = cities.Sum(c => c.GetDiseaseCount(color));
        return totalCubes == 0;
    }

    public void BuildResearchStation(City city)
    {
        if (city.hasResearchStation || researchStationCount < 0) return;
        city.BuildResearchStation();
        researchStationCount -= 1;
    }

    public void CheckWin()
    {
        if (curePool[DiseaseColor.Red] &&
            curePool[DiseaseColor.Blue] &&
            curePool[DiseaseColor.Yellow] &&
            curePool[DiseaseColor.Black])
            gm.WinGame();
    }

    private void UpdateCount()
    {
        if (gm != null && gm.infectionPoolUI != null)
        {
            gm.infectionPoolUI.UpdateInfectionCounts(
                cubePool[DiseaseColor.Blue],
                cubePool[DiseaseColor.Yellow],
                cubePool[DiseaseColor.Black],
                cubePool[DiseaseColor.Red]
            );
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
        cc.OnClick(city);

        if (pa != null)
        {
            pa.OnCitySelected(city);
        }
    }

    public void Highlight(City city)
    {
        if (cityMarkers.TryGetValue(city, out GameObject marker))
        {
            Outline outline = marker.GetComponent<Outline>();
            if (outline != null)
            {
                outline.effectColor = Color.cyan;
                outline.effectDistance = new Vector2(6f, 6f);
                outline.enabled = true;
            }
        }
    }

    public void ClearHighlight(City city)
    {
        if (cityMarkers.TryGetValue(city, out GameObject marker))
        {
            Outline outline = marker.GetComponent<Outline>();
            if (outline != null)
                outline.enabled = false;
        }
    }

    public void ClearAllHighlights()
    {
        foreach (var marker in cityMarkers.Values)
        {
            Outline outline = marker.GetComponent<Outline>();
            if (outline != null)
                outline.enabled = false;
        }
    }

    public void UpdatePlayerPosition(Player player)
    {
        if (!playerMarkers.TryGetValue(player, out GameObject marker))
            return;

        if (!cityMarkers.TryGetValue(player.CurrentCity, out GameObject cityMarker))
            return;

        if (!citySlots.ContainsKey(player.CurrentCity))
            citySlots[player.CurrentCity] = new Player[4];

        Player[] slots = citySlots[player.CurrentCity];

        foreach (var kvp in citySlots)
        {
            for (int i = 0; i < 4; i++)
            {
                if (kvp.Value[i] == player)
                    kvp.Value[i] = null;
            }
        }

        int assignedSlot = -1;
        for (int i = 0; i < 4; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = player;
                assignedSlot = i;
                break;
            }
        }

        if (assignedSlot == -1)
            assignedSlot = 0;

        Vector3[] offsets = new Vector3[]
        {
            new Vector3(0, -15, 0),
            new Vector3(-15, 0, 0),
            new Vector3(15, 0, 0),
            new Vector3(0, 15, 0),
        };

        Vector3 basePos = cityMarker.transform.position;
        marker.transform.position = basePos + offsets[assignedSlot];

        int cityIndex = cityMarker.transform.GetSiblingIndex();
        int baseIndex = cityIndex + 1;

        int[] order = new int[] { 3, 2, 1, 0 };

        int currentOffset = 0;
        foreach (int slot in order)
        {
            Player p = slots[slot];
            if (p == null) continue;

            if (!playerMarkers.TryGetValue(p, out GameObject pawnGO))
                continue;

            pawnGO.transform.SetSiblingIndex(baseIndex + currentOffset);
            currentOffset++;
        }
    }


    // Update research station visual
    public void UpdateResearchStationVisual(City city)
    {
        if (cityMarkers.TryGetValue(city, out GameObject marker))
        {
            Image stationImage = marker.transform.Find("StationIcon")?.GetComponent<Image>();
            if (stationImage != null)
                stationImage.enabled = true;
        }
    }

    public bool IsCured(DiseaseColor color)
    {
        return curePool.ContainsKey(color) && curePool[color];
    }

    public void ShowDiseasedCities()
    {
        foreach (City city in cities)
        {
            hasDisease = cityMarkers[city].transform.Find("DiseaseHere")?.GetComponent<Image>();
            if (city.GetDiseaseCount(DiseaseColor.Red) > 0 ||
                city.GetDiseaseCount(DiseaseColor.Blue) > 0 ||
                city.GetDiseaseCount(DiseaseColor.Yellow) > 0 ||
                city.GetDiseaseCount(DiseaseColor.Black) > 0)
            {
                if (hasDisease != null) hasDisease.enabled = true;
            }
            else if (hasDisease != null)
            {
                hasDisease.enabled = false;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public System.Action<City> OnCityInfected;
    public System.Action<City> OnOutbreak;
    public System.Action<City> OnResearchStationBuilt;

    public List<City> cities;
    public Dictionary<string, City> cityLookup;
    private HashSet<City> outbreaked;
    public Dictionary<DiseaseColor, int> cubePool;
    public Dictionary<DiseaseColor, bool> curePool;
    public Dictionary<Color, DiseaseColor> colorDict;

    public int outbreakCounter = 0;
    public int maxOutbreaks = 8;
    public int researchStationCount = 6;

    private GameObject cityPrefab;
    [SerializeField] private Transform citiesParent;

    private void Awake()
    {
        colorDict[Color.red] = DiseaseColor.Red;
        colorDict[Color.blue] = DiseaseColor.Blue;
        colorDict[Color.yellow] = DiseaseColor.Yellow;
        colorDict[Color.black] = DiseaseColor.Black;

        foreach (DiseaseColor color in Enum.GetValues(typeof(DiseaseColor)))
        {
            cubePool[color] = 24;
            curePool[color] = false;
        }
    }

    private void GenerateCities(CityData[] cityDatas)
    {
        foreach(CityData data in cityDatas)
        {
            GameObject obj = Instantiate(cityPrefab, data.position, quaternion.identity, transform);

            City city = obj.GetComponent<City>();

            city.Init(data);

            cities.Add(city);
        }

        BuildCityLookup();

        foreach(CityData cityData in cityDatas)
        {
            City city = cityLookup[cityData.cityName];
            foreach (var neighbor in cityData.neighbors)
            {
                if (cityLookup.ContainsKey(neighbor.cityName))
                {
                    city.neighbors.Add(cityLookup[neighbor.cityName]);
                }
            }
        }
    }

    private void BuildCityLookup()
    {
        cityLookup = new Dictionary<string, City>();
        foreach (var city in cities)
        {
            cityLookup[city.cityName] = city;
        }
    }

    public void InfectionPhase(List<City> infections)
    {
        foreach(var city in infections)
        {
            InfectCity(city, colorDict[city.diseaseColor]);
        }
    }

    public void InfectCity(City city, DiseaseColor disease)
    {
        if (cubePool[disease] <= 0)
        {
            Debug.Log("Disease is already eradicated");
            return; //TODO: put game end here instead of return.
        }

        if(IsEradicated(disease)) return;

        if (city.infectionLevels.Count > 3)
        {
            TriggerOutbreak(city, disease);
        } else
        {
            city.addCube(disease);
            cubePool[disease]--;
            city.infectionLevels[disease] = city.infectionLevels.Count;
        }

    }

    public void TriggerOutbreak(City city, DiseaseColor groundZeroColor)
    {
        if(outbreaked.Contains(city)) return;
        outbreakCounter++;
        
        if(outbreakCounter >= maxOutbreaks)
        {
            // end game logic
        }

        outbreaked.Add(city);
        foreach(var neigbor in city.neighbors)
        {
            InfectCity(neigbor, groundZeroColor);
        }

        OnOutbreak(city);
        outbreaked.Clear();
    }

    public void RemoveDisease(City city, DiseaseColor color)
    {
        if(city.GetDiseaseCount(color) == 0)
        {
            Debug.Log("Cannot remove any more cubes.");
            return;
        }
    }

    public bool canMove(City from, City to)
    {
        return from.neighbors.Contains(to);
    }
    
    public bool IsEradicated(DiseaseColor color)
    {
        if (cubePool[color] >= 24 && curePool[color])
        {
            Debug.Log(color + "disease is already eradicated");
            return true;
        }
        return false;
    }

    public void BuildResearchStation(City city)
    {
        if (city.hasResearchStation || researchStationCount < 1) return;
        city.BuildResearchStation();
        researchStationCount -= 1;
    }

    
}

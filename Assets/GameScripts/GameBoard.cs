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

    public List<City> cities = new List<City>();
    public Dictionary<string, City> cityLookup = new Dictionary<string, City>();
    public Dictionary<DiseaseColor, int> cubePool = new Dictionary<DiseaseColor, int>();
    public Dictionary<DiseaseColor, bool> curePool = new Dictionary<DiseaseColor, bool>();

    public int outbreakCounter = 0;
    public int maxOutbreaks = 8;
    public int researchStationCount = 6;
        
    [Header("City database")]
    [SerializeField] private CityDB citiesDB;

    private void Awake()
    {
        foreach (DiseaseColor color in Enum.GetValues(typeof(DiseaseColor)))
        {
            cubePool[color] = 24;
            curePool[color] = false;
        }
        GenerateCities();
    }

    private void GenerateCities()
    {
        foreach(CityData data in citiesDB.cities)
        {
            City city = new City();
            city.Init(data);
            cities.Add(city);
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
            OnCityInfected?.Invoke(city);
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
    }

    public void CureDisease(DiseaseColor color)
    {
        if(curePool[color]) return;
        curePool[color] = true;
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
        if (city.hasResearchStation || researchStationCount <= 1) return;
        city.BuildResearchStation();
        researchStationCount -= 1;
        OnResearchStationBuilt?.Invoke(city);
    }

    public bool CheckWin()
    { 
        foreach(DiseaseColor color in Enum.GetValues(typeof(DiseaseColor)))
            if(!curePool[color])
                if(!curePool[color]) return false;
            return true;
    }
}

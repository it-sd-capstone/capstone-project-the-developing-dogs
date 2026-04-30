using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public System.Action<City> OnCityInfected;
    public System.Action<City> OnOutbreak;
    public System.Action<City> OnResearchStationBuilt;

    public List<City> cities;
    public Dictionary<string, City> cityLookup;
    private HashSet<City> outbreaked;
    public Dictionary<Color, int> cubePool;

    public int outbreakCounter = 0;
    public int maxOutbreaks = 8;

    public int researchStationCount = 6;

    public void init()
    {
        outbreakCounter = 0;

        cubePool = new Dictionary<Color, int>()
        {
            { Color.blue, 24},
            { Color.yellow, 24},
            { Color.black, 24},
            { Color.red, 24}
        };

        cityLookup = new Dictionary<string, City>();
        foreach (var city in cities)
        {
            city.infectionLevels = new Dictionary<Color, int>()
            {
                {Color.blue, 0},
                { Color.yellow, 0},
                { Color.black, 0},
                { Color.red, 0}
            };

            if (city.neighbors == null)
            {
                city.neighbors = new List<City>();

                cityLookup[city.cityName] = city;
            }

            if (city.cityName == "Atlanta")
            {
                city.hasResearchStation = true;
            }
        }

        // TODO: integrate initial infections code

    }

    public bool canMove(City from, City to)
    {
        return from.neighbors.Contains(to);
    }
    
    public void InfectionPhase(List<City> infections)
    {
        foreach(var city in infections)
        {
            InfectCity(city, city.diseaseColor);
        }
    }

    public void InfectCity(City city, Color disease)
    {
        city.infectionLevels.Add(disease, city.infectionLevels.Count);

        if (city.infectionLevels.Count > 3)
        {
            
            TriggerOutbreak(city, disease);
        }
    }

    public void TriggerOutbreak(City city, Color groundZeroColor)
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

    public void BuildResearchStation(City city)
    {
        if (city.hasResearchStation) return;

    }
}

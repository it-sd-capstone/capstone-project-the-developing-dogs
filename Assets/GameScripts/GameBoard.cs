using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public List<City> cities;
    public System.Action<City> OnCityInfected;
    public System.Action<City> OnOutbreak;
    public System.Action<City> OnResearchStationBuilt;
    
    public Dictionary<string, City> cityLookup;

    public int outbreakCounter = 0;
    public int maxOutbreaks = 8;

    private HashSet<City> outbreaked;

    

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

    public void InfectCity(City city, Color groundZeroColor)
    {
        city.infectionLevels.Add(groundZeroColor, city.infectionLevels.Count);

        if (city.infectionLevels.Count > 3)
        {
            
            TriggerOutbreak(city, groundZeroColor);
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
}

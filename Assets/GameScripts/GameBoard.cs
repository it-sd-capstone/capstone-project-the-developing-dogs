using System.Collections.Generic;
using UnityEngine;

// Controls the main board-related logic such as movement, infection, outbreaks, and research stations.
public class GameBoard : MonoBehaviour
{
    // Keeps track of which cities currently have research stations.
    public List<City> researchStations = new List<City>();

    // Events that other scripts can listen to when something important happens on the board.
    public System.Action<City> OnCityInfected;
    public System.Action<City> OnOutbreak;
    public System.Action<City> OnResearchStationBuilt;

    // Lookup table for finding cities by name.
    public Dictionary<string, City> cityLookup = new Dictionary<string, City>();

    // Tracks the number of outbreaks before the game ends.
    public int outbreakCounter = 0;
    public int maxOutbreaks = 8;

    // Used during an outbreak chain so the same city does not outbreak more than once.
    private HashSet<City> outbreaked = new HashSet<City>();

    // Builds a research station in the selected city if it does not already have one.
    public void BuildResearchStation(City city)
    {
        if (city == null)
            return;

        if (!researchStations.Contains(city))
        {
            researchStations.Add(city);
            city.hasResearchStation = true;
            OnResearchStationBuilt?.Invoke(city);
        }
    }

    // Checks if the player can move from one city to another based on neighbors.
    public bool canMove(City from, City to)
    {
        if (from == null || to == null || from.neighbors == null)
            return false;

        return from.neighbors.Contains(to);
    }

    // Infects each city in the list during the infection phase.
    public void InfectionPhase(List<City> infections)
    {
        if (infections == null)
            return;

        foreach (var city in infections)
        {
            if (city != null)
            {
                InfectCity(city, city.diseaseType);
            }
        }
    }

    // Adds a disease cube to a city and checks if an outbreak should happen.
    public void InfectCity(City city, DiseaseColor diseaseColor)
    {
        if (city == null)
            return;

        city.AddCube(diseaseColor);
        OnCityInfected?.Invoke(city);

        // If a city has more than 3 cubes, an outbreak is triggered.
        if (city.GetCubeCount(diseaseColor) > 3)
        {
            TriggerOutbreak(city, diseaseColor);
        }
    }

    // Handles outbreak logic and spreads infection to neighboring cities.
    public void TriggerOutbreak(City city, DiseaseColor diseaseColor)
    {
        if (city == null)
            return;

        // Prevents the same city from outbreaking multiple times in one chain reaction.
        if (outbreaked.Contains(city))
            return;

        outbreakCounter++;

        if (outbreakCounter >= maxOutbreaks)
        {
            // End game logic can be added here later.
        }

        outbreaked.Add(city);

        foreach (var neighbor in city.neighbors)
        {
            InfectCity(neighbor, diseaseColor);
        }

        OnOutbreak?.Invoke(city);
        outbreaked.Clear();
    }
}

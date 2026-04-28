using System.Collections.Generic;
using UnityEngine;

// This class stores information for one city on the game board.
// It keeps track of the city name, disease cubes, neighbors, and research station status.
[System.Serializable]
public class City
{
    // Name of the city. This can be used later for UI, cards, or lookup.
    public string cityName { get; set; }

    // This is mainly for visual color in Unity, not the disease enum logic.
    public Color diseaseColor;

    // Cities that are directly connected to this city.
    public List<City> neighbors = new List<City>();

    // Tracks whether this city has a research station built on it.
    public bool hasResearchStation = false;

    // Stores how many cubes of each disease color are currently in this city.
    private Dictionary<DiseaseColor, int> diseaseCubes = new Dictionary<DiseaseColor, int>();

    // Returns how many cubes of a specific color are in this city.
    public int GetCubeCount(DiseaseColor color)
    {
        // If the city does not have this color yet, treat it as 0 cubes.
        if (!diseaseCubes.ContainsKey(color))
            return 0;

        return diseaseCubes[color];
    }

    // Removes disease cubes from the city without letting the count go below 0.
    public void RemoveCubes(DiseaseColor color, int amount)
    {
        // If this city does not have this disease color, there is nothing to remove.
        if (!diseaseCubes.ContainsKey(color))
            return;

        diseaseCubes[color] -= amount;

        // Prevent negative cube counts.
        if (diseaseCubes[color] < 0)
            diseaseCubes[color] = 0;
    }

    // Checks if another city is directly connected to this city.
    public bool IsConnectedTo(City other)
    {
        return neighbors != null && neighbors.Contains(other);
    }

    // The game disease type for this city, such as red, blue, yellow, or black.
    public DiseaseColor diseaseType;

    // Adds one disease cube of the chosen color to this city.
    public void AddCube(DiseaseColor color)
    {
        // If this color is not in the dictionary yet, start it at 0 first.
        if (!diseaseCubes.ContainsKey(color))
            diseaseCubes[color] = 0;

        diseaseCubes[color]++;
    }
}

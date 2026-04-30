using System.Collections.Generic;
using Unity;
using UnityEngine;

[System.Serializable]
public class City
{
    public string cityName;
    public Color diseaseColor;
    public List<City> neighbors;
    
    public Dictionary<Color, int> infectionLevels;
    public bool hasResearchStation;
}
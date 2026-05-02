using System.Collections.Generic;
using Unity;
using UnityEngine;

[System.Serializable]
public class City
{
    public string cityName {get; private set;}
    public Color diseaseColor {get; private set;}
    public List<City> neighbors {get; private set;}
    
    public Dictionary<DiseaseColor, int> infectionLevels;
    public bool hasResearchStation;

    public void Init(CityData data)
    {
        cityName = data.cityName;
        diseaseColor = data.color;
        
        foreach(DiseaseColor color in System.Enum.GetValues(typeof(DiseaseColor)))
        {
            infectionLevels[color] = 0;
        }
    }

    public bool IsConnectedTo(City other) => neighbors.Contains(other);

    public void addCube(DiseaseColor color)
    {
        infectionLevels[color] += 1;
    }

    public void removeCube(DiseaseColor color)
    {
        infectionLevels[color] -= 1;
    }

    public int GetDiseaseCount(DiseaseColor color) => infectionLevels[color];
    
    public void CureCity(DiseaseColor color)
    {
        infectionLevels[color] = 0;
    }

    public void BuildResearchStation()
    {
        hasResearchStation = true;
    }
}
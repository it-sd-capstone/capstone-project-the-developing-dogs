using System.Collections.Generic;
using Unity;
using UnityEngine;

[System.Serializable]
public class City
{
    public string cityName {get; private set;}
    public DiseaseColor diseaseColor {get; private set;}
    public List<City> neighbors {get; private set;}
    
    public Dictionary<DiseaseColor, int> infectionLevels;
    public bool hasResearchStation;

    public void Init(CityData data)
    {
        cityName = data.cityName;
        diseaseColor = data.diseaseColor;
        neighbors = new List<City>();
        infectionLevels = new Dictionary<DiseaseColor, int>();
        
        foreach(DiseaseColor color in System.Enum.GetValues(typeof(DiseaseColor)))
        {
            infectionLevels[color] = 0;
        }
    }

    public bool IsConnectedTo(City other) => neighbors.Contains(other);

    public void addCube(DiseaseColor color)
    {
        if(infectionLevels[color] <3) infectionLevels[color]++;
    }

    public void removeCube(DiseaseColor color)
    {
        if(infectionLevels[color] <0) infectionLevels[color]--;
    }

    public void RemoveAllCubes(DiseaseColor color)
    {
        infectionLevels[color] = 0;
    }

    public int GetDiseaseCount(DiseaseColor color)
    {
        return infectionLevels[color];
    }

    public void BuildResearchStation()
    {
        hasResearchStation = true;
    }
}
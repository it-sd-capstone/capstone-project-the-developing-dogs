using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CityData", menuName = "Scriptable Objects/CityData")]
public class CityData : ScriptableObject
{
    // Display name for the city.
    public String cityName;

    // Game color from diseasecolor enum
    public DiseaseColor diseaseColor;

    // Visual color used for this city in Unity.
    public Color color;

    // Cities connected to this city.
    public List<CityData> neighbors;

    // Position where this city should appear on the board.
    public Vector2 position;
}

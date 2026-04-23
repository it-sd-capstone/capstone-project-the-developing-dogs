using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CityData", menuName = "Scriptable Objects/CityData")]
public class CityData : ScriptableObject
{
    public String cityName;
    public Color color;
    public List<CityData> neighbors;
    public Vector2 position;
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CityDB", menuName = "Scriptable Objects/CityDB")]
public class CityDB : ScriptableObject
{
    public List<CityData> cities;
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CityDB", menuName = "Scriptable Objects/CityDB")]
public class CityDB : ScriptableObject
{
    // List of all cities that can be used to build the game board.
    public List<CityData> cities;
}

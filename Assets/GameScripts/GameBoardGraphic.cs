using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameBoardGraphics : MonoBehaviour
{
    [Header("Board visuals")]
    [SerializeField] private Image background;
    [SerializeField] private RectTransform boardContainer;

    [Header("City config")]
    [SerializeField] private GameObject cityMarkerPrefab;
    [SerializeField] private GameObject connectionLinePrefab;

    [Header("City positions")]
    [SerializeField] private CityDB citiesDB;

    private GameBoard gameBoard;       // reference to the logic board
    private Dictionary<City, GameObject> cityMarkers = new Dictionary<City, GameObject>();

    private void Start()
    {
        gameBoard = FindAnyObjectByType<GameBoard>();
        if (gameBoard == null)
        {
            Debug.LogError("No GameBoard found in scene!");
            return;
        }

        DrawCities();
        DrawConnections();

        foreach (Transform child in boardContainer)
        {
        if (child.name.Contains("Line")) // for line prefab
            child.SetSiblingIndex(0);
        else
            child.SetSiblingIndex(1);
        }
    }

    private void DrawCities()
    {
        foreach (CityData data in citiesDB.cities)
        {
            City city = gameBoard.cityLookup[data.cityName];
            GameObject marker = Instantiate(cityMarkerPrefab, boardContainer);
            RectTransform rect = marker.GetComponent<RectTransform>();
            rect.anchoredPosition = data.position;

            // Set colour based on the city's disease type
            Image img = marker.GetComponent<Image>();
            if (img != null)
                img.color = GetDisplayColor(data.diseaseColor);

            cityMarkers[city] = marker;
        }
    }

    private void DrawConnections()
    {
        // Use a set to avoid duplicate lines
        HashSet<(City, City)> drawn = new HashSet<(City, City)>();

        foreach (City city in gameBoard.cities)
        {
            foreach (City neighbor in city.neighbors)
            {
                if (drawn.Contains((city, neighbor)) || drawn.Contains((neighbor, city)))
                    continue;
                drawn.Add((city, neighbor));

                // Get positions of the two city markers
                if (cityMarkers.TryGetValue(city, out GameObject fromObj) &&
                    cityMarkers.TryGetValue(neighbor, out GameObject toObj))
                {
                    Vector2 fromPos = fromObj.GetComponent<RectTransform>().anchoredPosition;
                    Vector2 toPos = toObj.GetComponent<RectTransform>().anchoredPosition;
                    if(Mathf.Abs(toPos.x - fromPos.x) > boardContainer.rect.width / 2f)
                        WrapLine(fromPos, toPos);
                    else DrawLine(fromPos, toPos);
                }
            }
        }
    }

    private void WrapLine(Vector2 from, Vector2 to)
    {
        bool left = (to.x > from.x);
        float width = boardContainer.rect.width;

        Vector2 leftPoint, rightPoint;
        float shift = 0;

        if (left)  shift = to.x - width;
        else shift = to.x + width;

        float slope = (to.y - from.y) / (shift - from.x);

        float edgeY = (slope * (shift - from.x)) + from.y;

        leftPoint = new Vector2(0, edgeY);
        rightPoint = new Vector2(width, edgeY);

        Debug.Log(leftPoint);
        Debug.Log(rightPoint);

        DrawLine(from, left ? leftPoint : rightPoint);
        DrawLine(to, left ? rightPoint  : leftPoint);
    }

    private void DrawLine(Vector2 start, Vector2 end, float thickness = 5f)
    {
        GameObject line = Instantiate(connectionLinePrefab, boardContainer);
        RectTransform rt = line.GetComponent<RectTransform>();
        Vector2 dir = end - start;
        float dist = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rt.anchoredPosition = start;
        rt.sizeDelta = new Vector2(dist, thickness);
        rt.rotation = Quaternion.Euler(0, 0, angle);
    }

    private Color GetDisplayColor(DiseaseColor disease)
    {
        switch (disease)
        {
            case DiseaseColor.Red: return Color.red;
            case DiseaseColor.Blue: return Color.blue;
            case DiseaseColor.Yellow: return Color.yellow;
            case DiseaseColor.Black: return Color.black;
            default: return Color.gray;
        }
    }
}
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Timeline;
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

    [Header("City Card")]
    [SerializeField] private CityCard cc;

    private GameBoard gameBoard;       // reference to the logic board
    private Dictionary<City, GameObject> cityMarkers = new Dictionary<City, GameObject>();
    private PlayerGraphics pg;
    private GameManager gm;


    private void Start()
    {
        gameBoard = FindAnyObjectByType<GameBoard>();
        if (gameBoard == null)
        {
            Debug.LogError("No GameBoard found in scene!");
            return;
        }
        pg = FindAnyObjectByType<PlayerGraphics>();
        gm = FindAnyObjectByType<GameManager>();

        DrawCities();

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

            Button btn = marker.GetComponent<Button>();
            btn.onClick.AddListener(() => OnCityClick(city));

            var outline = marker.AddComponent<Outline>();
            outline.effectColor = Color.softYellow;
            outline.enabled = false;
        }
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

    private void OnCityClick(City city)
    {
        cc.OnClick(city);
        if (pg.driveState && gm.players[gm.currentPlayerIndex].CurrentCity.neighbors.Contains(city))
        {
            gm.players[gm.currentPlayerIndex].MoveTo(city);
            Debug.Log($"Player {gm.currentPlayerIndex + 1} has moved to {city}");
            gm.actionCount--;
            gm.EndTurn();
        }
    }

    public void Highlight(City city, bool value)
    {
        var outline = cityMarkers[city].GetComponent<Outline>();
        outline.enabled = value;
    }
}
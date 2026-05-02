using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class GameBoardGraphics : MonoBehaviour
{
    [Header("Board visuals")]
    [SerializeField] private Image background;
    [SerializeField] private RectTransform boardContainer;

    [Header("City config")]
    [SerializeField] private GameObject cityMarkerPrefab;
    [SerializeField] private GameObject connectionLinePrefab;

    [Header("City positions")]
    [SerializeField] private CityDB cities = new CityDB();

    
}
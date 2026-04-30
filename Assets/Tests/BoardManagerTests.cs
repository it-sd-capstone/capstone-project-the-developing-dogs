using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;


public class BoardManagerTests
{
    GameBoard board;

    [SetUp]
    public void Setup()
    {
        board = new GameBoard();
        board.cities = new List<City>()
        {
            new City {cityName = "Atlanta", diseaseColor = Color.blue},
            new City {cityName = "Chicago", diseaseColor = Color.blue}
        };
    }
}
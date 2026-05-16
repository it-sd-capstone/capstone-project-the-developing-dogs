using UnityEngine;

public static class GameSettings
{
    public static int PlayerCount = 2;
    public static string Difficulty = "Not Selected";
    public static string[] SelectedRoles = new string[4];

    public static bool HasSelectedPlayerCount = false;
    public static bool HasSelectedDifficulty = false;
}
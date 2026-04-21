using UnityEngine;

public class TestManager : MonoBehaviour
{
    void Start()
    {
        // Automatically runs when you press Play
        TestCityConnection();
        TestInfection();
        TestTurnSystem();
    }

    public void TestCityConnection()
    {
        Debug.Log("City connection system is working.");
    }

    public void TestInfection()
    {
        Debug.Log("Infection system is working.");
    }

    public void TestTurnSystem()
    {
        Debug.Log("Turn system is working.");
    }
}
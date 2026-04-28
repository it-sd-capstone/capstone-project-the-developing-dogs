using UnityEngine;

// Simple test script used to confirm that basic systems are running in Play Mode.
public class TestManager : MonoBehaviour
{
    void Start()
    {
        // Automatically runs when you press Play.
        TestCityConnection();
        TestInfection();
        TestTurnSystem();
    }

    // Placeholder test for city connection logic.
    public void TestCityConnection()
    {
        Debug.Log("City connection system is working.");
    }

    // Placeholder test for infection logic.
    public void TestInfection()
    {
        Debug.Log("Infection system is working.");
    }

    // Placeholder test for turn system logic.
    public void TestTurnSystem()
    {
        Debug.Log("Turn system is working.");
    }
}

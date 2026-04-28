using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Loads the main board scene when the player clicks Play.
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Board");
    }

    // Quits the game. This works in a built version of the game.
    public void QuitGame()
    {
        Application.Quit();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Start Game clicked");
        SceneManager.LoadScene("Board");
    }

    // Quits the game. This works in a built version of the game.
    public void QuitGame()
    {
        Application.Quit();
    }

    [Header("Panels")]
    public GameObject playerSelectionPanel;
    public GameObject difficultyPanel;

    [Header("Message Text")]
    public TMP_Text playerSelectionText;
    public TMP_Text difficultySelectionText;
    public TMP_Text finalSelectionText;

    private int selectedPlayers = 0;
    private string selectedDifficulty = "Not selected";

    public void SetPlayerCount(int count)
    {
        selectedPlayers = count;
        GameSettings.PlayerCount = count;

        playerSelectionText.text = "Players selected: " + count;

        finalSelectionText.gameObject.SetActive(false);  // Hide if settings are modified
    }

    public void SetDifficulty(string difficulty)
    {
        selectedDifficulty = difficulty;
        GameSettings.Difficulty = difficulty;

        difficultySelectionText.text = "Difficulty selected: " + difficulty;

        finalSelectionText.gameObject.SetActive(true); 
        UpdateFinalSelectionText();
    }

    public void ShowDifficultyPanel()
    {
        playerSelectionPanel.SetActive(false);
        difficultyPanel.SetActive(true);

        UpdateFinalSelectionText();
    }

    public void ShowPlayerSelectionPanel()
    {
        difficultyPanel.SetActive(false);
        playerSelectionPanel.SetActive(true);
    }

    void Start()
    {
        finalSelectionText.gameObject.SetActive(false);
    }

    private void UpdateFinalSelectionText()
    {
        finalSelectionText.text =
            "Current settings:" + "\n" +
            "Players: " + selectedPlayers + "\n" +
            "Difficulty: " + selectedDifficulty;
    }
}

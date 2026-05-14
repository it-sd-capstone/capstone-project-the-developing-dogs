using TMPro;
using UnityEngine.UI;
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

    [Header("Player Buttons")]
    public Button soloButton;
    public Button twoPlayerButton;
    public Button threePlayerButton;
    public Button fourPlayerButton;

    public Color normalButtonColor = Color.white;
    public Color selectedButtonColor = Color.yellow;

    [Header("Difficulty Buttons")]
    public Button introductoryButton;
    public Button standardButton;
    public Button heroicButton;

    private void ResetDifficultyButtonColors()
    {
        introductoryButton.image.color = normalButtonColor;
        standardButton.image.color = normalButtonColor;
        heroicButton.image.color = normalButtonColor;
    }

    private void ResetPlayerButtonColors()
    {
        soloButton.image.color = normalButtonColor;
        twoPlayerButton.image.color = normalButtonColor;
        threePlayerButton.image.color = normalButtonColor;
        fourPlayerButton.image.color = normalButtonColor;
    }

    public void SetPlayerCount(int count)
    {
        selectedPlayers = count;
        GameSettings.PlayerCount = count;

        ResetPlayerButtonColors();

        if (count == 1)
            soloButton.image.color = selectedButtonColor;
        else if (count == 2)
            twoPlayerButton.image.color = selectedButtonColor;
        else if (count == 3)
            threePlayerButton.image.color = selectedButtonColor;
        else if (count == 4)
            fourPlayerButton.image.color = selectedButtonColor;

        if (finalSelectionText != null)
            finalSelectionText.gameObject.SetActive(false);
    }

    public void SetDifficulty(string difficulty)
    {
        selectedDifficulty = difficulty;
        GameSettings.Difficulty = difficulty;

        ResetDifficultyButtonColors();

        switch (difficulty)
        {
            case "Introductory":
                introductoryButton.image.color = selectedButtonColor;
                break;

            case "Standard":
                standardButton.image.color = selectedButtonColor;
                break;

            case "Heroic":
                heroicButton.image.color = selectedButtonColor;
                break;
        }

        if (difficultySelectionText != null)
            difficultySelectionText.text = "Difficulty selected: " + difficulty;

        if (finalSelectionText != null)
        {
            finalSelectionText.gameObject.SetActive(true);
            UpdateFinalSelectionText();
        }
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
            "Difficulty: " + "\n" + selectedDifficulty;
    }
}

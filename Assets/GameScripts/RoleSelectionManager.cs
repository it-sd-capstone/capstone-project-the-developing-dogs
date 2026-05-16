using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoleSelectionManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject setupPanel;
    public GameObject roleSelectionPanel;

    [Header("Text")]
    public TMP_Text currentPlayerText;
    public TMP_Text selectedRoleText;
    public TMP_Text setupWarningText;

    [Header("Role Buttons")]
    public Button medicButton;
    public Button scientistButton;
    public Button dispatcherButton;
    public Button researcherButton;
    public Button quarantineSpecialistButton;
    public Button contingencyPlannerButton;
    public Button operationsExpertButton;

    [Header("Navigation Buttons")]
    public Button continueButton;
    public Button startGameButton;

    [Header("Colors")]
    public Color normalButtonColor = Color.white;
    public Color selectedButtonColor = Color.green;
    public Color disabledButtonColor = Color.gray;

    private int currentPlayerIndex = 0;
    private string currentSelectedRole = "";

    private readonly Dictionary<string, Button> roleButtons = new Dictionary<string, Button>();

    private void Awake()
    {
        roleButtons["Medic"] = medicButton;
        roleButtons["Scientist"] = scientistButton;
        roleButtons["Dispatcher"] = dispatcherButton;
        roleButtons["Researcher"] = researcherButton;
        roleButtons["Quarantine Specialist"] = quarantineSpecialistButton;
        roleButtons["Contingency Planner"] = contingencyPlannerButton;
        roleButtons["Operations Expert"] = operationsExpertButton;
    }

    public void OpenRoleSelection()
    {
        if (!GameSettings.HasSelectedPlayerCount || !GameSettings.HasSelectedDifficulty)
        {
            if (setupWarningText != null)
            {
                setupWarningText.text = "Please select the number of players and difficulty first.";
            }

            return;
        }

        if (setupWarningText != null)
        {
            setupWarningText.text = "";
        }

        currentPlayerIndex = 0;
        currentSelectedRole = "";

        for (int i = 0; i < GameSettings.SelectedRoles.Length; i++)
        {
            GameSettings.SelectedRoles[i] = "";
        }

        setupPanel.SetActive(false);
        roleSelectionPanel.SetActive(true);

        UpdateRoleScreen();
    }

    public void SelectRole(string roleName)
    {
        if (IsRoleAlreadySelected(roleName))
        {
            return;
        }

        currentSelectedRole = roleName;

        selectedRoleText.text = "Selected Role: " + roleName;

        if (IsLastPlayer())
        {
            startGameButton.interactable = true;
        }
        else
        {
            continueButton.interactable = true;
        }

        UpdateButtonHighlights();
    }

    public void ContinueToNextPlayer()
    {
        Debug.Log("Saved role for Player " + (currentPlayerIndex + 1) + ": " + currentSelectedRole);

        GameSettings.SelectedRoles[currentPlayerIndex] = currentSelectedRole;

        currentPlayerIndex++;
        currentSelectedRole = "";

        UpdateRoleScreen();
    }

    public void StartGame()
    {
        GameSettings.SelectedRoles[currentPlayerIndex] = currentSelectedRole;

        SceneManager.LoadScene("Board");
    }

    private void UpdateRoleScreen()
    {
        currentPlayerText.text = "Player " + (currentPlayerIndex + 1) + ": Select Your Role";
        selectedRoleText.text = "Selected Role: None";

        continueButton.interactable = false;
        startGameButton.interactable = false;

        if (IsLastPlayer())
        {
            continueButton.gameObject.SetActive(false);
            startGameButton.gameObject.SetActive(true);
        }
        else
        {
            continueButton.gameObject.SetActive(true);
            startGameButton.gameObject.SetActive(false);
        }

        UpdateButtonHighlights();
    }

    private void UpdateButtonHighlights()
    {
        foreach (var roleButton in roleButtons)
        {
            string roleName = roleButton.Key;
            Button button = roleButton.Value;

            bool alreadySelected = IsRoleAlreadySelected(roleName);

            if (alreadySelected)
            {
                button.interactable = false;
                button.image.color = disabledButtonColor;
            }
            else if (roleName == currentSelectedRole)
            {
                button.interactable = true;
                button.image.color = selectedButtonColor;
            }
            else
            {
                button.interactable = true;
                button.image.color = normalButtonColor;
            }
        }
    }

    private bool IsRoleAlreadySelected(string roleName)
    {
        for (int i = 0; i < currentPlayerIndex; i++)
        {
            if (GameSettings.SelectedRoles[i] == roleName)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsLastPlayer()
    {
        return currentPlayerIndex >= GameSettings.PlayerCount - 1;
    }
}
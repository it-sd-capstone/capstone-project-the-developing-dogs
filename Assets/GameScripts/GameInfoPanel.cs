using UnityEngine;

public class GameInfoPanelToggle : MonoBehaviour
{
    public GameObject gameInfoPanel;

    public void TogglePanel()
    {
        gameInfoPanel.SetActive(!gameInfoPanel.activeSelf);
    }
}
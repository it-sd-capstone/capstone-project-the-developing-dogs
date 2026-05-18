using TMPro;
using UnityEngine;

public class GameInfoUI : MonoBehaviour
{
    [Header("Text References")]
    public TMP_Text currentPlayerText;
    public TMP_Text roleText;
    public TMP_Text playerDeckCountText;
    public TMP_Text infectionDeckCountText;

    public void UpdateGameInfo(Player currentPlayer, PlayerDeck playerDeck, InfectionDeck infectionDeck)
    {
        currentPlayerText.text = "Current Player: " + currentPlayer.PlayerName;

        string roleName = currentPlayer.Role != null
            ? currentPlayer.Role.RoleName
            : (string.IsNullOrEmpty(currentPlayer.RoleName) ? "None" : currentPlayer.RoleName);

        roleText.text = "Role: " + roleName;

        playerDeckCountText.text = "Player Deck Cards Remaining: " + playerDeck.Count;
        infectionDeckCountText.text = "Infection Deck Cards Remaining: " + infectionDeck.Count;
    }
}

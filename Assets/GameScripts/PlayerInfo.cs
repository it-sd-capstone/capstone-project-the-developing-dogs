using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private GameManager gm;

    [Header("Info readouts")]
    [SerializeField] private TextMeshProUGUI pName;
    [SerializeField] private TextMeshProUGUI pRole;
    [SerializeField] private TextMeshProUGUI actions;

    public void OnPlayerChange(Player player)
    {
        if (player == null)
        {
            Debug.Log("missing player");
            return;
        }

        // Player name
        pName.text = string.IsNullOrEmpty(player.PlayerName)
            ? "Unknown"
            : player.PlayerName;

        // Role name (from Role object if available)
        if (player.Role != null && !string.IsNullOrEmpty(player.Role.RoleName))
        {
            pRole.text = player.Role.RoleName;
        }
        else if (!string.IsNullOrEmpty(player.RoleName))
        {
            // Fallback to Player.RoleName string if still used elsewhere
            pRole.text = player.RoleName;
        }
        else
        {
            pRole.text = "?";
        }

        // Show current action count instead of hard‑coded "4"
        if (gm != null)
        {
            actions.text = gm.actionCount.ToString();
        }
        else
        {
            actions.text = "0";
        }
    }

    public void OnAction()
    {
        if (gm != null && actions != null)
        {
            actions.text = gm.actionCount.ToString();
        }
    }
}

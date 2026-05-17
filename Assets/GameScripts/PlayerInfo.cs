using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] GameManager gm;
    [Header("Info readouts")]
    [SerializeField]private TextMeshProUGUI pName;
    [SerializeField]private TextMeshProUGUI pRole;
    [SerializeField]private TextMeshProUGUI actions;

    public void OnPlayerChange(Player player)
    {
        if (player == null)
        {
            Debug.Log("missing player");
            return;
        }

        pName.text = player.PlayerName;

        if (player.RoleName == null)
            pRole.text = "?";
        else
            pRole.text = player.RoleName;

        actions.text = "4";
    }

    public void OnAction()
    {
        actions.text = gm.actionCount.ToString();
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] GameManager gm;
    [Header("Info readouts")]
    [SerializeField]private TextMeshProUGUI pName;
    [SerializeField]private TextMeshProUGUI pRole;
    [SerializeField]private TextMeshProUGUI actions;
    [SerializeField]private GameObject PlayerIcon;

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

        PlayerIcon.GetComponent<Image>().color = gm.GetPawnColor(player.Role);
        PlayerIcon.GetComponent<Button>().onClick.AddListener(()=>player.UseRoleAbility(gm.board));
        
    }

    public void OnAction()
    {
        actions.text = gm.actionCount.ToString();
    }
}

<<<<<<< HEAD
using System;
using TMPro;
=======
﻿using TMPro;
>>>>>>> 09fdd5a98997c8b88f10955fc65e16fbaa51b973
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private GameManager gm;

    [Header("Info readouts")]
<<<<<<< HEAD
    [SerializeField]private TextMeshProUGUI pName;
    [SerializeField]private TextMeshProUGUI pRole;
    [SerializeField]private TextMeshProUGUI actions;
    [SerializeField]private GameObject PlayerIcon;
=======
    [SerializeField] private TextMeshProUGUI pName;
    [SerializeField] private TextMeshProUGUI pRole;
    [SerializeField] private TextMeshProUGUI actions;
>>>>>>> 09fdd5a98997c8b88f10955fc65e16fbaa51b973

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

<<<<<<< HEAD
        actions.text = "4";

        PlayerIcon.GetComponent<Image>().color = gm.GetPawnColor(player.Role);
        PlayerIcon.GetComponent<Button>().onClick.AddListener(()=>player.UseRoleAbility(gm.board));
        
=======
        // Show current action count instead of hard‑coded "4"
        if (gm != null)
        {
            actions.text = gm.actionCount.ToString();
        }
        else
        {
            actions.text = "0";
        }
>>>>>>> 09fdd5a98997c8b88f10955fc65e16fbaa51b973
    }

    public void OnAction()
    {
        if (gm != null && actions != null)
        {
            actions.text = gm.actionCount.ToString();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class GameManagerGraphics : MonoBehaviour
{
    public GameManager manager;

    public Image[] infectionRateHighlights;

    private void Update()
    {
        if (manager == null) return;

        for (int i = 0; i < infectionRateHighlights.Length; i++)
        {
            infectionRateHighlights[i].enabled = (i == manager.infectionRateIndex);
        }
    }
}

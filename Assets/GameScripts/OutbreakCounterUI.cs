using UnityEngine;
using UnityEngine.UI;

public class OutbreakCounterUI : MonoBehaviour
{
    public GameBoard board;
    public Image[] outbreakSlots; // 9 highlight images

    void Update()
    {
        if (board == null) return;

        int count = board.outbreakCounter;

        // Highlight all slots up to the current outbreak count
        for (int i = 0; i < outbreakSlots.Length; i++)
        {
            outbreakSlots[i].enabled = (i <= count);
        }
    }
}

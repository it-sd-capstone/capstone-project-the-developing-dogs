using UnityEngine;

public class EpidemicCard : PlayerCard
{
    public EpidemicCard()
    {
        // Epidemic cards do not need city or color
        City = null;
        Color = default;
    }
}
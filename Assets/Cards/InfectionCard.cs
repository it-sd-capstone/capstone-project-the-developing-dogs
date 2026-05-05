using UnityEngine;

public class InfectionCard
{
    public City City { get; private set; }
    public DiseaseColor Color { get; private set; }

    public InfectionCard(City city, DiseaseColor color)
    {
        City = city;
        Color = color;
    }
}
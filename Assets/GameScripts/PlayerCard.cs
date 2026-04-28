// Basic player card class.
// A card can be tied to a city and a disease color for cure and sharing rules.
public class PlayerCard
{
    // City connected to this card.
    public City City { get; set; }

    // Disease color of this card.
    public DiseaseColor Color { get; set; }
}

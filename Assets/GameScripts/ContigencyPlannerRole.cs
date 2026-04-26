using UnityEngine;

public class ContigencyPlannerRole : Role
{
    public EventCard StoredEventCard { get; private set; }

    public ContingencyPlannerRole()
    {
        RoleName = "Contingency Planner";
        RoleColor = new Color(0.2f, 0.6f, 1f);
    }

    public bool CanStoreEventCard => StoredEventCard == null;

    public void StoreEventCard(EventCard card)
    {
        if (StoredEventCard == null)
            StoredEventCard = card;
    }

    public EventCard PlayStoredEventCard()
    {
        EventCard card = StoredEventCard;
        StoredEventCard = null;
        return card;
    }
}

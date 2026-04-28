using UnityEngine;

// Role for the Contingency Planner.
// This role can store one event card and play it later.
public class ContingencyPlannerRole : Role
{
    // The event card currently being saved by this role.
    public EventCard StoredEventCard { get; private set; }

    public ContingencyPlannerRole()
    {
        RoleName = "Contingency Planner";
        RoleColor = new Color(0.2f, 0.6f, 1f);
    }

    // The player can only store a card if there is no stored card already.
    public bool CanStoreEventCard => StoredEventCard == null;

    // Stores an event card if the stored slot is empty.
    public void StoreEventCard(EventCard card)
    {
        if (StoredEventCard == null)
            StoredEventCard = card;
    }

    // Plays the stored event card and clears the stored card slot.
    public EventCard PlayStoredEventCard()
    {
        EventCard card = StoredEventCard;
        StoredEventCard = null;
        return card;
    }
}

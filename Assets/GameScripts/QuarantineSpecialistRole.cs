using UnityEngine;

// Role for the Quarantine Specialist.
// This role prevents infection in the player's city and connected cities.
public class QuarantineSpecialistRole : Role
{
    public QuarantineSpecialistRole()
    {
        RoleName = "Quarantine Specialist";
        RoleColor = new Color(0.1f, 0.8f, 0.1f);
    }

    // GameManager can call this before infecting a city.
    public bool PreventsInfectionIn(City city)
    {
        // Prevent infection in the player's current city.
        if (player.CurrentCity == city)
            return true;

        // Prevent infection in a neighboring city.
        if (player.CurrentCity.IsConnectedTo(city))
            return true;

        return false;
    }
}

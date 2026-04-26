using UnityEngine;

public class QuarantineSpecialistRole : Role
{
    public QuarantineSpecialistRole()
    {
        RoleName = "Quarantine Specialist";
        RoleColor = new Color(0.1f, 0.8f, 0.1f);
    }

    // GameManager checks infecting cities
    public bool PreventsInfectionIn(City city)
    {
        // Prevent infection- player
        if (player.CurrentCity == city)
            return true;

        // Prevent infection - neighbor
        if (player.CurrentCity.IsConnectedTo(city))
            return true;

        return false;
    }
}

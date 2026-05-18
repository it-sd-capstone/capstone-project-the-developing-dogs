using UnityEngine;

public class QuarantineSpecialistRole : Role
{
    public QuarantineSpecialistRole()
    {
        RoleName = "Quarantine Specialist";
        RoleColor = new Color(0.1f, 0.8f, 0.1f);
    }

    public bool PreventsInfectionIn(City city)
    {
        if (player == null || player.CurrentCity == null)
            return false;

        if (player.CurrentCity == city)
            return true;

        if (player.CurrentCity.IsConnectedTo(city))
            return true;

        return false;
    }
}

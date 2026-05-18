using UnityEngine;

public class ScientistRole : Role
{
    public ScientistRole()
    {
        RoleName = "Scientist";
        RoleColor = Color.white;
    }

    public override int CardsRequiredForCure => 4;
}

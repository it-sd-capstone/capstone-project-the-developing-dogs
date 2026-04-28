using UnityEngine;

// Role for the Scientist.
// Scientist needs fewer cards to discover a cure.
public class ScientistRole : Role
{
    public ScientistRole()
    {
        RoleName = "Scientist";
        RoleColor = Color.white;
    }

    // Scientist only needs 4 cards instead of the default 5.
    public override int CardsRequiredForCure => 4;
}

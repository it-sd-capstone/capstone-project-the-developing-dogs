using UnityEngine;

// Role for the Operations Expert.
// This role can be expanded later for research station and special action abilities.
public class OperationsExpertRole : Role
{
    public OperationsExpertRole()
    {
        RoleName = "Operations Expert";
        RoleColor = new Color(0.1f, 0.6f, 0.1f);
    }

    public override void UseSpecialAbility(GameBoard board)
    {
        // Empty for now. Operations Expert ability logic will be added later.
    }
}

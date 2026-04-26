using UnityEngine;

public class OperationsExpertRole : Role
{
    public OperationsExpertRole()
    {
        RoleName = "Operations Expert";
        RoleColor = new Color(0.1f, 0.6f, 0.1f);
    }

    public override void UseSpecialAbility(Board board)
    {
        // Empty for now
    }
}

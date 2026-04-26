using UnityEngine;

public class DispatcherRole : Role
{
    public DispatcherRole()
    {
        RoleName = "Dispatcher";
        RoleColor = new Color(0.7f, 0.2f, 0.8f);
    }

    public override void UseSpecialAbility(Board board)
    {
        // Empty for now
    }
}

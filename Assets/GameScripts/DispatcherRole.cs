using UnityEngine;

// Role for the Dispatcher.
// The special ability can be added later when movement rules are built out more.
public class DispatcherRole : Role
{
    public DispatcherRole()
    {
        RoleName = "Dispatcher";
        RoleColor = new Color(0.7f, 0.2f, 0.8f);
    }

    public override void UseSpecialAbility(GameBoard board)
    {
        // Empty for now. Dispatcher ability logic will be added later.
    }
}

using UnityEngine;

public class OperationsExpertRole : Role
{
    public OperationsExpertRole()
    {
        RoleName = "Operations Expert";
        RoleColor = new Color(0.4f, 0.4f, 0.4f);
        CanBuildStationForFree = true;
    }
}

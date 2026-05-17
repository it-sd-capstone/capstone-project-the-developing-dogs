using UnityEngine;

// Role for the Operations Expert.
// This role can be expanded later for research station and special action abilities.
public class OperationsExpertRole : Role
{
    public new void Initialize(Player player)
    {
        this.player = player;
        CanBuildStationForFree = true;
    }
}

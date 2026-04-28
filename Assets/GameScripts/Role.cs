using UnityEngine;

// Base class for all player roles.
// Each role can override these properties and methods to create special abilities.
public abstract class Role
{
    // The player who currently owns this role.
    protected Player player;

    // Display information for the role.
    public string RoleName { get; protected set; }
    public Color RoleColor { get; protected set; }

    // Default role rules. Child role classes can override these when needed.
    public virtual int CardsRequiredForCure => 5;
    public virtual bool CanGiveAnyCard => false;
    public virtual bool CanBuildStationForFree => false;

    // Gives the role a reference to the player that owns it.
    public void Initialize(Player owner)
    {
        player = owner;
    }

    // Hook method for roles that change disease treatment behavior.
    public virtual void OnTreatDisease(City city, DiseaseColor color, ref int cubesToRemove)
    {
        // Default is empty.
    }

    // Hook method for roles that have a special ability.
    public virtual void UseSpecialAbility(GameBoard board)
    {
        // Default is empty.
    }
}

using UnityEngine;

public abstract class Role
{
    public void Initialize(Player owner)
    {
        player = owner;
    }

    public virtual int CardsRequiredForCure => 5;

    public virtual void OnTreatDisease(City city, DiseaseColor color, ref int cubesToRemove)
    {
        //Default is empty
    }

    public virtual void UseSpecialAbility(Board board)
    {
        // Default is empty
    } 
}

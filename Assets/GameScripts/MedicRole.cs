using UnityEngine;

// Role for the Medic.
// The Medic removes all cubes of the selected color instead of only one.
public class MedicRole : Role
{
    public MedicRole()
    {
        RoleName = "Medic";
        RoleColor = new Color(1f, 0.5f, 0f);
    }

    public override void OnTreatDisease(City city, DiseaseColor color, ref int cubesToRemove)
    {
        // Remove all cubes of that color in one action.
        int cubesInCity = city.GetDiseaseCount(color);
        cubesToRemove = cubesInCity;
    }
}

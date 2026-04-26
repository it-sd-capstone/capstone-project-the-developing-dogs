using UnityEngine;

public class MedicRole : Role
{
    public MedicRole()
    {
        RoleName = "Medic";
        RoleColor = new Color(1f, 0.5f, 0f); 
    }

    public override void OnTreatDisease(City city, DiseaseColor color, ref int cubesToRemove)
    {
        //Remove all cubes of that color in one action
        int cubesInCity = city.GetCubeCount(color);
        cubesToRemove = cubesInCity;
    }
}

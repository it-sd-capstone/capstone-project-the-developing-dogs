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
        int cubesInCity = city.GetDiseaseCount(color);
        cubesToRemove = cubesInCity;
    }
}

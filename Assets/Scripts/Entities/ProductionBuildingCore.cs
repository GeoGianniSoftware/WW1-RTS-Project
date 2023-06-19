using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProductionBuildingCore", menuName = "New Entity/New Production Building", order = 0)]
[System.Serializable]
public class ProductionBuildingCore : BuildingCore
{
    public List<EntityCore> productionList = new List<EntityCore>();
    public float productionPerSecond = 1f;

    private void OnValidate() {
        if (TYPE == EntityType.BASIC)
            TYPE = EntityType.PRODUCTION;
    }
}

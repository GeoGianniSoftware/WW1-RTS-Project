using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFactoryBuildingCore", menuName = "New Entity/New Factory Building", order = 0)]
[System.Serializable]
public class FactoryBuildingCore : BuildingCore
{
    public ResourceCore productionType;
    public float productionTime;
    public int productionAmount;
    public int maxStorageAmount;

    private void OnValidate() {
        if (TYPE == EntityType.BASIC)
            TYPE = EntityType.FACTORY;
    }
}

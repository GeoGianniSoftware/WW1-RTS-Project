using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingCore", menuName = "New Entity/New Building", order = 0)]
[System.Serializable]
public class BuildingCore : EntityCore
{
    public bool continousDefense;
    public buildingType coreType;

    public bool canBeBuilt = true;
    public EntityCore backfillEntityCore = null;
    public bool flipBackfill = false;
    public bool replaceOnExtend = false;
    public List<BuildingCore> replaceExtend = new List<BuildingCore>();

    public bool canHoldCargo = false;
    public int maxSupplyStorage;

    public bool canHoldPassengers = false;

    [Header("Garrison")]
    public bool canBeStationed = false;
    public float stationOffsetDistance = 0f;
    public float garrison_rangeOffset;
    public float garrison_hitchanceOffset;
}

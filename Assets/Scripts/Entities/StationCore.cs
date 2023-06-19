using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStationCore", menuName = "New Entity/New Station", order = 0)]
[System.Serializable]
public class StationCore : BuildingCore
{
    public bool fillContinous = false;
    public float continousOffset = 0f;


    private void OnValidate() {
        coreType = buildingType.Station;
    }
}

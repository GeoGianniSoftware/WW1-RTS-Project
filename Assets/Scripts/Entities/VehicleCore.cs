using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVehicleCore", menuName = "New Entity/New Vehicle", order = 1)]
[System.Serializable]
public class VehicleCore : UnitCore
{
    public UnitCore crewCore;
    public List<int> driverPositions = new List<int>();
    public bool isCargoVehicle;
    public bool isTransportVehicle;
    public bool requiresSupply;
    public int maxSupplyStorage;
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStaticDefenseCore", menuName = "New Entity/New Static Defense", order = 0)]
[System.Serializable]
public class DefenseCore : BuildingCore
{
    public bool fillContinous = false;
    public float continousOffset = 0f;

    private void OnValidate() {
        coreType = buildingType.Static;
    }
}

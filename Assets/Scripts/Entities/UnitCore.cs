using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewUnitCore", menuName = "New Entity/New Unit", order = 0)]
[System.Serializable]
public class UnitCore : EntityCore
{
    public float speed;
    public float range;
    public float accuracy = .3f;
    public float fireRate;

    public List<Unit_Order.OrderTypes> unitActions = new List<Unit_Order.OrderTypes>();
}



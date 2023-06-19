using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Unit_Order
{
    public enum OrderTypes { 
        MoveOrder,
        AttackOrder,
        GarrisonOrder,
        BuildOrder,
        LoadSupplyOrder,
        UnloadSupplyOrder,
        EnterVehicleOrder,
        CrewVehicleOrder,
        UnloadVehiclePassengersOrder,
        UnloadVehicleCrewOrder,
        LoadPassengersOrder,
        TransferPassengersOrder,
    }

    public OrderTypes orderType;
    public Vector3 orderPosition;
    public Entity orderTarget;
    public LogisticLink logicLink = null;

    public Unit_Order(OrderTypes Type, Vector3 pos) {
        orderType = Type;
        orderPosition = pos;
    }

    public Unit_Order(OrderTypes Type, Vector3 pos, Entity target, LogisticLink link = null) {
        orderType = Type;
        orderPosition = pos;
        orderTarget = target;
        logicLink = link;
    }

}

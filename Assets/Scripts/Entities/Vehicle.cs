using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Vehicle : Unit
{
    public float driverOffset = 1.73f;
    public Unit[] passengers;
    public List<Transform> passengerPositions;
    public List<Transform> cargoPositions;
    public List<Entity> cargoList;

    public int currentSupply;


    GameObject driverGO = null;

    public UnitCore tempPassenger;



    

    private void Start() {
        NMA = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();

        passengers = new Unit[passengerPositions.Count];

        SpawnDriver();
        //SpawnPassengers(tempPassenger);
    }

    private void Update() {
        VehicleTick();
        Move();
        PickupResources();
    }

    public override void Interact(Entity entityInteracting) {

        if (getVehicleCore().isCargoVehicle && CMD.checkBasicType(EntityType.BUILDING, entityInteracting) && ((BuildingCore)entityInteracting.entityCore).canHoldCargo && ((BuildingCore)entityInteracting.entityCore).coreType != buildingType.Factory) {
            Building buildingToSupply = (Building)entityInteracting;
            if (CMD.CMND.cmd_logistics.SearchLogisticsLinks(buildingToSupply).Count > 0) {
                AddUnloadSupplyOrder(buildingToSupply, CMD.CMND.cmd_logistics.SearchLogisticsLinks(buildingToSupply)[0].routeID);
                print("Attempting to Supply: " + entityInteracting.name);

            }
            else {

                AddLoadSupplyOrder(buildingToSupply, -1);
            }
            print("Attempting to Supply: " + entityInteracting.name);

            
            
        }else if (getVehicleCore().isCargoVehicle && CMD.checkBasicType(EntityType.BUILDING, entityInteracting) && ((BuildingCore)entityInteracting.entityCore).canHoldCargo && ((BuildingCore)entityInteracting.entityCore).coreType == buildingType.Factory) {
            Building buildingToDesupply = (Building)entityInteracting;
            if (CMD.CMND.cmd_logistics.SearchLogisticsLinks(buildingToDesupply).Count > 0) {
                AddUnloadSupplyOrder(buildingToDesupply, CMD.CMND.cmd_logistics.SearchLogisticsLinks(buildingToDesupply)[0].routeID);
                print("Attempting to Supply: " + entityInteracting.name);

            }
            
        }
        else if (getVehicleCore().isTransportVehicle && entityInteracting.entityCore.TYPE == EntityType.STATION && ((StationCore)entityInteracting.entityCore).canHoldPassengers) {
            Station buildingToDesupply = (Station)entityInteracting;
            if (CMD.CMND.cmd_logistics.SearchLogisticsLinks(buildingToDesupply).Count > 0) {
                AddLoadPassengersOrder(buildingToDesupply, CMD.CMND.cmd_logistics.SearchLogisticsLinks(buildingToDesupply)[0].routeID);
                print("Attempting to Pickup Passengers: " + entityInteracting.name);

            }

        }
        else {
            print("Interacting with " + entityInteracting.name);
        }
    }

    void PickupResources() {
        if (canCarryCargo()) {
            Collider[] cols = Physics.OverlapSphere(transform.position, 3f);
            foreach (Collider c in cols) {
                if (c.GetComponent<Resource>() && c.GetComponent<Resource>().resourcePickup) {
                    loadCargo(c.GetComponent<Resource>());
                }
            }
        }   
    }

    public bool canCarryCargo() {
        if (getVehicleCore().isCargoVehicle && cargoList.Count < cargoPositions.Count)
            return true;

        return false;
    }

    public bool canCarryPassengers() {
        if (getVehicleCore().isTransportVehicle && getEmptySeatCount() > 0)
            return true;

        return false;
    }

    public int getEmptySeatCount() {
        int count = 0;
        for (int i = 0; i < passengers.Length; i++) {
            if (passengers[i] == null) {
                count++;
            }
        }
        return count;
    }

    public int getPassengerSeatCount() {
        int count = 0;
        for (int i = 0; i < passengers.Length; i++) {
            if (passengers[i] != null) {
                count++;
            }
        }
        return count;
    }

    public int getNextEmptySeat() {
        for (int i = 0; i < passengers.Length; i++) {
            if (passengers[i] == null) {
                return i;
            }
        }
        return -1;
    }

    public int loadCargo(Entity cargo) {
        if (canCarryCargo() && cargo.entityCore.TYPE != EntityType.UNIT && !cargoList.Contains(cargo)) {
            cargoList.Add(cargo);
            cargo.transform.position = cargoPositions[cargoList.Count - 1].position;
            cargo.transform.SetParent(cargoPositions[cargoList.Count - 1], true);
            cargo.transform.localRotation = Quaternion.Euler(Vector3.zero);
            cargo.isCargo = true;
            return cargoList.Count-1;
        }
        return -1;
    }

    public Entity offloadCargo(int index) {
        if (index < cargoList.Count) {
            Entity e = cargoList[index];
            e.isCargo = false;
            cargoList.Remove(e);
            return e;
        }
            

        return null;
    }

    public VehicleCore getVehicleCore() {
        return (VehicleCore)entityCore;
    }

    public void SpawnDriver() {
        print("Spawning Driver: " + getVehicleCore().crewCore.name);
        
        driverGO = Instantiate(getVehicleCore().crewCore.entityPrefab);
        driverGO.GetComponent<Unit>().EnterVehicle(this, 0);
        setAsPassenger(driverGO.GetComponent<Unit>(), 0);
    }

    public void SpawnPassengers(UnitCore core) {
        for (int i = 1; i < passengerPositions.Count; i++) {
            GameObject passengerGO = Instantiate(getVehicleCore().crewCore.entityPrefab);
            passengerGO.GetComponent<Unit>().EnterVehicle(this, i);
            setAsPassenger(passengerGO.GetComponent<Unit>(), i);

        }
    }

    public void setAsPassenger(Unit unit, int position = -1) {
        if (position == -1)
            position = getNextEmptySeat();

        passengers[position] = unit;
        unit.gameObject.transform.SetParent(this.transform, true);
        unit.transform.rotation = passengerPositions[position].rotation;

    }

    public void removePassenger(Unit unit, bool exitVehicle = true) {

        for (int i = 0; i < passengers.Length; i++) {
            if (passengers[i] == unit) {

                if(exitVehicle)
                    unit.ExitVehicle();

                if (unit.gameObject.transform.root == this.transform)
                    unit.gameObject.transform.SetParent(null, true);

                passengers[i] = null;
            }
                
        }
        
        
    }

    public Unit removeLastPassenger(bool exitVehicle = true) {

        for (int i = passengers.Length-1; i >= 0; i--) {
            if (getVehicleCore().driverPositions.Contains(i))
                continue;

            if (passengers[i] != null) {

                Unit temp = passengers[i];

                if (exitVehicle)
                    temp.ExitVehicle();

                if (temp.gameObject.transform.root == this.transform)
                    temp.gameObject.transform.SetParent(null, true);

                passengers[i] = null;
                return temp;
            }


        }
        return null;

    }

    public int getPassengerIndex(Unit unit) {
        for (int i = 0; i < passengers.Length; i++) {
            if (passengers[i] == unit) {
                return i;
            }

        }

        return -1;
    }

    public void VehicleTick() {
        UnitTick();
        
    }



}

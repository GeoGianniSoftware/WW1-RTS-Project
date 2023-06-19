using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Unit : Entity
{
    public NavMeshAgent NMA;
    public Animator Anim;
    
    public int ammoCount = 5;
    public int damage = 5;
    
    public bool isReloading = false;
    public bool isFiring = false;

    public List<Unit_Order> orderQueue = new List<Unit_Order>();
    public Unit_Order currentOrder = null;

    int enemyCount = 0;
    List<Entity> senses = new List<Entity>();
    List<Entity> nearbyEnemies = new List<Entity>();

    public Building currentGarrison = null;
    public Vehicle currentVehicle = null;
    public GameObject currentWeapon = null;

    //Logic Variables
    public float delayTimer = 0;

    public UnitCore getUnitCore() {
        return (UnitCore)entityCore;
    }
    
    public override void Interact(Entity entityInteracting) {

        if(CMD.checkBasicType(EntityType.BUILDING, entityInteracting) && ((BuildingCore)entityInteracting.entityCore).canBeStationed) {
            Building buildingToGarrison = (Building)entityInteracting;

            AddGarrisonOrder(buildingToGarrison);
            print("Attempting to Garrison: " + entityInteracting.name);
        }else if(entityInteracting.getEntityCore().TYPE == EntityType.VEHICLE && ((VehicleCore)entityInteracting.getEntityCore()).isTransportVehicle && ((Vehicle)entityInteracting).canCarryPassengers()) {
            print("Attempting to Enter: " + entityInteracting.name);
            AddEnterTransportOrder(((Vehicle)entityInteracting));
        }
        else {
            print("Interacting with " + entityInteracting.name);
        }
    }

    public bool isNavReady() {
        if(NMA != null && NMA.isActiveAndEnabled && NMA.isOnNavMesh) {
            return true;
        }

        return false;
    }


    Vector3 lastPathLocation = Vector3.zero;
    public bool SetUnitPath(Vector3 position, bool debug = false) {
        if (lastPathLocation == position && NMA.hasPath) {
            if (debug)
                print("Duplicate Path, Ignoring");

            return false;
        }
            

        NavMeshPath path = new NavMeshPath();
        NMA.CalculatePath(currentOrder.orderPosition, path);
        if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathComplete) {
            lastPathLocation = position;
            if (debug)
                print("Path Succesful!");
            NMA.path = path;
            return true;
        }
        else {
            if (debug)
                print("Path Error: " + path.status);
            return false;
        }
    }

    public int lastRouteID = -1;
    public void Move() {
        if(currentOrder != null && isNavReady()) {
            //Move
            if (currentOrder.orderType == Unit_Order.OrderTypes.MoveOrder) {
                if (currentGarrison != null)
                    ExitStation();

                SetUnitPath(currentOrder.orderPosition);
                if (Vector3.Distance(transform.position, currentOrder.orderPosition) < getUnitCore().size) {
                    //Arrived at Destination
                    clearCurrentOrder();
                    return;
                }

                
            }
            //Garrison
            if (currentOrder.orderType == Unit_Order.OrderTypes.GarrisonOrder) {
                Vector3 pos = CMD.CMND.getNearestNavPosition(currentOrder.orderTarget.transform.position, 5f);
                SetUnitPath(pos);
                if (Vector3.Distance(transform.position, currentOrder.orderPosition) < getUnitCore().size + ((BuildingCore)currentOrder.orderTarget.entityCore).stationOffsetDistance) {
                    //Arrived at Destination
                    
                    EnterStation();
                    clearCurrentOrder();
                    return;
                }
            }
            //Enter Vehicle
            if (currentOrder.orderType == Unit_Order.OrderTypes.EnterVehicleOrder) {
                Vehicle vic = null;
                if (!((Vehicle)currentOrder.orderTarget).canCarryPassengers()) {
                    clearCurrentOrder(true);
                    return;
                }
                else {
                    vic = (Vehicle)currentOrder.orderTarget;
                }
                Vector3 pos = CMD.CMND.getNearestNavPosition(currentOrder.orderTarget.transform.position, 5f);
                SetUnitPath(pos);
                if (Vector3.Distance(transform.position, currentOrder.orderPosition) < (getUnitCore().size + vic.getVehicleCore().size) && vic.canCarryPassengers()) {
                    //Arrived at Destination
                    EnterVehicle(vic);
                    clearCurrentOrder();
                    return;
                }
            }
            //Unload Vehicle
            if (currentOrder.orderType == Unit_Order.OrderTypes.UnloadVehiclePassengersOrder) {
                


                Vehicle vic = null;
                if (currentOrder.orderTarget.getEntityCore().TYPE == EntityType.VEHICLE && ((Vehicle)currentOrder.orderTarget).getVehicleCore().isTransportVehicle) {
                    vic = (Vehicle)currentOrder.orderTarget;
                    for (int i = 0; i < vic.passengers.Length; i++) {
                        if(!vic.getVehicleCore().driverPositions.Contains(i) && vic.passengers[i] != null) {
                            vic.removePassenger(vic.passengers[i]);
                        }
                    }
                }
                clearCurrentOrder();
                return;
            }
            //Load Supply
            if (currentOrder.orderType == Unit_Order.OrderTypes.LoadSupplyOrder && entityCore.TYPE == EntityType.VEHICLE) {
                print("SupplyRoute");
                Vector3 pos = CMD.CMND.getNearestNavPosition(currentOrder.orderTarget.transform.position, 5f);
                SetUnitPath(pos);
                if (Vector3.Distance(transform.position, currentOrder.orderPosition) < getUnitCore().size) {

                    Vehicle vic = gameObject.GetComponent<Vehicle>();

                    if(CMD.checkBasicType(EntityType.BUILDING, currentOrder.orderTarget, EntityType.PRODUCTION)) {
                        Building dropOff = (Building)currentOrder.orderTarget;
                        print("Arrived");

                        if (vic != null && vic.getVehicleCore().isCargoVehicle && vic.cargoList.Count > 0) {
                            int amt = vic.cargoList.Count;

                            for (int i = 0; i < amt; i++) {
                                if (dropOff.canCarryCargo()) {
                                    Entity cargo = vic.offloadCargo(vic.cargoList.Count - 1);
                                    cargo.GetComponent<Resource>().resourcePickup = false;
                                    print("CargoDropped: " + cargo.name);
                                    dropOff.loadCargo(cargo);

                                }
                            }


                        }
                    }else if (currentOrder.orderTarget.entityCore.TYPE == EntityType.VEHICLE) {
                        Vehicle dropOff = (Vehicle)currentOrder.orderTarget;
                        print("Arrived");

                        if (vic != null && vic.getVehicleCore().isCargoVehicle && vic.cargoList.Count > 0) {
                            int amt = vic.cargoList.Count;

                            for (int i = 0; i < amt; i++) {
                                if (dropOff.currentSupply < dropOff.getVehicleCore().maxSupplyStorage) {
                                    Entity cargo = vic.offloadCargo(vic.cargoList.Count - 1);
                                    Destroy(cargo.gameObject);
                                    dropOff.currentSupply++;
                                }
                            }


                        }
                    }
                    else if (currentOrder.orderTarget.entityCore.TYPE == EntityType.PRODUCTION) {
                        ProductionBuilding dropOff = (ProductionBuilding)currentOrder.orderTarget;
                        print("Arrived Production");

                        if (vic != null && vic.getVehicleCore().isCargoVehicle && vic.cargoList.Count > 0) {
                            int amt = vic.cargoList.Count;

                            for (int i = 0; i < amt; i++) {
                                if (dropOff.currentSupply < dropOff.getProductionCore().maxSupplyStorage) {
                                    Entity cargo = vic.offloadCargo(vic.cargoList.Count - 1);
                                    Destroy(cargo.gameObject);
                                    dropOff.currentSupply++;
                                }
                            }


                        }
                    }


                    int routeId = currentOrder.logicLink.routeID;
                    Entity nextDest = currentOrder.logicLink.origin;
                    clearCurrentOrder();
                    if (routeId >= 0) {
                        print("LogicLink");
                        AddUnloadSupplyOrder((Building)nextDest, routeId);
                    }
                    return;
                }
            }
            //Unload Supply
            if (currentOrder.orderType == Unit_Order.OrderTypes.UnloadSupplyOrder && entityCore.TYPE == EntityType.VEHICLE) {
                print("DesupplyRoute");
                Vector3 pos = CMD.CMND.getNearestNavPosition(currentOrder.orderTarget.transform.position, 5f);
                SetUnitPath(pos);
                if (Vector3.Distance(transform.position, currentOrder.orderPosition) < getUnitCore().size) {
                    Building pickup = (Building)currentOrder.orderTarget;
                    Vehicle vic = gameObject.GetComponent<Vehicle>();
                   
                    print("Arrived");

                    if (pickup != null && ((BuildingCore)pickup.entityCore).canHoldCargo && pickup.cargoList.Count > 0) {
                        int amt = pickup.cargoList.Count;

                        for (int i = 0; i < amt; i++) {
                            print("tried");
                            if (vic.canCarryCargo()) {
                                Entity cargo = pickup.offloadCargo(pickup.cargoList.Count - 1);
                                cargo.GetComponent<Resource>().resourcePickup = false;
                                print("CargoDropped: " + cargo.name);
                                vic.loadCargo(cargo);
                                
                            }
                        }


                    }

                    int routeId = currentOrder.logicLink.routeID;
                    Entity nextDest = currentOrder.logicLink.destination;
                    clearCurrentOrder();
                    if (routeId >= 0) {
                        print("LogicLink");
                        AddLoadSupplyOrder(nextDest, routeId);
                    }
                    return;
                }
            }
            //Transfer Passengers
            if (currentOrder.orderType == Unit_Order.OrderTypes.TransferPassengersOrder && entityCore.TYPE == EntityType.VEHICLE) {
                print("TransportRoute");
                Vector3 pos = CMD.CMND.getNearestNavPosition(currentOrder.orderTarget.transform.position, 5f);
                SetUnitPath(pos);
                if (Vector3.Distance(transform.position, currentOrder.orderPosition) < getUnitCore().size + currentOrder.orderTarget.entityCore.size) {

                    Vehicle vic = gameObject.GetComponent<Vehicle>();

                    if (currentOrder.orderTarget.entityCore.TYPE == EntityType.STATION) {
                        Station dropOff = (Station)currentOrder.orderTarget;
                        print("Arrived");

                        if (vic != null && vic.getVehicleCore().isTransportVehicle && vic.getPassengerSeatCount() > 0) {
                            int amt = vic.getPassengerSeatCount();

                            for (int i = 0; i < amt; i++) {
                                if (dropOff.canStationUnits()) {
                                    Unit passenger = vic.removeLastPassenger();

                                    if(passenger != null)
                                        passenger.AddGarrisonOrder(dropOff);

                                }
                            }


                        }
                    }


                    int routeId = currentOrder.logicLink.routeID;
                    Entity nextDest = currentOrder.logicLink.origin;
                    clearCurrentOrder();
                    if (routeId >= 0) {
                        print("LogicLink");
                        AddLoadPassengersOrder((Station)nextDest, routeId);
                    }
                    return;
                }
            }
            //Load Passengers
            if (currentOrder.orderType == Unit_Order.OrderTypes.LoadPassengersOrder && entityCore.TYPE == EntityType.VEHICLE) {
                print("TransportRoute");
                Vector3 pos = CMD.CMND.getNearestNavPosition(currentOrder.orderTarget.transform.position, 5f);
                SetUnitPath(pos);
                if (Vector3.Distance(transform.position, currentOrder.orderPosition) < getUnitCore().size + currentOrder.orderTarget.entityCore.size) {

                    Vehicle vic = gameObject.GetComponent<Vehicle>();

                    if (currentOrder.orderTarget.entityCore.TYPE == EntityType.STATION) {
                        Station pickup = (Station)currentOrder.orderTarget;
                        print("Arrived");

                        if (vic != null && vic.getVehicleCore().isTransportVehicle && vic.getEmptySeatCount() > 0) {
                            int amt = vic.getEmptySeatCount();

                            for (int i = 0; i < amt; i++) {
                                if (pickup.stationedUnits.Count > 0) {
                                    
                                    Unit passenger = pickup.removeLastUnitFromStation();

                                    if(passenger != null)
                                        passenger.AddEnterTransportOrder((Vehicle)this);

                                }
                            }


                        }
                    }


                    int routeId = currentOrder.logicLink.routeID;
                    Entity nextDest = currentOrder.logicLink.destination;
                    clearCurrentOrder();
                    if (routeId >= 0) {
                        print("LogicLink");
                        AddTransferPassengersOrder((Station)nextDest, routeId);
                    }
                    return;
                }
            }
        }
        
            

    }


    public void addOrder(Unit_Order orderToAdd, bool overrideBool = false) {
        if(currentOrder == null || overrideBool) {
            currentOrder = orderToAdd;
        }
        else {
            orderQueue.Add(orderToAdd);
        }
    }
    public void clearCurrentOrder(bool resetPath = false) {
        if(currentOrder != null) {
            currentOrder = null;
        }

        if (resetPath && NMA.isActiveAndEnabled) {
            NMA.ResetPath();
        }
        
    }
    public void clearOrderQueue() {
        if (orderQueue.Count > 0) {
            orderQueue.Clear();
        }
    }
    public virtual void MoveOrder(Vector3 dest) {
        if (isNavReady()) {
            addOrder(new Unit_Order(Unit_Order.OrderTypes.MoveOrder, dest), true);
        }
    }
    public void AddMoveOrder(Vector3 dest) {
        if (isNavReady()) {
            addOrder(new Unit_Order(Unit_Order.OrderTypes.MoveOrder, dest));
        }
    }
    public void AddGarrisonOrder(Building building) {
        if (isNavReady()) {
            addOrder(new Unit_Order(Unit_Order.OrderTypes.GarrisonOrder, building.transform.position, building), true);
        }
    }
    public void AddUnloadSupplyOrder(Building building, int logicLinkID) {
        if (isNavReady()) {
            addOrder(new Unit_Order(Unit_Order.OrderTypes.UnloadSupplyOrder, building.transform.position, building, CMD.CMND.cmd_logistics.playerLogisticLinks[logicLinkID]), true);
        }
    }
    public void AddLoadSupplyOrder(Entity entity, int logicLinkID) {
        if (isNavReady()) {
            print(logicLinkID);
            addOrder(new Unit_Order(Unit_Order.OrderTypes.LoadSupplyOrder, entity.transform.position, entity, CMD.CMND.cmd_logistics.playerLogisticLinks[logicLinkID]), true);
        }
    }

    public void AddTransferPassengersOrder(Station building, int logicLinkID) {
        if (isNavReady()) {
            addOrder(new Unit_Order(Unit_Order.OrderTypes.TransferPassengersOrder, building.transform.position, building, CMD.CMND.cmd_logistics.playerLogisticLinks[logicLinkID]), true);
        }
    }
    public void AddLoadPassengersOrder(Station entity, int logicLinkID) {
        if (isNavReady()) {
            print(logicLinkID);
            addOrder(new Unit_Order(Unit_Order.OrderTypes.LoadPassengersOrder, entity.transform.position, entity, CMD.CMND.cmd_logistics.playerLogisticLinks[logicLinkID]), true);
        }
    }

    public void AddEnterTransportOrder(Vehicle entity) {
        if (isNavReady()) {
            addOrder(new Unit_Order(Unit_Order.OrderTypes.EnterVehicleOrder, entity.transform.position, entity), true);
        }
    }

    public void AddUnloadTransportOrder(Vehicle entity) {
        if (isNavReady()) {
            print("Added Unload Passengers Order");
            addOrder(new Unit_Order(Unit_Order.OrderTypes.UnloadVehiclePassengersOrder, entity.transform.position, entity), true);
        }
    }

    private void Awake() {
        NMA = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();

        
    }

    public void UnitTick() {
        if(NMA != null && defaultHeight == -1 && NMA.baseOffset != -1)
            defaultHeight = NMA.baseOffset;


        EntityTick();
        if (currentOrder == null && orderQueue.Count > 0) {
            currentOrder = orderQueue[0];
            orderQueue.RemoveAt(0);
        }

        if (isNavReady()) {
            NMA.speed = getUnitCore().speed;
            if(Anim != null)
            Anim.SetFloat("speed", NMA.velocity.magnitude);

        }

        if(delayTimer > 0)
            delayTimer -= Time.deltaTime;
    }

    private void Update() {
        UnitTick();

        if (alive) {
            UnitUI();
            CalculateSenses();
            EngageTargets();
            Move();
        }
    }

    public void EnterVehicle(Vehicle vehicle, int position = -1) {
        if(position == -1) {
            position = vehicle.getNextEmptySeat();
        }

        if (currentVehicle == null) {
            NMA.enabled = false;
            
            transform.rotation = vehicle.passengerPositions[position].rotation;
            transform.position = vehicle.passengerPositions[position].position;
            if (vehicle.getVehicleCore().driverPositions.Contains(position))
                Anim.SetBool("driving", true);
            else {
                Anim.SetBool("sitting", true);
            }
            NMA.baseOffset = vehicle.driverOffset;
            active = false;
            currentVehicle = vehicle;
            vehicle.setAsPassenger(this, position);
        }
    }
    public void ExitVehicle() {
        if(currentVehicle != null) {
            currentVehicle.removePassenger(this, false);
            currentVehicle = null;

            Anim.SetBool("driving", false);
            Anim.SetBool("sitting", false);
            resetUnitHeight();
            NMA.enabled = true;
            transform.position = CMD.CMND.getNearestNavPosition(transform.position, 5f);
            NMA.nextPosition = CMD.CMND.getNearestNavPosition(transform.position, 5f);
            NMA.ResetPath();
            active = true;
        }
        
    }

    float defaultHeight = -1;
    public void setUnitHeight(float y) {

        NMA.baseOffset = y;
    }

    public void resetUnitHeight() {
        NMA.baseOffset = defaultHeight;
    }

    public void EnterStation() {
        currentGarrison = (Building)currentOrder.orderTarget;
        if (currentGarrison.addUnitToStation(this)) {
            int index = currentGarrison.stationedUnits.Count-1;
            Vector3 garrisonPos = currentGarrison.stationPositions[index].position;
            NMA.nextPosition = garrisonPos;
            NMA.ResetPath();
            NMA.velocity = Vector3.zero;
            setUnitHeight(-(transform.position.y - garrisonPos.y));
        }
        else {
            currentGarrison = null;
        }
        

        
    }
    public void ExitStation() {
        if (currentGarrison != null) {
            currentGarrison.removeUnitFromStation(this);
            currentGarrison = null;
            resetUnitHeight();
        }
    }

    void UnitUI() {
        if (infoUI != null && (!active || currentVehicle != null)) {
            infoUI.gameObject.SetActive(false);
        }
        else if(infoUI != null){
            infoUI.gameObject.SetActive(true);
        }

    }


    #region Health & Combat
    void EngageTargets() {
        if (nearbyEnemies.Count > 0) {
            addOrder(new Unit_Order(Unit_Order.OrderTypes.AttackOrder, nearbyEnemies[0].transform.position, nearbyEnemies[0]), true);
            NMA.isStopped = true;
        }

        if (currentOrder != null && currentOrder.orderType == Unit_Order.OrderTypes.AttackOrder) {
            Entity currentTarget = currentOrder.orderTarget;


            if (Vector3.Distance(transform.position, currentTarget.transform.position) > getUnitCore().range || currentTarget.currentHealth <= 0) {
                currentTarget = null;
                clearCurrentOrder();
                return;
            }


            LookAt(currentTarget.transform.position);

            if (ammoCount > 0 && !isReloading && delayTimer <= 0) {
                StartFire();
            }
            else {
                isFiring = false;

                if(ammoCount == 0)
                    Reload();
            }
        }
        else {
            isFiring = false;
        }
    }

    public void DropWeapon() {

        currentWeapon.GetComponent<Rigidbody>().isKinematic = false;
        currentWeapon.transform.SetParent(null);
    }

    public override void Die() {
        base.Die();
        if (NMA.enabled)
            NMA.enabled = false;

        if (Anim != null)
            Anim.enabled = false;

        isFiring = false;
        isReloading = false;
        clearCurrentOrder();
        clearOrderQueue();

        if(GetComponentInChildren<RagdollScript>() != null) {
            GetComponentInChildren<RagdollScript>().EnableRagdoll(lastDamageTaken);
        }

        if(currentWeapon != null) {
            if (Random.Range(0, 100) != 0) {
                DropWeapon();
            }
                
        }

    }

    public float senseTimer = 0;
    public void CalculateSenses() {
       if(senseTimer <= 0) {
            senses = new List<Entity>();
            senses.AddRange(getEnemiesInRange());
            nearbyEnemies = new List<Entity>();

            foreach (Entity e in senses) {
                if (e.currentHealth > 0) {
                    nearbyEnemies.Add(e);
                }
            }
            enemyCount = nearbyEnemies.Count;
            
            senseTimer = CMD.CMND.cmd_pathfinding.AISensesTickInterval;
        }
        else {
            senseTimer -= Time.deltaTime;
        }


        
    }

    public float getRange() {
        float offset = 0;
        if (currentGarrison != null)
            offset += ((BuildingCore)currentGarrison.getBuildingCore()).garrison_rangeOffset;

        return getUnitCore().range + offset;
    }

    public float getHitChance() {
        float offset = 0;
        if (currentGarrison != null)
            offset += ((BuildingCore)currentGarrison.getBuildingCore()).garrison_hitchanceOffset;

        return getUnitCore().baseHitChance + offset;
    }

    public override void TakeDamage(Damage dmg) {
        float chance = Random.Range(0f, 1f);
        if (chance > 1f - getHitChance()) {
            lastDamageTaken = dmg;
            currentHealth -= dmg.amount;
        }
    }

    void LookAt(Vector3 target) {
        Vector3 fin = target + transform.forward + (transform.right);
        fin.y = transform.position.y;
        transform.LookAt(fin);
        
    }

    public void FinishReload() {
        isReloading = false;
        ammoCount = 5;
    }

    public void Reload() {
        if (!isReloading) {
            Anim.SetTrigger("reloading");
            isReloading = true;
        }
    }

    public bool hasValidOrder() {
        if (currentOrder != null) {
            return true;
        }
            

        return false;
    }

    public virtual bool canFire() {
        if(currentHealth > 0 && hasValidOrder() && ammoCount > 0 && currentOrder.orderTarget != null) {
            return true;
        }

        return false;
    }

    public virtual void Fire() {
        if (canFire()) {
            delayTimer += getUnitCore().fireRate;
            float hitChance = Random.Range(0f, 1f);
            if(hitChance > 1f - getUnitCore().accuracy)
                currentOrder.orderTarget.SendMessage("TakeDamage", new Damage(damage), SendMessageOptions.DontRequireReceiver);
        }
        else {
            return;
        }
        ammoCount--;
        
    }

    public void StartFire() {
        isFiring = true;

        if(Anim != null) {
            Anim.SetTrigger("fire");
            Anim.SetFloat("firingSpeed", getUnitCore().fireRate);
        }
        if(NMA != null)
        NMA.speed = getUnitCore().speed / 2;
    }

    public List<Entity> getEnemiesInRange() {
        List<Entity> returnList = new List<Entity>();
        foreach (Entity e in getEntitiesInRange()) {
            if(e.owner != owner && e != this) {
                returnList.Add(e);
            }
        }

        return returnList;
    }

    public List<Entity> getEntitiesInRange() {
        List<Entity> returnList = new List<Entity>();

        Collider[] cols = Physics.OverlapSphere(transform.position, getRange());
        foreach(Collider c in cols) {
            if (c.GetComponentInChildren<Entity>()) {

                returnList.Add(c.GetComponentInChildren<Entity>());
            
            }
        }

        return returnList;
    }
    #endregion


}

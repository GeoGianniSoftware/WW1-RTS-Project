using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum buildingType
{
    Basic,
    Production,
    Station,
    Static,
    Factory
}


[System.Serializable]
public class Building : Entity
{

    public bool placed = false;

    [System.NonSerialized]
    public List<Building> connectedBuildings = new List<Building>();


    public List<Transform> stationPositions = new List<Transform>();
    public List<Unit> stationedUnits = new List<Unit>();

    public List<Transform> cargoPositions;
    public List<Entity> cargoList;
    public int currentSupply;


    private void Update() {
        BuildingTick();
        
    }



    public virtual void PlaceBuilding(BuildingCore core, bool skip = false) {

        print("a: " + core.TYPE + " " + core.continousDefense);

        if (core.continousDefense && !Input.GetKey(KeyCode.LeftControl) && !skip) {
            print("ah");
                Building lastPlaced = CMD.CMND.cmd_building.lastPlacedBuildingType;

                if (lastPlaced != null) {
                print("ahh");
                addBuildingConnection(lastPlaced);

                if (core.coreType == buildingType.Static) {
                    print("ah1");
                    Vector3 relativePos = lastPlaced.gameObject.transform.position - transform.position;

                    Quaternion finRot = Quaternion.LookRotation(relativePos, Vector3.up);

                    finRot.eulerAngles = new Vector3(finRot.eulerAngles.x, finRot.eulerAngles.y + 90f, finRot.eulerAngles.z);
                    if (core.flipBackfill) {
                        finRot.eulerAngles = new Vector3(finRot.eulerAngles.x, finRot.eulerAngles.y + 90f + 90f, finRot.eulerAngles.z);
                    }

                    transform.rotation = finRot;
                }

                if (lastPlaced.entityCore.TYPE == EntityType.STATICDEFENSE) {
                    print("ah2");
                    Vector3 relativePos = lastPlaced.gameObject.transform.position - transform.position;

                    Quaternion finRot = Quaternion.LookRotation(relativePos, Vector3.up);

                    finRot.eulerAngles = new Vector3(finRot.eulerAngles.x, finRot.eulerAngles.y + 90f, finRot.eulerAngles.z);

                    lastPlaced.transform.rotation = finRot;
                }

                    


                lastPlaced.addBuildingConnection(this);

                if (core.coreType == buildingType.Static)
                    lastPlaced.GetComponent<Building>().Backfill();

                if (core.coreType == buildingType.Station)
                    lastPlaced.GetComponent<Building>().Backfill(-1);

            }
        }

        if (GetComponent<NavMeshObstacle>())
            GetComponent<NavMeshObstacle>().enabled = true;

        placed = true;
    }

    public bool addUnitToStation(Unit unitToAdd) {
        if (readyToStation(unitToAdd)) {
            stationedUnits.Add(unitToAdd);
            return true;
        }

        return false;
    }



    public void removeUnitFromStation(Unit unitToRemove) {
        for (int i = 0; i < stationedUnits.Count; i++) {
            if (stationedUnits[i] == unitToRemove)
                stationedUnits.RemoveAt(i);
        }
    }

    public Unit removeLastUnitFromStation() {
        for (int i = stationedUnits.Count-1; i >= 0; i--) {
            if (stationedUnits[i] != null) {
                Unit temp = stationedUnits[i];
                stationedUnits.RemoveAt(i);
                return temp;
            }
                
        }

        return null;
    }

    public bool canStationUnits() {
        if (stationedUnits.Count < stationPositions.Count) {
            return true;
        }

        return false;
    }

    public bool readyToStation(Unit unitToCheck) {
        if (stationedUnits.Count < stationPositions.Count && !stationedUnits.Contains(unitToCheck)) {
            return true;
        }

        return false;
    }

    public BuildingCore getBuildingCore() {
       if(entityCore.TYPE == EntityType.STATION) {
            return (StationCore)entityCore;
       }else if (entityCore.TYPE == EntityType.STATICDEFENSE) {
            return (DefenseCore)entityCore;
        }
        if (entityCore.TYPE == EntityType.PRODUCTION) {
            return (ProductionBuildingCore)entityCore;
        }
        else {
            return (BuildingCore)entityCore;
        }
    }



    public virtual void addBuildingConnection(Building building) {
        connectedBuildings.Add(building);
    }

    public void Backfill(int countOffset = 0) {
        Building lastBuilding = CMD.CMND.cmd_building.lastPlacedBuildingType;
        
        if (lastBuilding.getBuildingCore().coreType == buildingType.Static || lastBuilding.getBuildingCore().coreType == buildingType.Station) {
            int offset = 1;
            float buildingDistance = -1;
            if (lastBuilding.getBuildingCore().coreType == buildingType.Static) {
                buildingDistance = ((DefenseCore)entityCore).continousOffset;
                
            }

            if (lastBuilding.getBuildingCore().coreType == buildingType.Station) {
                buildingDistance = CMD.CMND.cmd_building.lastPlacedStatic.continousOffset;
               
            }

            GameObject test = null;
            if (((BuildingCore)lastBuilding.entityCore).replaceOnExtend && lastBuilding.connectedBuildings.Count > 1) {

                test = Instantiate(((BuildingCore)lastBuilding.entityCore).replaceExtend[0].entityPrefab, lastBuilding.transform.position, lastBuilding.transform.rotation);
                test.name = "ExtendedFiller"+lastBuilding.entityCore.Name;
                Building build = test.GetComponent<Building>();
               
                Destroy(lastBuilding.gameObject);
                lastBuilding.connectedBuildings[0] = build;
                build.PlaceBuilding((BuildingCore)lastBuilding.entityCore, true);
            }



            float fillDistance = Vector3.Distance(transform.position, connectedBuildings[0].transform.position);
            int cloneCount = (int)(fillDistance / 2) + countOffset;


            Vector3 relativePos = connectedBuildings[0].transform.position - transform.position;
            if (connectedBuildings.Count == 2) {
                fillDistance = Vector3.Distance(transform.position, connectedBuildings[1].transform.position);
                cloneCount = (int)(fillDistance / 2) + countOffset;
                relativePos = connectedBuildings[1].transform.position - transform.position;
            }


            if((fillDistance / 2)-cloneCount < .5f) {
                cloneCount--;
                buildingDistance += .1f;
            }


            Quaternion finRot = Quaternion.LookRotation(relativePos, Vector3.up);

            finRot.eulerAngles = new Vector3(finRot.eulerAngles.x, finRot.eulerAngles.y + 90f, finRot.eulerAngles.z);

            
            GameObject fillToSpawn = entityCore.entityPrefab;

            if (getBuildingCore().backfillEntityCore != null) {
                fillToSpawn = getBuildingCore().backfillEntityCore.entityPrefab;
            }

            if (getBuildingCore().coreType == buildingType.Station) {
                fillToSpawn = CMD.CMND.cmd_building.lastPlacedStatic.entityPrefab;
            }

            


            for (int i = 0; i < cloneCount; i++) {
                GameObject fillGO = Instantiate(fillToSpawn, transform.position + (((relativePos.normalized) * ((i + 1) * buildingDistance)) * offset), finRot);
                Building build = fillGO.GetComponent<Building>();

                if (build != null) {
                    build.PlaceBuilding(build.getBuildingCore(), true);
                }

            }

            if (lastBuilding.getBuildingCore().replaceOnExtend && lastBuilding.connectedBuildings.Count > 1) {
                Building build = test.GetComponent<Building>();
                foreach (Collider col in Physics.OverlapSphere(test.transform.position, 2f)) {
                    if (col.GetComponent<Building>() && col.GetComponent<Building>().getBuildingCore() == lastBuilding.getBuildingCore().backfillEntityCore) {
                        Destroy(col.gameObject);
                    }
                }
            }


        }
    }

    #region Cargo

    public bool canCarryCargo() {
        if (getBuildingCore().canHoldCargo && cargoList.Count < cargoPositions.Count)
            return true;

        return false;
    }

    public int loadCargo(Entity cargo) {
        if (canCarryCargo() && cargo.entityCore.TYPE != EntityType.UNIT && !cargoList.Contains(cargo)) {
            cargoList.Add(cargo);
            cargo.transform.position = cargoPositions[cargoList.Count - 1].position;
            cargo.transform.SetParent(cargoPositions[cargoList.Count - 1], true);
            cargo.transform.localRotation = Quaternion.Euler(Vector3.zero);
            cargo.isCargo = true;
            if (cargo.GetComponent<Resource>())
                cargo.GetComponent<Resource>().resourcePickup = false;

            print("load");
            return cargoList.Count - 1;
        }
        return -1;
    }

    public Entity offloadCargo(int index) {
        if (index < cargoList.Count) {
            Entity e = cargoList[index];
            e.isCargo = false;
            cargoList.RemoveAt(index);
            return e;
        }


        return null;
    }

    #endregion

    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float f = Mathf.Atan2(v3.z - v1.z, v3.x - v1.x) - Mathf.Atan2(v2.z - v1.z, v2.x - v1.x);
        return f;
    }

    public void BuildingTick() {
        EntityTick();

    }

   

}

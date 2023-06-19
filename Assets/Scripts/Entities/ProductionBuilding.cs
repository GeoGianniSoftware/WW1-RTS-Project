using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building
{
    public int currentProductionIndex;
    public float buildingTimer;
    public Transform productionSpawnPoint;
    public float currentProductionPoints = 0f;
    public Vector3 rallypoint = new Vector3(-500, -500);
    public GameObject rallyflag = null;
    // Start is called before the first frame update

    public ProductionBuildingCore getProductionCore() {
        return (ProductionBuildingCore)entityCore;
    }

    private void OnValidate() {

    }



    // Update is called once per frame
    void Update()
    {
        if (placed) {
            if (buildingTimer > 0 && currentSupply >= getProductionCore().productionList[currentProductionIndex].supplyCost) {
                buildingTimer -= Time.deltaTime;
            }
            if(buildingTimer <= 0 && currentSupply >= getProductionCore().productionList[currentProductionIndex].supplyCost) {
                ProductionTick();
                buildingTimer = 1f;
            }

            if(cargoList.Count > 0) {
                Destroy(cargoList[cargoList.Count - 1].gameObject);
                cargoList.RemoveAt(cargoList.Count - 1);
            }
        }


        

        if (selected) {

            if (Input.GetMouseButtonDown(1)) {
                setRallyAtMouse();
            }

            if (rallyflag == null && rallypoint != new Vector3(-500,-500)) {
                rallyflag = Instantiate(Resources.Load<GameObject>("Misc/RallyFlag"));
                rallyflag.transform.position = rallypoint;
            }
            else if(rallyflag != null && rallyflag.transform.position != rallypoint && rallypoint != new Vector3(-500, -500)) {
                rallyflag.transform.position = rallypoint;
            }
        }
        else {
            if (rallyflag != null)
                Destroy(rallyflag);
        }
          
    }

    public void setRallyAtMouse() {
        CMD.CMDHitRay ray = CMD.CMND.RaycastMousePositionOnTerrain();
        if (ray != null)
            rallypoint = ray.posHit;
    }

    public override void SelectObject() {
        base.SelectObject();

    }
    public override void DeselectObject() {
        base.DeselectObject();
        CMD.CMND.cmd_building.DeactivateBuildingContextMenu(this);
    }

    public void setCurrentProduction(int index) {
        currentProductionPoints = 0;
        if(index < getProductionCore().productionList.Count)
            currentProductionIndex = index;
    }


    void ProductionTick() {
        if(getCurrentProductionCore() == null) {
            return;
        }


        if (getProductionCore().productionList.Count > 0) {
            currentProductionPoints += getProductionRate();


            if(currentProductionPoints >= getCurrentProductionCore().productionCost) {
                OutputProduction();
            }

        }
    }

    public float getProductionRate() {
        return 1 * getProductionCore().productionPerSecond;
    }



    public float getCurrentProductionPercentage() {
        return currentProductionPoints / getCurrentProductionCore().productionCost;
    }
    public float getTimeTillProductionFinish() {
        return buildingTimer;
    }

    void OutputProduction() {
        Entity e = CMD.CMND.SpawnEntity(getCurrentProductionCore(), productionSpawnPoint.transform.position, productionSpawnPoint.transform.eulerAngles.y);
        currentProductionPoints -= getCurrentProductionCore().productionCost;
        currentSupply -= getCurrentProductionCore().supplyCost;


        if (rallypoint != new Vector3(-500, -500)) {
            if (e.entityCore.TYPE == EntityType.UNIT || e.entityCore.TYPE == EntityType.VEHICLE) {
                ((Unit)e).MoveOrder(rallypoint);
            }
        }
        
    }

    public EntityCore getCurrentProductionCore() {
        if(currentProductionIndex >= 0 && getProductionCore().productionList[currentProductionIndex] != null) {
            return getProductionCore().productionList[currentProductionIndex];
        }

        return null;
    }
}

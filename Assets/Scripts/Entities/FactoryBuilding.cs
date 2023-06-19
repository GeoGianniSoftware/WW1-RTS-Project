using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryBuilding : Building
{

    public float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update() {
        BuildingTick();
        FactoryTick();
    }


    void FactoryTick() {
        if(timer <= 0) {
            FactoryOutput();
        }
        else {
            timer -= Time.deltaTime;
        }
        
    }

    void FactoryOutput() {
        FactoryBuildingCore core = getFactoryCore();

        for (int i = 0; i < core.productionAmount; i++) {
            if (getBuildingCore().coreType == buildingType.Factory && cargoList.Count < core.maxStorageAmount) {
                Entity cargo = Instantiate(core.productionType.entityPrefab, cargoPositions[0]).GetComponent<Entity>();
                cargo.transform.localPosition = Vector3.zero;
                cargoList.Add(cargo);
                cargo.isCargo = true;
                if (cargo.GetComponent<Resource>())
                    cargo.GetComponent<Resource>().resourcePickup = false;

            }
        }

        timer += core.productionTime;
    }

    FactoryBuildingCore getFactoryCore() {
        return (FactoryBuildingCore)entityCore;
    }
}

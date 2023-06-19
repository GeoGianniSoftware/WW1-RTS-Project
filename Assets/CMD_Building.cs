using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMD_Building : CMDScript
{
    
    public List<BuildingCore> buildingList = new List<BuildingCore>();
    public List<BuildingCore> defenselist = new List<BuildingCore>();
    public List<BuildingCore> stationList = new List<BuildingCore>();

    public int currentBuildingIndex;
    public bool isBuilding = false;

    public float currentRotation = 0f;

    public Building lastPlacedBuildingType = null;
    public GameObject lastPlacedBuildingGO = null;
    public DefenseCore lastPlacedStatic = null;


    public List<ProductionBuilding> currentProductionBuildings = new List<ProductionBuilding>();
    public List<BuildingContextUI> currentProductionBuildingsUI = new List<BuildingContextUI>();

    public GameObject ghostObject;
    Vector3 mousePos;

    private void Start() {
        lastPlacedStatic = Resources.Load<DefenseCore>("Army/Defenses/Cores/BarbedWire");
        PopulateBuildingLists();
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R) || Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0) {
            int dir = 1;
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
                dir = -1;

            IncrementRotation(dir);
        }

       

        if (isBuilding) {
            Vector3 terrainPos = Vector3.zero;
            if(CMND.currentMouseObjectInformation != null) {
                terrainPos = CMND.currentMouseObjectInformation.posHit;
            }
            else {
                return;
            }



            if (ghostObject == null) {
                GameObject temp = Instantiate(getCurrentBuilding(), terrainPos, getCurrentRotation());
                if (temp.GetComponent<UnityEngine.AI.NavMeshObstacle>())
                    temp.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
                ghostObject = temp;
            }
            else {
                ghostObject.transform.position = terrainPos;
                ghostObject.transform.rotation = getCurrentRotation();
            }


            if (Input.GetMouseButtonDown(0) && !CMD_UI.IsPointerOverUIObject()) {
                SpawnBuilding(CMND.currentMouseObjectInformation.posHit);
            }

        }
        else {
            if(ghostObject != null) {
                Destroy(ghostObject);
            }
        }

    }

    public void ActivateBuildingContextMenu(ProductionBuilding building) {
        if (currentProductionBuildings.Contains(building))
            return;
        print("Activating Building UI");

        GameObject ui = Instantiate(Resources.Load<GameObject>("UI/BuildingContextUI"), building.transform.position + (Vector3.up*10f), Quaternion.identity);
        ui.GetComponent<Canvas>().worldCamera = Camera.main;
        ui.GetComponent<BuildingContextUI>().Activate(building);
        currentProductionBuildings.Add(building);
        currentProductionBuildingsUI.Add(ui.GetComponent<BuildingContextUI>());
    }

    public void DeactivateBuildingContextMenu(ProductionBuilding building) {
        int index = -1;

        for (int i = 0; i < currentProductionBuildings.Count; i++) {
            if(building == currentProductionBuildings[i]) {
                index = i;
                break;
            }
        }

        if(index != -1) {
            Destroy(currentProductionBuildingsUI[index].gameObject);
            currentProductionBuildingsUI.RemoveAt(index);
            currentProductionBuildings.RemoveAt(index);
        }
        

    }

    public void toggleBuildMode() {
        toggleBuildMode(!isBuilding);

    }

    public void toggleBuildMode(bool set) {
        isBuilding = set;


    }
    void PopulateBuildingLists() {
        foreach(GameObject g in Resources.LoadAll<GameObject>("Army/Buildings")) {
            AddBuildingToList(0, g);
        }
        foreach (GameObject g in Resources.LoadAll<GameObject>("Army/Defenses")) {
            AddBuildingToList(1, g);
        }
        foreach (GameObject g in Resources.LoadAll<GameObject>("Army/Stations")) {
            AddBuildingToList(2, g);
        }



    }

    public void AddBuildingToList(int index, GameObject go) {
        EntityCore core = go.GetComponent<Entity>().entityCore;

        if (!((BuildingCore)core).canBeBuilt)
            return;

        switch (index) {
            default:
                buildingList.Add((BuildingCore)core);
                break;
            case 1:
                defenselist.Add((DefenseCore)core);
                break;
            case 2:
                stationList.Add((StationCore)core);
                break;
        }

    }

    public void ClearGhostObject() {
        if(ghostObject != null)
            Destroy(ghostObject);
    }

    public void IncrementRotation(int dir = 1) {
        currentRotation += 22.5f * dir;
    }

    public Quaternion getCurrentRotation() {
        return Quaternion.Euler(new Vector3(0, currentRotation, 0));
    }


    public GameObject getCurrentBuilding() {

        return getCurrentBuildList()[currentBuildingIndex].entityPrefab;
        
    }

    public List<BuildingCore> getCurrentBuildList() {
        switch (CMND.cmd_ui.currentBuildingMenuIndex) {
            default:
                return buildingList;
            case 1:
                return defenselist;
            case 2:
                return stationList;
        }
    }

    public void SpawnBuilding(Vector3 pos) {
        if (pos == Vector3.zero)
            return;

        GameObject buildingSpawned = Instantiate(getCurrentBuilding(), pos, getCurrentRotation());
         
        if (buildingSpawned.GetComponent<Building>()) {
            Building building = buildingSpawned.GetComponent<Building>();

            buildingSpawned.GetComponent<Building>().PlaceBuilding((BuildingCore)building.entityCore);
            lastPlacedBuildingType = buildingSpawned.GetComponent<Building>();
            if(lastPlacedBuildingType.getBuildingCore().coreType == buildingType.Static) {
                lastPlacedStatic = (DefenseCore)lastPlacedBuildingType.getBuildingCore();
            }
        }
            

        lastPlacedBuildingGO = buildingSpawned;
    }

    public void setBuildingIndex(int index) {
        if(index < getCurrentBuildList().Count) {
            currentBuildingIndex = index;
            ClearGhostObject();
        }

    }

    public void setBuildingIndexLock(int index) {
        print("DING: "+index);
        currentBuildingIndex = index;
        ClearGhostObject();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CMD : MonoBehaviour
{
    public static CMD CMND;


    List<CMDScript> cmd_scripts = new List<CMDScript>();

    [HideInInspector] public CMD_PlayerData cmd_playerData;
    [HideInInspector] public CMD_Research cmd_research;
    [HideInInspector] public CMD_MapManager cmd_mapmanager;
    [HideInInspector] public CMD_Units cmd_units;
    [HideInInspector] public CMD_SelectionManager cmd_selectionmanager;
    [HideInInspector] public CMD_UI cmd_ui;
    [HideInInspector] public CMD_Building cmd_building;
    [HideInInspector] public CMD_Pathfinding cmd_pathfinding;
    [HideInInspector] public CMD_OrderTools cmd_ordertools;
    [HideInInspector] public CMD_Logistics cmd_logistics;
    public static Color purpeColor = new Color(0.4053566f, 0.1016732f, 0.4811321f, 1f);

    public bool debug_DrawCursorPosOnTerrain = true;
    public CMDHitRay currentMouseObjectInformation;

    // Start is called before the first frame update
    void Start()
    {
        AssignScripts();
    }

    public Vector3 getNearestNavPosition(Vector3 pos, float dist) {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos, out hit, dist, NavMesh.AllAreas)) {
            Debug.DrawLine(pos, hit.position);
            

            Vector3 dir = hit.position - pos;

            Vector3 finalPos = hit.position + (dir * 2);
            Debug.DrawLine(finalPos, finalPos + Vector3.up, Color.red);

            return finalPos;
        }

        return -pos;
    }

    private void OnValidate() {
        AssignScripts();
    }
    public void AssignScripts() {
        CMND = this;


        cmd_playerData = GetComponentInChildren<CMD_PlayerData>();
        cmd_building = GetComponentInChildren<CMD_Building>();
        cmd_research = GetComponentInChildren<CMD_Research>();
        cmd_mapmanager = GetComponentInChildren<CMD_MapManager>();
        cmd_units = GetComponentInChildren<CMD_Units>();
        cmd_selectionmanager = GetComponentInChildren<CMD_SelectionManager>();
        cmd_ui = GetComponentInChildren<CMD_UI>();
        cmd_pathfinding = GetComponentInChildren<CMD_Pathfinding>();
        cmd_ordertools = GetComponentInChildren<CMD_OrderTools>();
        cmd_logistics = GetComponentInChildren<CMD_Logistics>();

        cmd_scripts.AddRange(GetComponentsInChildren<CMDScript>());
        foreach (CMDScript s in cmd_scripts) {
            s.setCMND(this);
        }
    }

    public RaycastHit RaycastMousePosition() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            return hit;
        }

        return hit;
    }

    public Entity SpawnEntity(EntityCore entityToSpawn, Vector3 position, float rotation = 0f) {
        if((UnitCore)entityToSpawn != null) {
            return cmd_units.SpawnUnit((UnitCore)entityToSpawn, position, rotation);
        }
        else {

            return Instantiate(entityToSpawn.entityPrefab, position, Quaternion.Euler(new Vector3(0, rotation, 0))).GetComponent<Entity>();
        }


    }

    public CMDHitRay RaycastMousePositionOnTerrain() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        RaycastHit[] hits = Physics.RaycastAll(ray, 3000f, 1 << 6);
        for (int i = 0; i < hits.Length; i++) {
            if(!hits[i].transform.CompareTag("Water"))
                return new CMDHitRay(hits[i].transform.gameObject, hits[i].point);
        }

        return null;
    }

    public static bool checkBasicType(EntityType type, Entity e, EntityType excludeType = EntityType.BASIC) {

        if (excludeType != EntityType.BASIC && e.entityCore.TYPE == excludeType)
            return false;

        switch (type) {

            case EntityType.BUILDING:
                return (e.entityCore.TYPE == EntityType.BUILDING || e.entityCore.TYPE == EntityType.FACTORY || e.entityCore.TYPE == EntityType.STATION || e.entityCore.TYPE == EntityType.PRODUCTION || e.entityCore.TYPE == EntityType.STATICDEFENSE);
            case EntityType.UNIT:
                return (e.entityCore.TYPE == EntityType.VEHICLE || e.entityCore.TYPE == EntityType.UNIT);
            default:
                print("No case for: " + type.ToString());
                return false;
        }

    }

    public static bool CalculateLobAngle(Vector3 origin, Vector3 target, float projectileVelocity, out float trajectoryAngle) {
        float actualDistance = Vector3.Distance(origin, target);

        float v = projectileVelocity;
        float g = Physics.gravity.y;
        float x = actualDistance;
        float y = 0;

        float v2 = v * v;
        float v4 = v * v * v * v;

        float gx2 = g * x * x;
        float yv2 = 2 * y * v * v;
        float gx = g * x;

        float res = Mathf.Sqrt(v4 - g * (gx2 + yv2));
        float res1 = v2 + res;
        float res2 = res1 / gx;

        trajectoryAngle = Mathf.Atan(res2) * 180 / Mathf.PI;

        if (float.IsNaN(trajectoryAngle)) {
            trajectoryAngle = 0;
            return false;
        }

        return true;
    }

    public static bool CalculateTrajectory(float TargetDistance, float ProjectileVelocity, out float CalculatedAngle) {
        CalculatedAngle = 0.5f * (Mathf.Asin((-Physics.gravity.y * TargetDistance) / (ProjectileVelocity * ProjectileVelocity)) * Mathf.Rad2Deg);
        if (float.IsNaN(CalculatedAngle)) {
            CalculatedAngle = 0;
            return false;
        }
        return true;
    }

    [System.Serializable]
    public class CMDHitRay
    {
        public GameObject objHit;
        public Vector3 posHit;

        public CMDHitRay(GameObject g, Vector3 p) {
            objHit = g;
            posHit = p;
        }
    }


    private void OnDrawGizmos() {
        if(debug_DrawCursorPosOnTerrain && RaycastMousePositionOnTerrain() != null)
            Gizmos.DrawCube(RaycastMousePositionOnTerrain().posHit, Vector3.one);
    }
    private void Update() {
        currentMouseObjectInformation = RaycastMousePositionOnTerrain();
    }
}

public class CMDScript: MonoBehaviour
{
    public CMD CMND;

    public void setCMND(CMD toSet) {
        CMND = toSet;
    }
}
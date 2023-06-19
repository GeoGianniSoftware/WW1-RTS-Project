using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMD_Pathfinding : CMDScript
{
    public float AISensesTickInterval = .5f;

    public Vector2Int mapSubdivisionCount;
    public Vector2 subSize;
    public PathNode[,] PathNodes = null;
    public Terrain currentTerrain;

    [Header("Gizmos")]
    public bool DrawPathGizmos = false;
    public bool DrawGizmosWhenSelected = true;
    public bool DrawValidGizmos = true;
    public bool DrawInvalidGizmos = false;
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate() {
        if (DrawPathGizmos && !Application.isPlaying) {
            CreateMap();

        }
        else {
            ClearMap();
        }
    }

    void DrawDebugGizmos() {
        if (PathNodes == null)
            return;

        for (int x = 0; x < PathNodes.GetLength(0); x++) {
            for (int y = 0; y < PathNodes.GetLength(1); y++) {

                float steep = PathNodes[x, y].steepness;
                if (steep > 45) {
                    if (DrawInvalidGizmos) {
                        Gizmos.color = Color.red;
                    }
                    else continue;
                }
                else if (!DrawValidGizmos) {
                    continue;
                }
                else if (steep < 45 && steep > 10)
                    Gizmos.color = Color.yellow;
                else
                    Gizmos.color = Color.green;


                Gizmos.DrawLine(PathNodes[x, y].wPos, PathNodes[x, y].wPos + Vector3.up);

            }
        }
    }

    private void OnDrawGizmos() {
        if (DrawGizmosWhenSelected)
            return;

        DrawDebugGizmos();
    }

    private void OnDrawGizmosSelected() {
        if (!DrawGizmosWhenSelected)
            return;
        DrawDebugGizmos();
    }

    void ClearMap() {
        PathNodes = null;
        subSize = Vector2.zero;
    }

    void CreateMap() {
        if (PathNodes != null)
            return;

        PathNodes = new PathNode[mapSubdivisionCount.x, mapSubdivisionCount.y];

        if(mapSubdivisionCount.x < 1 || mapSubdivisionCount.y < 1) {
            return;
        }
        
        int xSub = CMND.cmd_mapmanager.mapSize.x / mapSubdivisionCount.x;
        int zSub = CMND.cmd_mapmanager.mapSize.y / mapSubdivisionCount.y;
        subSize = new Vector2(xSub, zSub);

        Vector3 topLeft = new Vector3(-CMND.cmd_mapmanager.mapSize.x / 2 + (xSub / 2), 0, -CMND.cmd_mapmanager.mapSize.y / 2 + (zSub / 2));

        for (int x = 0; x < mapSubdivisionCount.x; x++) {
            for (int z = 0; z < mapSubdivisionCount.y; z++) {

                Vector3 pos = new Vector3(x * xSub, 25, z * zSub);
                
                pos += topLeft;

                Vector3 terrainLocalPos = currentTerrain.transform.InverseTransformPoint(pos);
                Vector2 normalizedPos = new Vector2(Mathf.InverseLerp(0.0f, currentTerrain.terrainData.size.x, terrainLocalPos.x),
                                            Mathf.InverseLerp(0.0f, currentTerrain.terrainData.size.z, terrainLocalPos.z));

                float steepness = currentTerrain.terrainData.GetSteepness(normalizedPos.x, normalizedPos.y);
                float height = currentTerrain.SampleHeight(pos);
                pos.y = height+currentTerrain.transform.position.y;

                PathNodes[x, z] = new PathNode(new Vector2Int(x, z), pos, steepness);
            }
        }
    }
}

public class PathNode
{
    public Vector2Int pos;
    public Vector3 wPos;
    public float steepness;
    public float walkablePercentage;

    public PathNode(Vector2Int p, Vector3 w, float s) {
        pos = p;
        wPos = w;
        steepness = s;
        walkablePercentage = 1 - (steepness / 45);
    }

}

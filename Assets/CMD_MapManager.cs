using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMD_MapManager : CMDScript
{

    public Vector2Int mapSize;
    public Vector2Int mapSubdivisionCount;
    public Vector2 subSize;
    public MapSubdivision[,] mapSubdivisions = null;
    public List<MapSubdivision> mapSubdivisionList = new List<MapSubdivision>();
    public Terrain currentTerrain;
    public MapDataHolder mapDataHolder;

    public List<GameObject> waterTiles = new List<GameObject>();

    // Start is called before the first frame update
    void Start() {
        CreateMapZones();

        
        

        waterTiles.AddRange(GameObject.FindGameObjectsWithTag("Water"));
        

        
    }

    public void SetMapHole(Vector3 position, Vector2Int size, bool setHole) {
        bool[,] holeData = getBoolArray(size.x, size.y, !setHole);

        int[] pos = WorldPositionToTerrainTexturePosition(position, size);

        currentTerrain.terrainData.SetHoles(pos[0], pos[1], holeData);
        mapDataHolder.holes.Add(new MapHoleSet(pos[0], pos[1], holeData));
    }

    int[] WorldPositionToTerrainTexturePosition(Vector3 position, Vector2Int size) {
        float xPos = ((position.x - currentTerrain.transform.position.x)- (size.x / 2)) /currentTerrain.terrainData.size.x;
        float zPos = ((position.z - currentTerrain.transform.position.z)- (size.y / 2)) / currentTerrain.terrainData.size.z;


        int xFin = (int)(xPos * currentTerrain.terrainData.holesTexture.width * 1.003f);
        int zFin = (int)(zPos * currentTerrain.terrainData.holesTexture.height * 1.003f);

        return new int[] { xFin, zFin };
    }

    bool[,] getBoolArray(int xSize, int ySize, bool toSet) {
        bool[,] boolArray = new bool[xSize, ySize];
        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                boolArray[x, y] = toSet;
            }
        }

        return boolArray;
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnValidate() {
        if (mapDataHolder != null && !Application.isPlaying && mapDataHolder.holes.Count > 0) {
            foreach (MapHoleSet holeSet in mapDataHolder.holes) {
                currentTerrain.terrainData.SetHoles(holeSet.xPos, holeSet.yPos, getBoolArray(holeSet.xDataLength, holeSet.yDataLength, true));
            }

            mapDataHolder.holes.Clear();
        }

        if (mapSubdivisionCount.x < 1) {
            mapSubdivisionCount = new Vector2Int(1, mapSubdivisionCount.y);
        }
        if (mapSubdivisionCount.y < 1) {
            mapSubdivisionCount = new Vector2Int(mapSubdivisionCount.x, 1);
        }
    }

    private void OnDrawGizmos() {
        if (mapSubdivisions == null)
            return;

        for (int x = 0; x < mapSubdivisions.GetLength(0); x++) {
            for (int y = 0; y < mapSubdivisions.GetLength(1); y++) {

                MapSubdivision sub = mapSubdivisions[x, y];
                Gizmos.color = sub.zoneColor;
                Gizmos.DrawWireCube(sub.center, new Vector3(subSize.x, 50, subSize.y));

            }
        }

    }

    public void CreateMapZones() {
        mapSubdivisions = new MapSubdivision[mapSubdivisionCount.x, mapSubdivisionCount.y];

        int xSub = mapSize.x / mapSubdivisionCount.x;
        int zSub = mapSize.y / mapSubdivisionCount.y;
        subSize = new Vector2(xSub, zSub);

        Vector3 topLeft = new Vector3(-mapSize.x / 2 + (xSub / 2), 0, -mapSize.y / 2 + (zSub / 2));

        for (int x = 0; x < mapSubdivisionCount.x; x++) {
            for (int z = 0; z < mapSubdivisionCount.y; z++) {

                Vector3 pos = new Vector3(x * xSub, 25f, z * zSub);
                pos += topLeft;

                mapSubdivisions[x, z] = new MapSubdivision(pos, new Vector2Int(x, z));
                mapSubdivisionList.Add(mapSubdivisions[x, z]);
            }
        }
    }
}

[System.Serializable]
public class MapSubdivision
{
    public Vector3 center;
    public Vector2Int index;
    public Color zoneColor;

    public MapSubdivision(Vector3 _center, Vector2Int _index) {
        center = _center;
        index = _index;
        zoneColor = Random.ColorHSV();
    }
}


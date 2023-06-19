using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewMapData", menuName = "New MapData/New MapData", order = 1)]
[System.Serializable]
public class MapDataHolder: ScriptableObject
{
    public List<MapHoleSet> holes = new List<MapHoleSet>();



}

[System.Serializable]
public class MapHoleSet
{
    public int xPos;
    public int yPos;
    public bool[,] holeData;
    public int xDataLength;
    public int yDataLength;

    public MapHoleSet(int x, int y, bool[,] data) {
        xPos = x;
        yPos = y;
        holeData = data;
        xDataLength = data.GetLength(0);
        yDataLength = data.GetLength(1);
    }
}
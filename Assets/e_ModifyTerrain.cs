using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class e_ModifyTerrain : MonoBehaviour
{

    public bool markTerrain = false;
    void Update()
    {
        if(GetComponent<Building>() != null && !markTerrain) {
            if (GetComponent<Building>().placed) {
                CMD.CMND.cmd_mapmanager.SetMapHole(transform.position, new Vector2Int(11, 11), true);
                markTerrain = true;
            }
        }
    }
}

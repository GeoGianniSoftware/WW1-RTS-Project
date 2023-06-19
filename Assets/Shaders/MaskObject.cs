using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskObject : MonoBehaviour
{
    public List<GameObject> ObjMasked = new List<GameObject>();
    void Start()
    {
        foreach (GameObject g in CMD.CMND.cmd_mapmanager.waterTiles) {
            AddObjectToMask(g);
        }
    }


    void AddObjectToMask(GameObject obj) {
        if (obj.GetComponent<MeshRenderer>()) {
            obj.GetComponent<MeshRenderer>().material.renderQueue = 3002;
        }

    }

}


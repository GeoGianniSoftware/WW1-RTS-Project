using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TestBotCreator : MonoBehaviour
{
    public GameObject botPrefab;
    public GameObject unitPrefab;

    public Material mat1;
    public Material mat2;

    public float spacing = 3f;
    public int sortCount = 5;
    public int botPerPlacement = 10;

    public Transform target;
    public Transform target2;
    public int botCount = 0;

    public bool NavMesh = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }
    int heightIndex = 0;
    int widthIndex = 0;

    void SpawnBot(int i = -1, int team = 0) {
        Vector3 pos = CMD.CMND.RaycastMousePositionOnTerrain().posHit;

        Vector3 width = (Vector3.right * spacing) * (widthIndex - (sortCount / 2));
        Vector3 forward = (Vector3.forward * spacing) * (heightIndex - (sortCount / 2));

        Transform dest = target;
        Material mat = mat1;
        if(team == 1) {
            dest = target2;
            mat = mat2;
        }

        if (i != -1) {
            pos += width + forward; 

            if(widthIndex >= sortCount-1) {
                heightIndex++;
                widthIndex = 0;
            }
            else {
                widthIndex++;
            }
            
        }

        GameObject toSpawn = unitPrefab;
        if (!NavMesh)
            toSpawn = botPrefab;

        GameObject g = Instantiate(toSpawn, pos + (Vector3.up*.25f), Quaternion.identity);
        if (!NavMesh) {
            AIDestinationSetter destination = g.GetComponent<AIDestinationSetter>();
            AIPath path = g.GetComponent<AIPath>();
            path.maxSpeed = Random.Range(1f, 10f);
            destination.target = dest;
        }
        else {
            g.GetComponentInChildren<SkinnedMeshRenderer>().material = mat;
            g.GetComponent<Unit>().MoveOrder(dest.transform.position);
            g.GetComponent<Unit>().owner = team;
        }
        
        botCount++;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {

            for (int i = 0; i < botPerPlacement; i++) {
                SpawnBot(i, 0);
            }
            heightIndex = 0;
            widthIndex = 0;
            
        }

        if (Input.GetMouseButtonDown(1)) {

            for (int i = 0; i < botPerPlacement; i++) {
                SpawnBot(i, 1);
            }
            heightIndex = 0;
            widthIndex = 0;

        }
    }
}

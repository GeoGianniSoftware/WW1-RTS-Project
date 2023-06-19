using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CMD_Units : CMDScript
{

    public List<Unit> playerUnits = new List<Unit>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Unit SpawnUnit(UnitCore unitToSpawn, Vector3 position, float rotation = 0f) {
        GameObject unitSpawned = Instantiate(unitToSpawn.entityPrefab, position, Quaternion.Euler(new Vector3(0, rotation, 0)));
        if (unitSpawned.GetComponent<NavMeshAgent>()) {
            unitSpawned.GetComponent<NavMeshAgent>().nextPosition = position;
        }

        Unit _unit = unitSpawned.GetComponent<Unit>();

        playerUnits.Add(_unit);
        return _unit;
    }
}

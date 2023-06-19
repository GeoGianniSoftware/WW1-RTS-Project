using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class VehicleWheelScript : MonoBehaviour
{

    NavMeshAgent NMA;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (NMA == null)
            NMA = transform.root.GetComponent<NavMeshAgent>();
        else {
            transform.localRotation = Quaternion.Euler(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + NMA.velocity.magnitude * -.5f, transform.localEulerAngles.z));
        }


    }
}

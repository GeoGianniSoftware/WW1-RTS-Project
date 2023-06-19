using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : Entity
{
    

    public bool resourcePickup = true;
    public float pickupRange;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        EntityTick();
    }

}

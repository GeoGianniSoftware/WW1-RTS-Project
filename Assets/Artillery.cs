using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artillery : Vehicle
{

    public Vector3 firePosition = Vector3.zero;
    public GameObject shellPrefab;
    public Transform barrelTransform;

    public float velocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        VehicleTick();

        if (firePosition != Vector3.zero) {
            transform.LookAt(new Vector3(firePosition.x, transform.position.y, firePosition.z));
            if (canFire())
                Fire();
        }

        if(ammoCount == 0 && currentSupply > 0) {
            delayTimer += getVehicleCore().fireRate;
            currentSupply--;
            ammoCount += 5;
        }
    }

    public override bool canFire() {
        if (currentHealth > 0 && ammoCount > 0 && delayTimer <= 0) {
            return true;
        }

        return false;
    }

    public override void Fire() {
        if (canFire()) {
            print("Firing");
            FireArtillery();
            
            delayTimer += getUnitCore().fireRate;
        }
        else {
            return;
        }
        ammoCount--;
    }

    float getAccuracyOffset() {
        float a = Random.Range(getVehicleCore().accuracy, 1f);

        return a;

    }

    public void FireArtillery() {
        float angle = 0f;
        if (CMD.CalculateLobAngle(barrelTransform.position, firePosition, velocity, out angle)) {
            

            float offset = getAccuracyOffset();

            print(angle+offset);
            barrelTransform.localEulerAngles = new Vector3(angle + offset, 0f, 0f);
        }

        
        GameObject shell = Instantiate(shellPrefab, barrelTransform.position, barrelTransform.rotation);
        shell.GetComponent<Rigidbody>().AddForce(barrelTransform.forward * (velocity+Random.Range(-3f,3f)), ForceMode.VelocityChange);
        shell.GetComponent<ArtilleryShell>().SetupShell(damage);
    }

    public override void MoveOrder(Vector3 dest) {
        firePosition = dest;


    }
}

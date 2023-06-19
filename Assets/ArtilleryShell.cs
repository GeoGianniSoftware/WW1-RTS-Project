using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryShell : Explosive
{
    Rigidbody RG;
    // Start is called before the first frame update
    void Start()
    {
        RG = GetComponent<Rigidbody>();
    }

    public void SetupShell(int dmg) {
        damage = dmg;
    }

    // Update is called once per frame
    void Update()
    {

        Ray ray = new Ray(transform.position, RG.velocity);
        transform.GetChild(0).LookAt(transform.position + RG.velocity);
        Debug.DrawRay(ray.origin, ray.direction * 5f);

        RaycastHit[] cols = Physics.RaycastAll(ray, 1f);
        if(cols.Length > 0) {
            transform.GetChild(1).SetParent(null);
            Explode();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, 25f);
    }
}

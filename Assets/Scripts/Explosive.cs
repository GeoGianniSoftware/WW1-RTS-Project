using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public int owner;
    public int damage;
    public float radius;
    public void Explode() {
        Instantiate(Resources.Load<GameObject>("FX/LargeExplosion"), transform.position, Quaternion.identity);


        Collider[] cols = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider c in cols) {


            if (c.GetComponent<Entity>()) {
                Entity e = c.GetComponent<Entity>();


                float percentOfDistance = 1f - (Vector3.Distance(transform.position, e.transform.position) / radius);
                if (percentOfDistance < .25f)
                    percentOfDistance = .25f;

                int totalDamage = Mathf.RoundToInt(percentOfDistance * damage) + 1;
                if (totalDamage > damage) {
                    totalDamage = damage;
                }

                e.SendMessage("TakeDamage", new Damage(totalDamage, true, transform.position, radius));
            }
        }
        
        Destroy(gameObject);

    }
}

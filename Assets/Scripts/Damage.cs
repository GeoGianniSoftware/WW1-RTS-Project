using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage
{
    public int amount;
    public bool explosive;
    public Vector3 damageSourceLocation;
    public float radius;

    public Damage(int amt) {
        amount = amt;
    }

    public Damage(int amt, bool _explosive, Vector3 dmgLoc, float _radius = -1) {
        amount = amt;
        explosive = _explosive;
        damageSourceLocation = dmgLoc;
        radius = _radius;
    }
}

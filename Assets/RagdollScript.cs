using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollScript : MonoBehaviour
{
    public bool loadRagdollData = false;

    public bool ragdollEnabled = false;
    public List<Rigidbody> rigidBodies = new List<Rigidbody>();
    public List<Collider> colliders = new List<Collider>();
    public List<CharacterJoint> joints = new List<CharacterJoint>();

    private void Start() {
        SetRagdollState(false);
    }

    Damage damageData = null;
    public void EnableRagdoll(Damage dmg = null) {

        if (dmg != null) {
            damageData = dmg;
        }

        ragdollEnabled = true;
        GetComponentInParent<Collider>().enabled = false;
        SetRagdollState(ragdollEnabled);
    }

    public void SetRagdollState(bool state) {
        print("Ragdoll: " + state.ToString());

        ragdollEnabled = state;


        for (int i = 0; i < rigidBodies.Count; i++) {
            Rigidbody rigid = rigidBodies[i];
            rigid.isKinematic = !ragdollEnabled;

            rigid.velocity = Vector3.zero;
        }

        Vector3 dir = -transform.forward + (transform.up * -.1f);

        //BulletImpact
        //rigidBodies[Random.Range(0,rigidBodies.Count)].AddForce(dir * 60f, ForceMode.Impulse);
        //Explosion

        if(damageData != null) {
            rigidBodies[Random.Range(0, rigidBodies.Count)].AddExplosionForce(10000f, damageData.damageSourceLocation, damageData.radius, 2f);
        }
        

        foreach (Rigidbody rigid in rigidBodies) {
            

            
        }
        foreach (Collider col in colliders) {
            col.enabled = ragdollEnabled;
        }
        foreach(CharacterJoint joint in joints) {

        }

    }

    void LoadRagdollData() {
        rigidBodies.Clear();
        colliders.Clear();

        rigidBodies.AddRange(GetComponentsInChildren<Rigidbody>());
        colliders.AddRange(GetComponentsInChildren<Collider>());
        joints.AddRange(GetComponentsInChildren<CharacterJoint>());

        print(rigidBodies.Count);
        print(colliders.Count);

        SetRagdollState(false);
    }

    private void OnValidate() {
        if (loadRagdollData) {
            LoadRagdollData();
            loadRagdollData = false;
        }
    }

}

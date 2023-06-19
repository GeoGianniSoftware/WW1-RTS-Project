using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmine : Explosive
{
    public enum ActivationTypes
    {
        Radius,
        AttachedEntity
    }
    public ActivationTypes activationType;

    public float activationRadius;

    public bool restrictTriggerType = false;
    public EntityType triggerType;


    public Entity attachedEntity;

    public float delay = 0;
    float timer;

    public bool activated = false;
    // Start is called before the first frame update
    void Start()
    {
        timer = delay;
    }

    private void Update() {
        if (activated) {
            timer -= Time.deltaTime;

            if (timer <= 0)
                Explode();
        }
    }

    public bool RestrictionsAndTriggers(Entity e = null){
        if (activationType == ActivationTypes.Radius && (!restrictTriggerType || (restrictTriggerType && e != null && e.entityCore.TYPE == triggerType))) {
            return true;
        }

        if(activationType == ActivationTypes.AttachedEntity && attachedEntity != null)
            return true;

        return false;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(activationType == ActivationTypes.Radius) {
            Collider[] cols = Physics.OverlapSphere(transform.position, activationRadius);
            foreach (Collider c in cols) {
                Entity entity = c.GetComponentInChildren<Entity>();
                if (entity != null && entity.owner != owner && entity.currentHealth > 0 && RestrictionsAndTriggers(entity)) {
                    activated = true;
                }
            }
        }else if(activationType == ActivationTypes.AttachedEntity && RestrictionsAndTriggers() && attachedEntity.currentHealth  <= 0) {
            activated = true;
        }
        
    }
}

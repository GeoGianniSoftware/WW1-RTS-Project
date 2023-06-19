using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EntityType
{
    BASIC,
    UNIT,
    VEHICLE,
    STATION,
    BUILDING,
    STATICDEFENSE,
    PRODUCTION,
    RESOURCE,
    FACTORY
}

public class Entity : MonoBehaviour {
    public Hash128 unitID;
    public EntityCore entityCore;
    public int owner = 0;
    public bool selected = false;

    public bool initalized = false;
    public bool alive = true;

    public int currentHealth;
    public float hitChance = 1f;

    public bool active = true;
    public UnitInfoUI infoUI;
    public bool isCargo = false;


    public void CreateHash() {
        unitID = new Hash128();
        unitID.Append(owner);
        unitID.Append(Time.time);
        unitID.Append(transform.position.x);
        unitID.Append(transform.position.z);
    }

    public override bool Equals(System.Object obj) {
        if (obj == null)
            return false;
        Entity e = obj as Entity;
        if ((System.Object)e == null)
            return false;
        return unitID == e.unitID;
    }
    public bool Equals(Entity c) {
        if ((object)c == null)
            return false;
        return unitID == c.unitID;
    }

    public Entity() {
        
    }

    public virtual void Interact(Entity entityInteracting) {

    }


    public Damage lastDamageTaken = null;
    public virtual void TakeDamage(Damage dmg) {
        float chance = Random.Range(0f, 1f);
        if (chance > 1f - hitChance) {
            lastDamageTaken = dmg;
            currentHealth -= dmg.amount;
        }
            

        
    }


    public float getHealthPercent() {
        if (getEntityCore().maxHealth <= 0)
            return 0f;

        return (float)currentHealth / (float)entityCore.maxHealth;
    }

    public virtual void Die() {
        alive = false;
        currentHealth = 0;
    }

    public virtual void SelectObject() {
        selected = true;
    }
    public virtual void DeselectObject() {
        selected = false;
    }

    public virtual EntityCore getEntityCore() {
        switch (entityCore.TYPE) {
            default:
                return entityCore;
            case EntityType.UNIT:
                return (UnitCore)entityCore;
            case EntityType.STATION:
                return (StationCore)entityCore;
            case EntityType.VEHICLE:
                return (VehicleCore)entityCore;
            case EntityType.STATICDEFENSE:
                return (DefenseCore)entityCore;
            case EntityType.PRODUCTION:
                return (ProductionBuildingCore)entityCore;
            case EntityType.BUILDING:
                return (BuildingCore)entityCore;
        }


        
    }

    public void EntityInitialize() {
        CreateHash();
        currentHealth = getEntityCore().maxHealth;
        if(owner == 0 && entityCore.TYPE != EntityType.UNIT)
            SpawnHealthbar(getEntityCore().infoUIOffset);

        initalized = true;
    }

    public void EntityTick() {
        if (!initalized) {
            EntityInitialize();
           
        }

        if (currentHealth <= 0) {
            if (alive) {
                Die();
            }
            //Do dead entity things here

            return;
        }
    }

    private void Update() {
        EntityTick();
    }

    public void SpawnHealthbar(Vector3 infoUIOffset) {
        infoUI = Instantiate(Resources.Load<GameObject>("UI/UnitInfoUI"), this.transform).GetComponent<UnitInfoUI>();
        infoUI.connectedEntity = this;
        infoUI.transform.position = this.transform.position + (Vector3.up*2.5f) + infoUIOffset;
    }
    private void Start() {
        if(entityCore != null) {

            currentHealth = entityCore.maxHealth;
        }

    }

    
}

[System.Serializable]
public class EntityCore : ScriptableObject
{
    public string Name;

    public EntityType TYPE = EntityType.BASIC;
    public Sprite cardSprite;
    public GameObject entityPrefab;

    public float size = 1f;


    public Vector3 infoUIOffset;


    public int maxHealth;
    public float baseHitChance;

    [Header("Pricing")]
    public int productionCost = -1;
    public int supplyCost = -1;
    public int componentCost = -1;
    public int fuelCost = -1;
    public int price = -1;

}



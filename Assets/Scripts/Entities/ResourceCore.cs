using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewResourceCore", menuName = "New Entity/New Resource", order = 1)]
[System.Serializable]
public class ResourceCore : EntityCore
{
    public ResourceType resourceType;
}
public enum ResourceType
{
    Supplies,
    Components,
    Oil,
    Money
}



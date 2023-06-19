using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newResearch", menuName ="New Entity/New Research", order = 1)]
public class ResearchEntry : ScriptableObject
{
    public string entryName;
    public string entryDesc;
    public int researchValue;

 }

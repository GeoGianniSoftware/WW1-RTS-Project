using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_UI : MonoBehaviour
{
    public GameObject BuildingUI;
    public GameObject ResearchUI;
    public GameObject LogisticsUI;


    [Header("Research")]
    public Transform currentResearchPanel;
    public GameObject currentResearchPrefab;
    public CurrentResearchFrame currentResearchGO;

    public Transform availableResearchPanel;
    public GameObject availableResearchPrefab;

    [Header("Building")]
    public GameObject buildingPanel;

    [Header("Logistics")]
    public Transform logisticsContentPanel;
    public GameObject logisticsContextPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

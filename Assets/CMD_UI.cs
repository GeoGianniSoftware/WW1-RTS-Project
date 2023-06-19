using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CMD_UI : CMDScript
{
    // Start is called before the first frame update

    public R_UI ref_ui;

    [Header("Research")]
    public List<ResearchDisplay> availableResearchDisplays = new List<ResearchDisplay>();
    [Header("Building")]
    public int currentBuildingMenuIndex = 0;

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        for (int i = 0; i < CMND.cmd_research.availableResearch.Count; i++) {
            ResearchNode node = CMND.cmd_research.availableResearch[i];
            if (!isContainedInDisplays(node)) {
                GameObject availableDisplay = Instantiate(ref_ui.availableResearchPrefab, ref_ui.availableResearchPanel);
                availableDisplay.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = node.entry.entryName;
                availableDisplay.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "" + node.entry.researchValue + " RP";

                Button b = availableDisplay.GetComponentInChildren<Button>();

                b.onClick.AddListener(delegate { CMND.cmd_research.SelectResearch(node); });
                availableResearchDisplays.Add(new ResearchDisplay(availableDisplay, node));
            }
        }

        if(CMND.cmd_research.currentResearch != null) {
            if(ref_ui.currentResearchGO == null) {
                GameObject currentDisplay = Instantiate(ref_ui.currentResearchPrefab, ref_ui.currentResearchPanel);
                CurrentResearchFrame frame = currentDisplay.GetComponent<CurrentResearchFrame>();
                frame.Setup(CMND.cmd_research.getCurrentResearchEntry().name);
                ref_ui.currentResearchGO = frame;
            }
            else {
                ref_ui.currentResearchGO.researchProgress = (CMND.cmd_research.researchProgress / (float)CMND.cmd_research.getCurrentResearchEntry().researchValue);
            }

        }
    }

    #region Research
    [System.Serializable]
    public class ResearchDisplay
    {
        public GameObject displayGO;
        public ResearchNode node;

        public ResearchDisplay(GameObject g, ResearchNode _node) {
            displayGO = g;
            node = _node;
        }
    }

    bool isContainedInDisplays(ResearchNode n) {
        bool result = false;
        foreach(ResearchDisplay display in availableResearchDisplays) {
            if (display.node == n)
                result = true;
        }
        return result;
    }

    public void removeNode(ResearchNode n) {
        if (!isContainedInDisplays(n))
            return;

        ResearchDisplay temp = null;
        foreach (ResearchDisplay display in availableResearchDisplays) {
            if (display.node == n)
                temp = display;
        }


        if (temp != null)
            availableResearchDisplays.Remove(temp);
        Destroy(temp.displayGO);
    }



    #endregion
    #region Building
    public void setBuildMenuIndex(int index) {
        currentBuildingMenuIndex = index;
        PopulateBuildPanel();
    }

    public void ClearBuildPanel() {
        for (int i = 0; i < ref_ui.buildingPanel.transform.childCount-1; i++) {
            Destroy(ref_ui.buildingPanel.transform.GetChild(i+1).gameObject);
        }
    }

    public void PopulateBuildPanel() {
        if(ref_ui.buildingPanel.transform.childCount > 1) {
            ClearBuildPanel();
        }
        for (int i = 0; i < CMND.cmd_building.getCurrentBuildList().Count; i++) {
            GameObject button = Instantiate(Resources.Load<GameObject>("UI/BuildingButton"), ref_ui.buildingPanel.transform);
            button.transform.GetChild(0).GetComponent<Image>().sprite = CMND.cmd_building.getCurrentBuildList()[i].cardSprite;
            button.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = CMND.cmd_building.getCurrentBuildList()[i].Name;
            int test = i;
            button.GetComponent<Button>().onClick.AddListener(delegate { CMND.cmd_building.setBuildingIndexLock(test); });
        }


    }
    #endregion

    public void toggleBuildUI() {
        toggleBuildUI(!ref_ui.BuildingUI.activeSelf);
    }
    public void toggleBuildUI(bool set) {
        CMND.cmd_building.toggleBuildMode(set);
        ref_ui.BuildingUI.SetActive(set);
    }
    public void toggleResearchUI() {
        toggleResearchUI(!ref_ui.ResearchUI.activeSelf);
    }
    public void toggleResearchUI(bool set) {
        ref_ui.ResearchUI.SetActive(set);
    }
    public void toggleLogisticshUI() {
        toggleLogisticshUI(!ref_ui.LogisticsUI.activeSelf);
    }
    public void toggleLogisticshUI(bool set) {
        ref_ui.LogisticsUI.SetActive(set);
    }



    public static bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

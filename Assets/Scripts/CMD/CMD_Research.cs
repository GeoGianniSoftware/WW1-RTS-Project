using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMD_Research : CMDScript
{

    public ResearchGraph playerResearch;
    public ResearchNode currentResearch;
    public float researchProgress = 0;


    public List<ResearchNode> availableResearch = new List<ResearchNode>();
    public List<ResearchNode> completedResearch = new List<ResearchNode>();


    // Start is called before the first frame update
    void Start()
    {
        playerResearch.Reset();
        

        foreach(ResearchNode node in playerResearch.nodes) {
            
            int count = 0;
            foreach(XNode.NodePort p in node.Inputs) {
                count += p.ConnectionCount;
            }
            if(count == 0) {
                availableResearch.Add(node);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentResearch != null) {
            if (researchProgress < currentResearch.entry.researchValue) {
                researchProgress+= Time.deltaTime;
            }

            if(researchProgress >= currentResearch.entry.researchValue) {
                if (!currentResearch.completed) {
                    researchComplete();
                    
                }
                
            }
            
        }
    }

    void researchComplete() {
        currentResearch.completed = true;
        PopulateAvailableResearch(currentResearch);
    }

    public ResearchEntry getCurrentResearchEntry() {
        return currentResearch.entry;
    }

    public void SelectResearch(ResearchNode nodeToResearch) {
        if ((currentResearch == null|| currentResearch.completed) && availableResearch.Contains(nodeToResearch)) {

            if(currentResearch != null) {
                completedResearch.Add(currentResearch);
            }

            clearNode(nodeToResearch);
            currentResearch = nodeToResearch;
            researchProgress = 0;

           if(CMND.cmd_ui.ref_ui.currentResearchGO != null) {
                CMND.cmd_ui.ref_ui.currentResearchGO.UpdateDisplay(nodeToResearch.entry.entryName);
            }
        }
        
    }

    void clearNode(ResearchNode nodeToClear) {
        print("clearing Node");
        availableResearch.Remove(nodeToClear);
        CMND.cmd_ui.removeNode(nodeToClear);
    }

    bool researchNodePrerequisiteAreComplete(ResearchNode nodeToCheck) {
        bool prerequisite = true;
        foreach (ResearchNode n in nodeToCheck.getInputs()) {


            if (!n.completed)
                prerequisite = false;
        }

        return prerequisite;
    }

    void PopulateAvailableResearch(ResearchNode nodeToTake) {
        foreach(ResearchNode node in nodeToTake.getOutputs()) {
            if (!availableResearch.Contains(node) && researchNodePrerequisiteAreComplete(node)) {
                availableResearch.Add(node);
            }
        }
    }

}


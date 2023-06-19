using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMD_Logistics : CMDScript
{
    public bool creatingLogisticLink = false;
    public List<LogisticLink> playerLogisticLinks = new List<LogisticLink>();
    public List<UI_LogisticsContextBar> linkUIs = new List<UI_LogisticsContextBar>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        LogisticsLinkLogic();

    }

    private void OnDrawGizmos() {
        for (int i = 0; i < playerLogisticLinks.Count; i++) {
            if (playerLogisticLinks[i].linkType == LogisticLink.LinkTypes.SupplyLink)
                Gizmos.color = Color.cyan;
            else if (playerLogisticLinks[i].linkType == LogisticLink.LinkTypes.ProductionLink)
                Gizmos.color = Color.green;
            else if (playerLogisticLinks[i].linkType == LogisticLink.LinkTypes.Ammunition)
                Gizmos.color = Color.red;
            else if (playerLogisticLinks[i].linkType == LogisticLink.LinkTypes.Passenger)
                Gizmos.color = CMD.purpeColor;

            if (playerLogisticLinks[i].origin != null && playerLogisticLinks[i].destination != null)
                    Gizmos.DrawLine(playerLogisticLinks[i].origin.transform.position, playerLogisticLinks[i].destination.transform.position);
        }
    }

    public List<LogisticLink> SearchLogisticsLinks(Entity origin = null, Entity dest = null) {
        List<LogisticLink> returnList = new List<LogisticLink>();

        for (int i = 0; i < playerLogisticLinks.Count; i++) {
            if(playerLogisticLinks[i].destination == dest || playerLogisticLinks[i].origin == origin) {
                returnList.Add(playerLogisticLinks[i]);
            }
        }

        return returnList;
    }

    public void startLinkLogicTool() {
        creatingLogisticLink = true;
    }

    public void stopLinkLogicTool() {
        creatingLogisticLink = false;
    }

    bool selectedEntityIsLogicValid(Entity e) {
        if(CMND.cmd_selectionmanager.SelectedObjects[0].entityCore.TYPE == EntityType.FACTORY || CMND.cmd_selectionmanager.SelectedObjects[0].entityCore.TYPE == EntityType.PRODUCTION ||
            (CMND.cmd_selectionmanager.SelectedObjects[0].entityCore.TYPE == EntityType.VEHICLE && ((VehicleCore)CMND.cmd_selectionmanager.SelectedObjects[0].entityCore).requiresSupply) ||
            (CMND.cmd_selectionmanager.SelectedObjects[0].entityCore.TYPE == EntityType.BUILDING && ((BuildingCore)CMND.cmd_selectionmanager.SelectedObjects[0].entityCore).canHoldCargo)){

            return true;
        }

        return false;
    }


    public Entity linkOrigin;
    public Entity linkDestination;
    LogisticLink.LinkTypes linkType = LogisticLink.LinkTypes.SupplyLink;
    void LogisticsLinkLogic() {
        if (creatingLogisticLink) {

            if(linkOrigin == null) {

                if(!CMND.cmd_selectionmanager.isSelectionEmpty() && selectedEntityIsLogicValid(CMND.cmd_selectionmanager.SelectedObjects[0])) {
                    if (CMND.cmd_selectionmanager.SelectedObjects[0].entityCore.TYPE == EntityType.VEHICLE)
                        linkType = LogisticLink.LinkTypes.Ammunition;
                    else if (CMND.cmd_selectionmanager.SelectedObjects[0].entityCore.TYPE == EntityType.PRODUCTION)
                        linkType = LogisticLink.LinkTypes.ProductionLink;
                    else if (CMND.cmd_selectionmanager.SelectedObjects[0].entityCore.TYPE == EntityType.STATION)
                        linkType = LogisticLink.LinkTypes.Passenger;
                    else
                        linkType = LogisticLink.LinkTypes.SupplyLink;

                    linkOrigin = CMND.cmd_selectionmanager.SelectedObjects[0];
                    CMND.cmd_selectionmanager.ClearSelection();
                }

            }
            else {
                if (!CMND.cmd_selectionmanager.isSelectionEmpty() && selectedEntityIsLogicValid(CMND.cmd_selectionmanager.SelectedObjects[0])) {
                    if (CMND.cmd_selectionmanager.SelectedObjects[0].entityCore.TYPE == EntityType.VEHICLE)
                        linkType = LogisticLink.LinkTypes.Ammunition;
                    else if(CMND.cmd_selectionmanager.SelectedObjects[0].entityCore.TYPE == EntityType.PRODUCTION)
                        linkType = LogisticLink.LinkTypes.ProductionLink;
                    else if (CMND.cmd_selectionmanager.SelectedObjects[0].entityCore.TYPE == EntityType.STATION)
                        linkType = LogisticLink.LinkTypes.Passenger;
                    else
                        linkType = LogisticLink.LinkTypes.SupplyLink;

                    linkDestination = CMND.cmd_selectionmanager.SelectedObjects[0];
                    CMND.cmd_selectionmanager.ClearSelection();
                }
            }

            if (linkDestination != null && linkOrigin != null) {
                CreateLogicLinks(linkType);
            }

            
        }

        if (linkUIs.Count < playerLogisticLinks.Count) {
            CreateLogicLinkUI(playerLogisticLinks[linkUIs.Count]);
        }
    }

    public void RemoveLogicLink(int linkID) {
        if(linkID < playerLogisticLinks.Count && linkID >= 0) {
            Destroy(linkUIs[linkID].gameObject);
            linkUIs.RemoveAt(linkID);
            playerLogisticLinks.RemoveAt(linkID);
        }
    }

    void CreateLogicLinkUI(LogisticLink link) {
        UI_LogisticsContextBar prefab = Instantiate(CMD.CMND.cmd_ui.ref_ui.logisticsContextPrefab, CMD.CMND.cmd_ui.ref_ui.logisticsContentPanel).GetComponent<UI_LogisticsContextBar>();
        prefab.PopulateUI(link);
        linkUIs.Add(prefab);
    }

    void CreateLogicLinks(LogisticLink.LinkTypes linkType) {

        LogisticLink link = new LogisticLink(linkType, linkOrigin, linkDestination, playerLogisticLinks.Count);
        playerLogisticLinks.Add(link);
        CreateLogicLinkUI(link);
        ClearLogicLinkSelections();
        stopLinkLogicTool();
    }

    void ClearLogicLinkSelections() {
        linkOrigin = null;
        linkDestination = null;
    }
}

[System.Serializable]
public class LogisticLink
{
    public enum LinkTypes
    {
        SupplyLink,
        ProductionLink,
        Ammunition,
        Passenger
    }

    public LinkTypes linkType;
    public Entity origin;
    public Entity destination;
    public int routeID = -1;

    public LogisticLink(LinkTypes type, Entity o, Entity d, int route) {
        linkType = type;
        origin = o;
        destination = d;
        routeID = route;
    }

}

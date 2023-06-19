using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CMD_SelectionManager : CMDScript
{
    GameObject lastObjectClicked;
    public List<Entity> SelectedObjects = new List<Entity>();
    [SerializeField]
    public RectTransform selectionBox;
    List<GameObject> selectionPlacecards = new List<GameObject>();
    List<GameObject> actionCards = new List<GameObject>();

    [Header("UI References")]
    public Transform unitSelectionPanelContent;
    public Transform unitActionPanelContent;
    public bool selectionEnabled = false;

    public List<UnitPlacecardUI> typesSelected = new List<UnitPlacecardUI>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {

        if(!CMND.cmd_building.isBuilding && (!CMND.cmd_ordertools.logicToolsEnabled || !CMND.cmd_logistics.creatingLogisticLink)) {
            selectionEnabled = true;

            SelectionLogic();
        }
        else {
            selectionEnabled = false;
        }
        

    }


    void SelectionLogic() {
        if (CMD_UI.IsPointerOverUIObject())
            return;

        //Test for selection boxes.
        SelectionBoxSetup();
        FillSelectedPortrait();
        
        if (Input.GetMouseButtonDown(0)) {
            lastObjectClicked = GetGameObjectAtCursor();

            if (Input.GetKey(KeyCode.LeftControl) && lastObjectClicked != null) {
                TrySelectGameObject(lastObjectClicked, false);
            }
            else if (lastObjectClicked != null) {
               TrySelectGameObject(lastObjectClicked, true);
            }
        }


        if (Input.GetMouseButtonDown(1) && SelectedObjects.Count > 0) {
            if (GetGameObjectAtCursor() != null && IsSelectable(GetGameObjectAtCursor())) {
                GameObject g = GetGameObjectAtCursor();

                foreach (Unit u in SelectedObjects) {
                    u.Interact(g.transform.GetComponentInParent<Entity>());

                }





            }
            else {
                if (Input.GetKey(KeyCode.LeftControl))
                    CommandSelectedAddMove(GetCursorWorldPosition());
                else
                    CommandSelectedMove(GetCursorWorldPosition());
            }


        }
    }

    Camera oldCamera = null;
    void FillSelectedPortrait() {
        if (SelectedObjects.Count > 0) {
            Entity objectRef = SelectedObjects[0];
            /*
            RawImage selectedImage = FindObjectOfType<SelectedDisplayImage>().GetComponent<RawImage>();
            Text selectedName = FindObjectOfType<SelectedNameText>().GetComponent<Text>();
            Image selectedHealthBar = FindObjectOfType<SelectedHealthBar>().GetComponent<Image>();
            
            selectedName.text = objectRef.Name;
            selectedHealthBar.fillAmount = (float)((float)objectRef.currentHealth / (float)objectRef.maxHealth);
            if(oldCamera != null)
                oldCamera.enabled = false;

            */
        }

    }

    public bool isSelectionEmpty() {
        if (SelectedObjects != null && SelectedObjects.Count > 0)
            return false;

        return true;
    }

    void ClearSelectionList() {
        foreach(GameObject g in selectionPlacecards) {
            Destroy(g);
        }
        foreach (GameObject g in actionCards) {
            Destroy(g);
        }
        typesSelected.Clear();
        selectionPlacecards.Clear();
        actionCards.Clear();
    }

    public void FillActionCards(Unit unit) {
        foreach(Unit_Order.OrderTypes order in (unit.getUnitCore().unitActions)){
            GameObject actionCard = Instantiate(Resources.Load<GameObject>("UI/ActionButton"), unitActionPanelContent);
            actionCard.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = order.ToString();

            actionCard.GetComponentInChildren<Button>().onClick.AddListener(delegate { QuickOrder(order); });
            actionCards.Add(actionCard);
        }
    }


    public void QuickOrder(Unit_Order.OrderTypes orderType) {
        print("Quick Order: " + orderType.ToString());
        List<Unit> selectedUnits = getSelectedUnits(EntityType.VEHICLE);

        if (orderType == Unit_Order.OrderTypes.UnloadVehiclePassengersOrder) {
            for (int i = 0; i < selectedUnits.Count; i++) {
                selectedUnits[i].AddUnloadTransportOrder((Vehicle)selectedUnits[i]);
            }
        }
    }

    void FillSelectedUnitList() {
        ClearSelectionList();
        if(SelectedObjects.Count > 0) {
            foreach(Entity u in SelectedObjects) {
                if (!checkTypeLogic(u.entityCore)) {
                    GameObject placecard = Instantiate((GameObject)Resources.Load("UI/UnitSelectionPlacecard") as GameObject, unitSelectionPanelContent, false);
                    selectionPlacecards.Add(placecard);

                    placecard.GetComponent<UnitPlacecardUI>().Activate(u);
                    typesSelected.Add(placecard.GetComponent<UnitPlacecardUI>());
                }

                if (u.GetComponent<Unit>()) {
                    Unit temp = u.GetComponent<Unit>();
                    if(temp.getUnitCore().unitActions.Count > 0) {
                        FillActionCards(temp);
                    }
                }



            }
        }
    }

    bool checkTypeLogic(EntityCore entityToCheck) {
        foreach(UnitPlacecardUI placecard in typesSelected) {
            if(placecard.attachedEntity.entityCore == entityToCheck) {
                placecard.incrementAmount();
                return true;
            }
        }

        return false;
    }


    Vector3 mousePos1, mousePos2;
    void SelectionBoxSetup() {
      
        if (Input.GetMouseButtonDown(0)) {
            mousePos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
               if(hit.point != mousePos1) {
                    return;
                }
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            mousePos2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (mousePos1 != mousePos2) {
                EvaluateSelectionBox();
            }
        }
    }

    public void SelectObject(Entity e) {
        ClearSelection();
        SelectedObjects.Add(e);
    }

    void EvaluateSelectionBox() {
        print("Selection Box");
        List<Entity> removeObjects = new List<Entity>();

        if (!Input.GetKey(KeyCode.LeftControl)) {
            
            ClearSelection();
        }

        Rect selectionRect = new Rect(mousePos1.x, mousePos1.y, mousePos2.x - mousePos1.x, mousePos2.y - mousePos1.y);
        

        foreach(Entity s in FindObjectsOfType<Entity>()) {
            if (s == null || (!selectionRect.Contains(Camera.main.WorldToViewportPoint(s.transform.position), true) && !SelectedObjects.Contains(s))) {
                removeObjects.Add(s);
            }
            else {
                TrySelectGameObject(s.gameObject, false);
            }
        }

        if(removeObjects.Count > 0) {
            foreach(Entity rems in removeObjects) {
                SelectedObjects.Remove(rems);
            }
            removeObjects.Clear();
        }
    }

    public GameObject GetGameObjectAtCursor() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            if (hit.transform.gameObject != null)
                return hit.transform.gameObject;
        }
        return null;
    }

    public Vector3 GetCursorWorldPosition() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            if (hit.transform.gameObject != null)
                return hit.point;
        }
        return new Vector3(0,-9999,0);
    }

    bool IsSelectable(GameObject g) {
        if (g == null)
            return false;
        if (g.transform.GetComponentInChildren<Entity>() != null)
            return true;
        if (g.transform.GetComponentInParent<Entity>() != null)
            return true;
        return false;
    }


    public void ClearSelection() {
        foreach (Entity s in SelectedObjects) {
            s.SendMessage("DeselectObject", SendMessageOptions.DontRequireReceiver);

        }
        SelectedObjects.Clear();
        ClearSelectionList();
    }


    void CommandSelectedAddMove(Vector3 posToMove) {
        print("AddMove");


        for (int i = 0; i < SelectedObjects.Count; i++) {
            if (SelectedObjects[i].GetComponent<Unit>() != null) {
                Unit u = SelectedObjects[i].GetComponent<Unit>();
                print(i);

                u.AddMoveOrder(posToMove);
                //if(u.Owner == FindObjectOfType<PlayerManager>().players[0])
                  //  u.Move(posToMove, i, SelectedObjects.Count);
            }
                
        }
   
    }
    void CommandSelectedMove(Vector3 posToMove) {

        for (int i = 0; i < SelectedObjects.Count; i++) {

            if (SelectedObjects[i].GetComponent<Unit>() != null) {
                Unit u = SelectedObjects[i].GetComponent<Unit>();

                u.MoveOrder(posToMove);

                //if (u.Owner == FindObjectOfType<PlayerManager>().players[0])
                //u.MoveToDest(posToMove, i, SelectedObjects.Count);
            }
        }
    }

    public List<Unit> getSelectedUnits(EntityType limit = EntityType.BASIC) {
        List<Unit> selectedUnits = new List<Unit>();

        for (int i = 0; i < SelectedObjects.Count; i++) {

            if(limit != EntityType.BASIC && SelectedObjects[i].getEntityCore().TYPE != limit) {
                continue;
            }

            if (SelectedObjects[i].GetComponent<Unit>() != null) {
                selectedUnits.Add(SelectedObjects[i].GetComponent<Unit>());
            }
        }

        return selectedUnits;
    }

    public bool TrySelectGameObject(GameObject objectToSelect, bool clearSelect) {


        if (IsSelectable(objectToSelect)) {
            Entity selectable = objectToSelect.transform.GetComponentInChildren<Entity>();
            if(selectable == null)
                objectToSelect.transform.GetComponentInParent<Entity>();

            if (selectable == null)
                return false;

            //Check if there was an obj previously selected
            if (!selectable.active)
                return false;

            if (SelectedObjects.Count > 0 && clearSelect)
                ClearSelection();
            //If yes deselect that object.

            if (SelectedObjects.Contains(selectable)) {
                return false;
            }
            //Select the new object
            SelectedObjects.Add(selectable);
            if(selectable.entityCore.TYPE == EntityType.PRODUCTION) {
                CMND.cmd_building.ActivateBuildingContextMenu((ProductionBuilding)selectable);

            }
            objectToSelect.SendMessage("SelectObject", SendMessageOptions.DontRequireReceiver);
            FillSelectedUnitList();
            return true;
        }
        else {
            return false;
        }
    }


}

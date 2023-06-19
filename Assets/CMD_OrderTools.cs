using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMD_OrderTools : CMDScript
{
    public bool logicToolsEnabled = false;
    public bool logicLine = false;
    public RectTransform logicRect;

    Vector3 mousePos1;
    Vector3 mousePos2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

   

    // Update is called once per frame
    void Update()
    {
        CreateLogicLine();
        
    }

   

    public void CreateLineOrders(List<Vector3> linePoints) {
        for (int i = 0; i < linePoints.Count; i++) {

            ((Unit)CMND.cmd_selectionmanager.SelectedObjects[i]).MoveOrder(linePoints[i]);
        }
    }

    void CreateLogicLine() {
        if (!logicLine)
            return;

        if (Input.GetMouseButtonDown(0)) {
            mousePos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.point != mousePos1) {
                    return;
                }
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            mousePos2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (mousePos1 != mousePos2) {
                EvaluateLogicLine();
            }
        }
    }

    void EvaluateLogicLine() {
        Rect selectionRect = new Rect(mousePos1.x, mousePos1.y, mousePos2.x - mousePos1.x, mousePos2.y - mousePos1.y);
    }

    private void OnDrawGizmos() {

        Gizmos.DrawLine(Camera.main.WorldToScreenPoint(mousePos1), Camera.main.WorldToScreenPoint(mousePos2));
    }
}


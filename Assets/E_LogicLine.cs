using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class E_LogicLine : MonoBehaviour
{
    public CMD CMND;

    private RectTransform logicRect;
    Vector3 startPos;
    Vector3 endPos;
    Vector3 endWorldPos;

    UIE_LineRenderer lineRenderer;

    public int divisionCount = 7;

    // Start is called before the first frame update
    void Start() {
        CMND = FindObjectOfType<CMD>();

        logicRect = CMND.cmd_ordertools.logicRect;
        lineRenderer = logicRect.GetComponent<UIE_LineRenderer>();
    }

    // Update is called once per frame
    bool started;

    void Update() {
        if (isLogicLineReady()) {

            GetMouseInformation();



            if (Input.GetMouseButton(0) && started) {
                UpdateLine();
            }

            UpdateLinePointsToLivePos();
        }
        else {
            if (!lineRenderer.isLineIdle()) {
                lineRenderer.ResetLine();
            }
        }


        
        
    }

    public bool isLogicLineReady() {
        if (!CMND.cmd_ordertools.logicToolsEnabled || !CMND.cmd_ordertools.logicLine || (!started && CMD_UI.IsPointerOverUIObject()) || logicRect == null)
            return false;

        return true;
    }

    void UpdateLine() {

        divisionCount = CMND.cmd_selectionmanager.SelectedObjects.Count;

        if (!logicRect.gameObject.activeInHierarchy)
            logicRect.gameObject.SetActive(true);

        endPos = Input.mousePosition;
        endWorldPos = CMND.cmd_selectionmanager.GetCursorWorldPosition();
        Vector3 squareStart = Camera.main.WorldToScreenPoint(startPos);
        squareStart.z = 0;

        Vector3 center = (squareStart + endPos) / 2f;

        //logicRect.position = center;

        float sizeX = Mathf.Abs(squareStart.x - endPos.x);
        float sizeY = Mathf.Abs(squareStart.y - endPos.y);

        //logicRect.sizeDelta = new Vector2(sizeX, sizeY);



        lineRenderer.points[0] = (Camera.main.WorldToScreenPoint(startPos) / new Vector2(Screen.width, Screen.height));
        lineRenderer.points[1] = (Camera.main.WorldToScreenPoint(CMND.cmd_selectionmanager.GetCursorWorldPosition()) / new Vector2(Screen.width, Screen.height));
        lineRenderer.SetVerticesDirty();
    }

    void GetMouseInformation() {
        GameObject GOatCursor = CMND.cmd_selectionmanager.GetGameObjectAtCursor();
        if (Input.GetMouseButtonDown(0) && GOatCursor != null) {
            startPos = CMND.cmd_selectionmanager.GetCursorWorldPosition();
            started = true;
        }
        if (Input.GetMouseButtonUp(0)) {
            started = false;
            SendLineData();
        }
    }



    void SendLineData() {
        if (currentLinePoints.Count == CMND.cmd_selectionmanager.SelectedObjects.Count && !started) {
            CMD.CMND.cmd_ordertools.CreateLineOrders(currentLinePoints);
        }
    }

    public List<Vector3> currentLinePoints = new List<Vector3>();

    void UpdateLinePointsToLivePos() {


        if (lineRenderer.points.Count >= 2) {

            Vector2 p1 = (Camera.main.WorldToScreenPoint(startPos) / new Vector2(Screen.width, Screen.height));

            


            Vector2 p2 = (Camera.main.WorldToScreenPoint(endWorldPos) / new Vector2(Screen.width, Screen.height));

            currentLinePoints.Clear();

           
            //gizmoPos.Add(endWorldPos);

            float segDist = Vector3.Distance(endWorldPos, startPos)/(divisionCount-1);
            for (int i = 0; i < divisionCount; i++) {
                float iDist = i * segDist;


                Vector3 dir = (endWorldPos-startPos).normalized;
                currentLinePoints.Add(startPos + (dir * iDist));
            }


            lineRenderer.points[0] = p1;
            lineRenderer.points[1] = p2;
            lineRenderer.SetVerticesDirty();

            if (p1.x >= 0 && p1.y >= 0 && p2.x >= 0 && p2.y >= 0 && p1.x <= 1 && p1.y <= 1 && p2.x <= 1 && p2.y <= 1) {
                
            }
            
        }
    }

    private void OnDrawGizmos() {
        foreach(Vector3 pos in currentLinePoints) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos, 1f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour
{
    public CMD CMND;

    private RectTransform selectSquare;
    Vector3 startPos;
    Vector3 endPos;

    // Start is called before the first frame update
    void Start()
    {
        CMND = FindObjectOfType<CMD>();

        selectSquare = CMND.cmd_selectionmanager.selectionBox;
        selectSquare.GetComponent<Image>().enabled = true;
        selectSquare.gameObject.SetActive(false);

    }

    // Update is called once per frame
    bool started;

    void Update(){
        if (!CMND.cmd_selectionmanager.selectionEnabled || (!started && CMD_UI.IsPointerOverUIObject()))
            
            return;


        if (selectSquare == null)
            return;

        GameObject GOatCursor = CMND.cmd_selectionmanager.GetGameObjectAtCursor();
        if (Input.GetMouseButtonDown(0) && GOatCursor != null) {
            startPos = CMND.cmd_selectionmanager.GetCursorWorldPosition();
            started = true;
        }
        if (Input.GetMouseButtonUp(0)) {
            selectSquare.gameObject.SetActive(false);
            started = false;
        }



        if (Input.GetMouseButton(0) && started) {
            if(!selectSquare.gameObject.activeInHierarchy )
                selectSquare.gameObject.SetActive(true);

            endPos = Input.mousePosition;
            Vector3 squareStart = Camera.main.WorldToScreenPoint(startPos);
            squareStart.z = 0;

            Vector3 center = (squareStart + endPos) / 2f;

            selectSquare.position = center;

            float sizeX = Mathf.Abs(squareStart.x - endPos.x);
            float sizeY = Mathf.Abs(squareStart.y - endPos.y);

            selectSquare.sizeDelta = new Vector2(sizeX, sizeY);
        }
    }
}

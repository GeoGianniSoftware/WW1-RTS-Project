using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : Building
{
    public bool occupied = false;
    public StationCore stationCore;

    public bool showFireRadius = true;
    public float distance;
    public float maxAngle;
    public float heightOffset;

    public override EntityCore getEntityCore() {
        return stationCore;
    }

    private void Update() {
        if(showFireRadius)
            DrawAimCone();
        else {
            HideAimCone();
        }
    }


    public override void PlaceBuilding(BuildingCore core, bool skip = false) {
        base.PlaceBuilding(core, skip);
        showFireRadius = false;
    }



    private void OnValidate() {

    }

    public override void Interact(Entity entity) {

    }

   



    List<Vector3> pointsTemp = new List<Vector3>();
    List<Vector2> points2DTemp = new List<Vector2>();
    private void DrawAimCone() {

        if (GetComponent<LineRenderer>())
            GetComponent<LineRenderer>().positionCount = 0;
        else
            return;

        pointsTemp = new List<Vector3>();
        points2DTemp = new List<Vector2>();

        pointsTemp.Add(transform.position + new Vector3(0, heightOffset, 0));
        points2DTemp.Add(new Vector2(transform.position.x, transform.position.z));


        for (float i = 0; i < maxAngle; i += 1) {
            Vector3 directionVector = GetDirectionVectorAroundVector(i - (maxAngle / 2) + transform.eulerAngles.y, transform.up);
            Vector3 dir = transform.position + directionVector * distance + new Vector3(0,heightOffset,0);

            Ray ray = new Ray(transform.position + new Vector3(0, heightOffset, 0), directionVector);
            RaycastHit[] hits;

            hits = Physics.RaycastAll(ray, distance);

            for (int x = 0; x < hits.Length; x++) {
                if(hits[x].transform.root != this.transform.root) {
                    pointsTemp.Add(hits[x].point);
                    points2DTemp.Add(new Vector2(hits[x].point.x, hits[x].point.z));
                    goto fin;
                }
            }
            


            pointsTemp.Add(dir);
            points2DTemp.Add(new Vector2(dir.x, dir.z));
            fin:
            continue;
            

        }

        if (GetComponent<LineRenderer>()) {
            GetComponent<LineRenderer>().positionCount = pointsTemp.Count + 1;
            for (int i = 0; i < pointsTemp.Count; i++) {
                GetComponent<LineRenderer>().SetPosition(i, pointsTemp[i]);
            }

            GetComponent<LineRenderer>().SetPosition(pointsTemp.Count, transform.position + new Vector3(0, heightOffset, 0));
        }
            
    }
    private void HideAimCone() {
        if (GetComponent<LineRenderer>() && GetComponent<LineRenderer>().positionCount > 0) {
            GetComponent<LineRenderer>().positionCount = 0;
        }
    }
    private void OnDrawGizmos() {
        if (connectedBuildings.Count > 0 && connectedBuildings[0] != null) {
            Gizmos.DrawLine(transform.position, connectedBuildings[0].transform.position);
        }
            
    }

    public Vector3 GetDirectionVectorAroundVector(float fDirection, Vector3 vNormal) {
        //Rotates the normale vector to get the vector of 0 degrees.
        Quaternion _q1 = Quaternion.Euler(90f, 0f, 0f);
        Vector3 _fwVector = _q1 * vNormal;
        //Then rotates that around the normalvector
        Quaternion _q2 = Quaternion.AngleAxis(fDirection, vNormal);
        return _q2 * _fwVector;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoBall : MonoBehaviour
{
    [Range(0f,360f)]
    public float maxAngle = 360;

    public float incrementSize = 1;
    public float distance;
    public Gradient scanColor;

    public List<Vector3> hitPoints = new List<Vector3>();
    public List<Vector2> hitPoints2D = new List<Vector2>();

    public bool sendit = false;

    private void OnValidate() {
        if (sendit) {

            

            sendit = false;
        }
    }

  

   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected() {
        List<Vector3> pointsTemp = new List<Vector3>();
        List<Vector2> points2DTemp = new List<Vector2>();

        pointsTemp.Add(transform.position);
        points2DTemp.Add(new Vector2(transform.position.x, transform.position.z));
        GetComponent<LineRenderer>().positionCount = 0;
        for (float i = 0; i < maxAngle; i+=incrementSize) {
            
            Vector3 directionVector = GetDirectionVectorAroundVector(i - (maxAngle/2), transform.up);
            Vector3 dir = transform.position + directionVector * distance;

            Ray ray = new Ray(transform.position, directionVector);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, distance)){
                float dst = Vector3.Distance(transform.position, hit.point);
                Color c = scanColor.Evaluate(dst / distance);

                Gizmos.color = c;
                Gizmos.DrawCube(hit.point, Vector3.one);
                
                Gizmos.DrawLine(transform.position, hit.point);

                pointsTemp.Add(hit.point);
                points2DTemp.Add(new Vector2(hit.point.x, hit.point.z));


            }
            else {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, dir);
                pointsTemp.Add(dir);
                points2DTemp.Add(new Vector2(dir.x, dir.z));
            }

            

            
        }
        GetComponent<LineRenderer>().positionCount = pointsTemp.Count+1;
        for (int i = 0; i < pointsTemp.Count; i++) {
            GetComponent<LineRenderer>().SetPosition(i, pointsTemp[i]);
        }

        GetComponent<LineRenderer>().SetPosition(pointsTemp.Count, transform.position);

        hitPoints = pointsTemp;
        hitPoints2D = points2DTemp;
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

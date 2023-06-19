using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    
    NavMeshAgent NMA;
    Animator Anim;
    public float speed;
    public int ammoCount = 5;
    public bool isReloading = false;
    public bool isFiring = false;

    private void Start() {
        NMA = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100)) {
                SetWaypoint(hit.point);
            }


            
        }
        NMA.speed = speed;
        if (NMA.isActiveAndEnabled) {
            Anim.SetFloat("speed", NMA.velocity.magnitude);

            if (Input.GetMouseButton(1)) {
                if(ammoCount > 0 && !isReloading) {
                    StartFire();
                }
                else {
                    isFiring = false;
                    Reload();
                }
            }
            else {
                isFiring = false;
            }

            if (isFiring) {
                Vector3 target = NMA.destination + transform.forward;
                target.y = transform.position.y;
                transform.LookAt(target);
            }
        }




        Anim.SetBool("firing", isFiring);
    }

    public void FinishReload() {
        isReloading = false;
        ammoCount = 5;
    }

    public void Reload() {
        if (!isReloading) {
            Anim.SetTrigger("reloading");
            isReloading = true;
        }
    }

    public void Fire() {
        ammoCount--;
    }

    public void StartFire() {
        isFiring = true;
        NMA.speed = speed / 2;
    }

    public void SetWaypoint(Vector3 pos) {
        
        NMA.SetDestination(pos);
    }
}

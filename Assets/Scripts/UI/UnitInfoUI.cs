using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoUI : MonoBehaviour
{
    public Entity connectedEntity;
    public Image healthbarFill;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthbarFill.fillAmount = connectedEntity.getHealthPercent();
        transform.LookAt(Camera.main.transform.position);
    }
}

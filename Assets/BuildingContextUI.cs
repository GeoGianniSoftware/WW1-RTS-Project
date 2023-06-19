using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingContextUI : MonoBehaviour
{
    public ProductionBuilding attachedBuilding;

    [Header("Information")]
    public TMPro.TextMeshProUGUI nameText;
    public TMPro.TextMeshProUGUI informationText;

    [Header("Current Production")]
    public Image currentProductionImage;
    public TMPro.TextMeshProUGUI currentProductionText;
    public TMPro.TextMeshProUGUI currentProductionTimeLeftText;
    public Image currentProductionProgressBar;

    [Header("Production List")]
    public GameObject productionListContent;
    public List<GameObject> productionListFrames = new List<GameObject>();
    


    public void Activate(ProductionBuilding building) {
        attachedBuilding = building;
        Initialize();
        UpdateValues();

    }

    void UpdateValues() {
        if (attachedBuilding != null && attachedBuilding.currentProductionIndex >= 0 && attachedBuilding.getCurrentProductionCore() != null) {
            EntityCore core = attachedBuilding.getCurrentProductionCore();
            currentProductionImage.sprite = core.cardSprite;
            currentProductionText.text = core.Name;
            currentProductionTimeLeftText.text = "" + (attachedBuilding.getCurrentProductionPercentage() * 100) + "%";
            currentProductionProgressBar.fillAmount = attachedBuilding.getCurrentProductionPercentage();

            informationText.text = "Level: 1";
            informationText.text += "\nProduction Amount: " + attachedBuilding.getProductionCore().productionPerSecond;
        }
    }

    void Initialize() {
        transform.localScale = new Vector3(transform.localScale.x*-1, transform.localScale.y, transform.localScale.z);


        for (int i = 0; i < attachedBuilding.getProductionCore().productionList.Count; i++) {
            GameObject g = Instantiate(Resources.Load<GameObject>("UI/EntityFrameButton"), productionListContent.transform);
            g.transform.GetChild(0).GetComponent<Image>().sprite = attachedBuilding.getProductionCore().productionList[i].cardSprite;
            g.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "<color=red>(<u>"+ attachedBuilding.getProductionCore().productionList[i].productionCost+ "</u>)</color> "+ attachedBuilding.getProductionCore().productionList[i].Name;
            productionListFrames.Add(g);
            addOnClickSetProduction(i, g.GetComponent<Button>());
        }
    }

    public void setCurrentProductionIndex(int index) {
        if(attachedBuilding.currentProductionIndex != index)
            attachedBuilding.setCurrentProduction(index);
    }

    public void addOnClickSetProduction(int index, Button b) {
        b.onClick.AddListener(delegate { setCurrentProductionIndex(index); });
    }

    private void Update() {
       transform.LookAt(Camera.main.transform.position);


        UpdateValues();
    }
}

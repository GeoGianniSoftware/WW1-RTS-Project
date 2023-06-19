using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CurrentResearchFrame : MonoBehaviour
{

    public TextMeshProUGUI researchTitleText;
    public TextMeshProUGUI researchProgressText;
    public Image researchFillbar;
    

    public float researchProgress;
    // Start is called before the first frame update
    public void Setup(string title)
    {
        researchTitleText.text = title;
    }

    public void UpdateDisplay(string title) {
        researchTitleText.text = title;
    }

    // Update is called once per frame
    void Update()
    {
        researchFillbar.fillAmount = researchProgress;
        researchProgressText.text = "" + Mathf.RoundToInt((researchProgress * 100f)) + "%";
    }

}

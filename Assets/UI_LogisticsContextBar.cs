using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LogisticsContextBar : MonoBehaviour
{
    public TMPro.TextMeshProUGUI logiIndex;
    public TMPro.TextMeshProUGUI logiName;
    public Image logiSourceImage;
    public Image logiDestImage;
    public Image logiTypeImage;
    public Button removeButton;

    public LogisticLink attachedLink;

    public void PopulateUI(LogisticLink link) {
        attachedLink = link;
        logiIndex.text =(attachedLink.routeID + 1) +".";
        logiName.text = attachedLink.linkType.ToString() + " Route.";
        logiSourceImage.sprite = attachedLink.origin.entityCore.cardSprite;
        logiDestImage.sprite = attachedLink.destination.entityCore.cardSprite;
        if(attachedLink.linkType == LogisticLink.LinkTypes.SupplyLink) {
            logiTypeImage.color = Color.cyan;
        }else if (attachedLink.linkType == LogisticLink.LinkTypes.Ammunition) {
            logiTypeImage.color = Color.red;
        }else if (attachedLink.linkType == LogisticLink.LinkTypes.ProductionLink) {
            logiTypeImage.color = Color.green;
        }
        removeButton.onClick.AddListener(delegate { CMD.CMND.cmd_logistics.RemoveLogicLink(attachedLink.routeID); } );

    }
}

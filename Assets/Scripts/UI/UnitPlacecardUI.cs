using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPlacecardUI : MonoBehaviour
{
    public GameObject outline;
    public Image unitCardImage;
    public Image unitCardHealthbar;
    public int amount = 1;
    public TMPro.TextMeshProUGUI amountText;
    public Entity attachedEntity;

    public void Activate(Entity e) {
        attachedEntity = e;
        unitCardImage.sprite = e.entityCore.cardSprite;
    }

    public void setAmount(int amt) {
        amount = amt;
        if(amt == 1) {
            amountText.text = "";
            return;
        }
        amountText.text = "x" + amt;
    }

    public void incrementAmount() {
        amount++;
        amountText.text = "x" + (amount+1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyTechPopup : Popup {
    [HideInInspector]
    public int techLevel, techCategory;
    [HideInInspector]
    public TechRow sender;

    private int cost;

    public Text costText, effectText;
    public Image effectIcon;
    public Button purchaseButton;

    void Start() {
        cost = Controller.CalculateTechPrices(techCategory, techLevel);
        costText.text = cost.ToString();
        //check if player has enough money
        if (cost > PlayerData.instance.playerData.money) {
            //costText.color = Color.red;
            purchaseButton.interactable = false;
        }
        //7 different improvement categories for upgrades; one upgrade should only change one
        for (int i = 0; i < 7; i++) {
            //print(techCategory + ", " + techLevel + ", " + i);
            //difference between current tech level and previous one (if level 0 then difference is just current tech bonus)
            int difference = techLevel == 0 ? Controller.techLevels[techCategory][techLevel][i] : (Controller.techLevels[techCategory][techLevel][i] - Controller.techLevels[techCategory][techLevel - 1][i]);
            if (difference != 0) {
                effectIcon.sprite = sender.effectIcons[i];
                //display minus sign for saving fuel
                effectText.text = (i == (int)Controller.TechCategory.Fuel ? "-" : "+") + difference;
                break;
            }
        }
    }
    public void BuyTech() {
        PlayerData.instance.playerData.money -= cost;
        PlayerData.instance.playerData.techLevels[techCategory] = techLevel + 1;
        PlayerData.instance.saveFile();

        sender.UpdateButtons();
        if (sender.nextYear) sender.nextYear.UpdateButtons();

        Dismiss();
    }
}

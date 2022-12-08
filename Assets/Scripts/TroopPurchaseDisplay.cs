using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class TroopPurchaseDisplay : MonoBehaviour {
    public ProductBar productController;
    public Image coinIcon, industryIcon;
    public Sprite coinSprite, oilSprite; 
    public Text unitNameDisplay, manpowerCostDisplay, industryCostDisplay;
    public Button buyButton; //disable if insufficient cost
    public int productId;

    [HideInInspector]
    public int unitManpowerCost, unitIndustryCost, unitOilCost; //changes based on tier

    public void buyUnit() {
        if (productController.selectedCategory != PurchaseCategory.nuclear) {
            if (Controller.instance.selectedSoldier != null) {
                Controller.instance.selectedSoldier.selected = false;
                Controller.instance.selectedSoldier = null;
            }
            Controller.instance.deselectTile(false, false);
            productController.purchaseProduct(productId);
            if (!Controller.instance.airplaneMode) {
                Controller.instance.selectedCity = null;
            }
            Controller.instance.buildButtonOn = false;
        } else {
            productController.purchaseProduct(productId);
        }
    }
    void Start() {
    }

    void Update() {
        if (productController.controller.editMode)
            return;
        if (productController.selectedCategory == PurchaseCategory.aerial) {
            coinIcon.sprite = oilSprite;
        } else
            coinIcon.sprite = coinSprite;  
        if (productController.airportTroopTypes.ContainsValue(productId) && productController.selectedCategory == PurchaseCategory.aerial) {
            int myOilCost = unitOilCost;
            int myIndustryCost = unitIndustryCost;

            if (productId == 2) {
                if (productController.controller.selectedCity == null || productController.controller.selectedCity.currentTile.occupant == null || productController.controller.selectedCity.currentTile.occupant.troopType != Troop.infantry) {
                    manpowerCostDisplay.text = "-";
                    industryCostDisplay.text = "-";
                } else {
                    
                    
                    
                    myIndustryCost *= productController.controller.selectedCity.currentTile.occupant.tier;
                    myOilCost *= productController.controller.selectedCity.currentTile.occupant.tier;
                    
                    
                    manpowerCostDisplay.text = myOilCost.ToString();
                    industryCostDisplay.text = myIndustryCost.ToString();
                }
                
            } else {
                manpowerCostDisplay.text = unitOilCost.ToString();
                industryCostDisplay.text = unitIndustryCost.ToString();
            }
            if (productController.controller.countryDatas[productController.controller.playerCountry].industry < myIndustryCost ||
                productController.controller.countryDatas[productController.controller.playerCountry].fuel < myOilCost || manpowerCostDisplay.text == "-")
                buyButton.interactable = false;
            else if (!buyButton.interactable)
                buyButton.interactable = true; 
        } else { 
            manpowerCostDisplay.text = (unitManpowerCost * productController.stackSize).ToString();
            industryCostDisplay.text = (unitIndustryCost * productController.stackSize).ToString();
            if (productController.controller.countryDatas[productController.controller.playerCountry].manpower < unitManpowerCost * productController.stackSize ||
                productController.controller.countryDatas[productController.controller.playerCountry].industry < unitIndustryCost * productController.stackSize ||
                 (productController.selectedCategory == PurchaseCategory.nuclear && Controller.instance.selectedCity != null && Controller.instance.selectedCity.roundsToBombProduction != 0))
                buyButton.interactable = false;
            else if(!buyButton.interactable)
                buyButton.interactable = true;
        }
    }
}
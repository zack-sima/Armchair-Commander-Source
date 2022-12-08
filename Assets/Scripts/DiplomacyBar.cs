using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

public class DiplomacyBar : MonoBehaviour {
    public Image countryDisplay;
    public Text countryNameDisplay;
    public Button manpowerAid, industryAid, fuelAid, warDeclaration;

    [HideInInspector]
    public string country;
    [HideInInspector]
    public Controller controller;

    void Update() {
        if (manpowerAid) {
            if (controller.countryDatas[controller.playerCountry].manpower < 50) {
                if (manpowerAid.interactable)
                    manpowerAid.interactable = false;
            } else {
                if (!manpowerAid.interactable)
                    manpowerAid.interactable = true;
            }
            if (controller.countryDatas[controller.playerCountry].fuel < 50) {
                if (fuelAid.interactable)
                    fuelAid.interactable = false;
            } else {
                if (!fuelAid.interactable)
                    fuelAid.interactable = true;
            }
            if (controller.countryDatas[controller.playerCountry].industry < 50) {
                if (industryAid.interactable)
                    industryAid.interactable = false;
            } else {
                if (!industryAid.interactable)
                    industryAid.interactable = true;
            }
        }
    }
    public void ManpowerAid() {
        if (controller.countryDatas[controller.playerCountry].manpower >= 50) {
            controller.countryDatas[controller.playerCountry].manpower -= 50;
            controller.countryDatas[country].manpower += 50;
        }
    }
    public void IndustryAid() {
        if (controller.countryDatas[controller.playerCountry].industry >= 50) {
            controller.countryDatas[controller.playerCountry].industry -= 50;
            controller.countryDatas[country].industry += 50;
        }
    }
    public void FuelAid() {
        if (controller.countryDatas[controller.playerCountry].fuel >= 50) {
            controller.countryDatas[controller.playerCountry].fuel -= 50;
            controller.countryDatas[country].fuel += 50;
        }
    }
    public void DeclareWar() {
        //will only put AI into the two default alliances
        if (controller.countriesIsNeutral.Contains(controller.playerCountry)) {
            if (controller.countriesIsAxis[country] != 1) {
                ChangeAlliance(1, true);
            } else {
                ChangeAlliance(0, true);
            }
        } else {
            if (controller.playerIsAxis != 1) {
                ChangeAlliance(1);
            } else {
                ChangeAlliance(0);
            }
        }
        foreach (Unit u in controller.soldiers) {
            if (u != null)
                u.CheckCountry();
        }
        
    }
    void ChangeAlliance(int teamNumber, bool forPlayer=false) {
        if (forPlayer) {
            controller.countriesIsAxis[controller.playerCountry] = teamNumber;
            controller.playerIsAxis = teamNumber;
        } else {
            controller.countriesIsAxis[country] = teamNumber;
        }
        

        GameEvent myEvent = new GameEvent();

        if (!controller.countriesIsNeutral.Contains(country) && !controller.countriesIsNeutral.Contains(controller.playerCountry)) {
            //has to be a country that isn't neutral to be betrayal

            myEvent.countryTarget = controller.playerCountry;
            myEvent.eventType = EventType.Health;
            myEvent.eventValue = 5;

            myEvent.title = CustomFunctions.TranslateText("Betrayal!");
            myEvent.description = CustomFunctions.TranslateText("We have betrayed our allies. Our morale and supply were sabotaged.");




            controller.ConsumeEvent(myEvent);
            controller.NewsPopup(myEvent);
        }
        if (forPlayer) {
            if (controller.countriesIsNeutral.Contains(controller.playerCountry))
                controller.countriesIsNeutral.Remove(controller.playerCountry);
        } else {
            if (controller.countriesIsNeutral.Contains(country))
                controller.countriesIsNeutral.Remove(country);
        }

        myEvent = new GameEvent();

        myEvent.countryTarget = country;
        myEvent.eventType = EventType.Diplomacy;
        myEvent.eventValue = teamNumber;

        myEvent.title = CustomFunctions.TranslateText("War!");
        string playerCountry = Controller.CheckCustomFlag(controller.countryCustomNameOverrides, controller.playerCountry) == "" ? controller.playerCountry : Controller.CheckCustomFlag(controller.countryCustomNameOverrides, controller.playerCountry);
        string targetCountry = Controller.CheckCustomFlag(controller.countryCustomNameOverrides, country) == "" ? country : Controller.CheckCustomFlag(controller.countryCustomNameOverrides, country);
        myEvent.description = CustomFunctions.TranslateText(playerCountry) + " " + CustomFunctions.TranslateText("declared war on") + " " + CustomFunctions.TranslateText(targetCountry);


        controller.NewsPopup(myEvent);

    }

}

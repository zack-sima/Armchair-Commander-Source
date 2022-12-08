using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

public class DiplomacyControl : MonoBehaviour {
    [HideInInspector]
    public Controller controller;
    public ScrollRect scroller;

    public DiplomacyBar bar; //acts as a prefab; instance is destoryed after instantiating copies of it
    //only countries with troops or cities are counted
    private List<CountryData> countriesAvailable;



    void Start() {
        countriesAvailable = new List<CountryData>();

        foreach (Unit i in controller.soldiers) {
            if (!countriesAvailable.Contains(controller.countryDatas[i.country]) && i.country != controller.playerCountry) {
                countriesAvailable.Add(controller.countryDatas[i.country]);
            }
        }
        foreach (City i in controller.cities) {
            if (!countriesAvailable.Contains(controller.countryDatas[i.currentTile.country]) && i.currentTile.country != controller.playerCountry) {
                countriesAvailable.Add(controller.countryDatas[i.currentTile.country]);
            }
        }
        countriesAvailable = countriesAvailable.OrderBy(o => controller.countriesIsAxis[o.name]).ToList(); //sorts events by round so only the first element of the round has to be checked

        if (countriesAvailable.Count > 8)
            scroller.content.sizeDelta = new Vector2(scroller.content.sizeDelta.x, countriesAvailable.Count * 75f / 1.17f);
        int index = 0;
        foreach (CountryData i in countriesAvailable) {
            DiplomacyBar insItem = Instantiate(bar.gameObject, bar.transform.position + new Vector3(0f, -index * 75f * Screen.width / 1920, 0f), Quaternion.identity).GetComponent<DiplomacyBar>();
            insItem.transform.localScale = new Vector2(Screen.width / 1920f * 1.17f, Screen.width / 1920f * 1.17f);
            insItem.transform.SetParent(scroller.content);

            insItem.country = i.name;
            insItem.controller = controller;
            insItem.countryNameDisplay.text = Controller.CheckCustomFlag(controller.countryCustomNameOverrides, i.name) == "" ? CustomFunctions.TranslateText(i.name) : CustomFunctions.TranslateText(Controller.CheckCustomFlag(controller.countryCustomNameOverrides, i.name));

            insItem.countryDisplay.sprite = controller.flags[i.name];

            if (!controller.countriesIsNeutral.Contains(controller.playerCountry) && controller.countriesIsAxis[i.name] != controller.playerIsAxis && !controller.countriesIsNeutral.Contains(i.name)) {
                //enemy
                Destroy(insItem.manpowerAid.gameObject);
                Destroy(insItem.industryAid.gameObject);
                Destroy(insItem.fuelAid.gameObject);

                Destroy(insItem.warDeclaration.gameObject);


                insItem.countryNameDisplay.text += " " + CustomFunctions.TranslateText("(At War)");
            } else if (controller.countriesIsNeutral.Contains(controller.playerCountry) || controller.countriesIsNeutral.Contains(i.name)) {
                Destroy(insItem.manpowerAid.gameObject);
                Destroy(insItem.industryAid.gameObject);
                Destroy(insItem.fuelAid.gameObject);

                //neutral
                insItem.countryNameDisplay.text += " " + CustomFunctions.TranslateText("(Neutral)");
            } else {
                //ally

            }

            index++;
        }


        Destroy(bar.gameObject);
    }

    void Update() {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerHosting : MonoBehaviour {
    [HideInInspector]
    public MultiplayerLobby multiplayerControl;
    public Toggle useCustomMaps; //if this is enabled, disable all dropdowns
    public Dropdown mapDropdown, playerCountryDropdown, opponentCountryDropdown;

    List<string> maps;

    List<string> countries;

    void Start() {
        maps = new List<string>();

        //fill map dropdown with editor maps
        countries = new List<string>(CustomFunctions.CountriesIsAxis.Keys);

        mapDropdown.options = new List<Dropdown.OptionData>();
        playerCountryDropdown.options = new List<Dropdown.OptionData>();
        opponentCountryDropdown.options = new List<Dropdown.OptionData>();
        foreach (string i in countries) {
            playerCountryDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i)));
            opponentCountryDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i)));
        }
        playerCountryDropdown.RefreshShownValue();
        opponentCountryDropdown.RefreshShownValue();

        foreach (string j in MyPlayerPrefs.instance.GetString("customDataAll").Split(',')) {
            try {
                int i = int.Parse(j);
                string s = MyPlayerPrefs.instance.GetString("customData" + i);
                if (s != "") {
                    try {
                        mapDropdown.options.Add(new Dropdown.OptionData(JsonUtility.FromJson<MapInfo>(s).missionName));
                        maps.Add(s);
                    } catch (Exception e) {
                        print("map not available: " + e);
                    }
                }
            } catch (Exception e) {
                print("map parse: " + e);
            }
        }
        if (maps.Count == 0) {
            useCustomMaps.interactable = false;
            useCustomMaps.isOn = false;
        }
    }
    void Update() {
        if (!useCustomMaps.isOn || maps.Count == 0) { //either use default or no custom maps to use
            if (playerCountryDropdown.interactable) {
                mapDropdown.interactable = false;
                playerCountryDropdown.interactable = false;
                opponentCountryDropdown.interactable = false;
            }
        } else {
            if (!playerCountryDropdown.interactable) {
                mapDropdown.interactable = true;
                playerCountryDropdown.interactable = true;
                opponentCountryDropdown.interactable = true;
            }
        }

    }
    public void OnMapSelected() {
    }
    public void HostGame() {
        multiplayerControl.StartMatching(false, !useCustomMaps.isOn, useCustomMaps.isOn ? maps[mapDropdown.value] : "",
            countries[playerCountryDropdown.value], countries[opponentCountryDropdown.value]);
        
        MyPlayerPrefs.instance.SetInt("isHost", 1);
        MyPlayerPrefs.instance.SetString("opponentCountry", countries[opponentCountryDropdown.value]);


        Destroy(gameObject);
    }
}

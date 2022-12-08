using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class AlmanacManager : MonoBehaviour {
    public Dropdown countryDropdown, unitDropdown, techDropdown;
    public Text soldierName;
    public RectTransform scrollviewContentRect;
    public ScrollRect scroller;
    public SoldierPrefabsManager troopPrefabsController;
    GameObject insItem = null;
    public Text hp, attack, armor, antiArmor, range, movement;
    public Sprite[] terrainIcons;
    public Sprite infantryIcon, armorIcon, navyIcon, artilleryIcon, fortificationIcon;
    public GameObject[] attackModifiers, defenceModifiers, specialAbilities;
    public void loadScene(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }
    Dictionary<int, string> troopNames;
    void Start() {
        countryDropdown.options = new List<Dropdown.OptionData>();
        foreach (string i in CustomFunctions.CountriesIsAxis.Keys) {
            countryDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i)));
        }
        troopNames = new Dictionary<int, string>();
        foreach (KeyValuePair<string, int> pair in SoldierPrefabsManager.troopTypes) {
            if (pair.Value < 70) {
                troopNames.Add(pair.Value, pair.Key);
            }
        }
        unitDropdown.options = new List<Dropdown.OptionData>();

        foreach (string i in troopNames.Values) {
            unitDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(i)));
        }
        unitDropdown.captionText.text = CustomFunctions.TranslateText(new List<string>(troopNames.Values)[0]);
        SpawnPrefab(0, "Soviet");
    }
    public void OnDropdownUpdate() {
        SpawnPrefab(new List<int>(troopNames.Keys)[unitDropdown.value], new List<string>(CustomFunctions.CountriesIsAxis.Keys)[countryDropdown.value]);
    }
    void Update() {
        //if (Input.GetKeyDown(KeyCode.C)) {
        //    SpawnPrefab(Random.Range(0, troopPrefabsController.troopPrefabs.Length), new List<string>(CustomFunctions.CountriesIsAxis.Keys)[Random.Range(0, CustomFunctions.CountriesIsAxis.Keys.Count)]);
        //}
    }
    void SpawnPrefab(int id, string country) {
        if (insItem != null) {
            Destroy(insItem);
        }
        
        soldierName.text = CustomFunctions.TranslateText(troopNames[id]);

        insItem = Instantiate(troopPrefabsController.troopPrefabs[id], Camera.main.ScreenToWorldPoint(new Vector3(350f * Screen.width / 1920f, Screen.height / 2f + 0f * (Screen.height/(float)Screen.width), 10f)), Quaternion.identity);
        insItem.transform.localScale = Vector3.one * 1.7f * (Screen.width / (float)Screen.height);

        foreach (TankAnimator ta in insItem.GetComponentsInChildren<TankAnimator>())
            ta.enabled = false;

        Unit insSoldier = insItem.GetComponent<Unit>();
        insSoldier.enabled = false;
        insSoldier.country = country;

        insSoldier.UpdateStats(outsideGame: true, manager: troopPrefabsController, techLevel: techDropdown.value);
        insSoldier.updateSkin();
        hp.text = ((int)insSoldier.maxHealth).ToString();
        attack.text = ((int)insSoldier.damage).ToString();
        armor.text = ((int)insSoldier.armor).ToString();
        antiArmor.text = ((int)insSoldier.armorPierce).ToString();
        range.text = insSoldier.range.ToString();
        movement.text = insSoldier.movement.ToString();

        string[] terrainTitles = new string[] {"On Plains", "In Forests", "On Mountains", "In Cities", "not shown", "not shown", "On Deserts", "not shown", "On Snow"}; //snow is yet to be added
        

        //public enum Terrain {plains, forest, mountains, city, water, oil, desert, highMountains}

        //attack modifiers
        int attackModifierCount = 0;
        int index = 0;
        foreach (float i in insSoldier.terrainAttackBonuses) {
            if (insSoldier.terrainAttackBonuses[index] != 0 && !new List<int>() {4, 5, 7}.Contains(index)) { //not part of the terrain bonuses hidden
                attackModifiers[attackModifierCount].SetActive(true);

                attackModifiers[attackModifierCount].transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText(terrainTitles[index]);
                attackModifiers[attackModifierCount].transform.GetChild(1).GetComponent<Text>().text = (int)(insSoldier.terrainAttackBonuses[index] * 100.1f) + "%";
                attackModifiers[attackModifierCount].GetComponent<Image>().sprite = terrainIcons[index];

                attackModifierCount++;
            }
            index++;
        }
        
        if (insSoldier.infantryAttackBonus != 0 && attackModifierCount < 7) {
            attackModifiers[attackModifierCount].SetActive(true);

            attackModifiers[attackModifierCount].transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("VS Infantry");
            attackModifiers[attackModifierCount].transform.GetChild(1).GetComponent<Text>().text = (int)(insSoldier.infantryAttackBonus * 100.1f) + "%";
            attackModifiers[attackModifierCount].GetComponent<Image>().sprite = infantryIcon;

            attackModifierCount++;
        }
        if (insSoldier.armorAttackBonus != 0 && attackModifierCount < 7) {
            attackModifiers[attackModifierCount].SetActive(true);

            attackModifiers[attackModifierCount].transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("VS Armor");
            attackModifiers[attackModifierCount].transform.GetChild(1).GetComponent<Text>().text = (int)(insSoldier.armorAttackBonus * 100.1f) + "%";
            attackModifiers[attackModifierCount].GetComponent<Image>().sprite = armorIcon;

            attackModifierCount++;
        }
        if (insSoldier.artilleryAttackBonus != 0 && attackModifierCount < 7) {
            attackModifiers[attackModifierCount].SetActive(true);

            attackModifiers[attackModifierCount].transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("VS Artillery");
            attackModifiers[attackModifierCount].transform.GetChild(1).GetComponent<Text>().text = (int)(insSoldier.artilleryAttackBonus * 100.1f) + "%";
            attackModifiers[attackModifierCount].GetComponent<Image>().sprite = artilleryIcon;

            attackModifierCount++;
        }
        if (insSoldier.navalAttackBonus != 0 && attackModifierCount < 7 ) {
            attackModifiers[attackModifierCount].SetActive(true);

            attackModifiers[attackModifierCount].transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("VS Ships");
            attackModifiers[attackModifierCount].transform.GetChild(1).GetComponent<Text>().text = (int)(insSoldier.navalAttackBonus * 100.1f) + "%";
            attackModifiers[attackModifierCount].GetComponent<Image>().sprite = navyIcon;

            attackModifierCount++;
        }
        if (insSoldier.fortificationAttackBonus != 0 && attackModifierCount < 7) {
            attackModifiers[attackModifierCount].SetActive(true);

            attackModifiers[attackModifierCount].transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("VS Fortifications");
            attackModifiers[attackModifierCount].transform.GetChild(1).GetComponent<Text>().text = (int)(insSoldier.fortificationAttackBonus * 100.1f) + "%";
            attackModifiers[attackModifierCount].GetComponent<Image>().sprite = fortificationIcon;

            attackModifierCount++;
        }
        if (attackModifierCount < 7 && id == 12) {
            attackModifiers[attackModifierCount].SetActive(true);

            attackModifiers[attackModifierCount].transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("VS Heavy Ships");
            attackModifiers[attackModifierCount].transform.GetChild(1).GetComponent<Text>().text = "35%";
            attackModifiers[attackModifierCount].GetComponent<Image>().sprite = navyIcon;

            attackModifierCount++;

        }
        if (attackModifierCount < 7 && id == 11) {
            attackModifiers[attackModifierCount].SetActive(true);

            attackModifiers[attackModifierCount].transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("VS Submarines");
            attackModifiers[attackModifierCount].transform.GetChild(1).GetComponent<Text>().text = "35%";
            attackModifiers[attackModifierCount].GetComponent<Image>().sprite = navyIcon;

            attackModifierCount++;
        }
//        print(attackModifierCount);
        for (int i = attackModifierCount; i < 7; i++) {
            attackModifiers[i].SetActive(false);
        }
        //defence modifiers
        int defenceModifierCount = 0;
        index = 0;
        foreach (float i in insSoldier.terrainDefenceBonuses) {
            if (insSoldier.terrainDefenceBonuses[index] != 0 && !new List<int>() { 4, 5, 7 }.Contains(index)) { //not part of the terrain bonuses hidden
                defenceModifiers[defenceModifierCount].SetActive(true);

                defenceModifiers[defenceModifierCount].transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText(terrainTitles[index]);
                defenceModifiers[defenceModifierCount].transform.GetChild(1).GetComponent<Text>().text = (int)(insSoldier.terrainDefenceBonuses[index] * 100.1f) + "%";
                defenceModifiers[defenceModifierCount].GetComponent<Image>().sprite = terrainIcons[index];

                defenceModifierCount++;
            }
            index++;
        }
        for (int i = defenceModifierCount; i < 7; i++) {
            defenceModifiers[i].SetActive(false);
        }

        int highestModifierCount = attackModifierCount > defenceModifierCount ? attackModifierCount : defenceModifierCount;
        if (highestModifierCount == 0)
            highestModifierCount = 1;

        float noSpecialsOffset = 0f;
        if (new List<int>() { 3, 7, 12, 14, 15, 18, 20 }.Contains(id)) {
            specialAbilities[0].transform.parent.gameObject.SetActive(true);
            specialAbilities[0].SetActive(true);
            if (id == 18) {
                specialAbilities[1].SetActive(true);
            } else {
                noSpecialsOffset = 50f;
                specialAbilities[1].SetActive(false);
            }
        } else {
            noSpecialsOffset = 205f;
            specialAbilities[0].SetActive(false);
            specialAbilities[1].SetActive(false);

            specialAbilities[0].transform.parent.gameObject.SetActive(false);
        }
        //float scale = 1;
        Transform t = specialAbilities[0].transform.parent;
        Vector3 p = attackModifiers[highestModifierCount - 1].transform.position;
        t.position = new Vector2(t.position.x, p.y - 82.5f * CustomFunctions.getUIScale());

        scrollviewContentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1035f - 50f * (attackModifiers.Length - highestModifierCount) - noSpecialsOffset);
        scroller.verticalScrollbar.value = 1;
    }
}

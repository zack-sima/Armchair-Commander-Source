using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralManagerPopup : MonoBehaviour {
    [HideInInspector]
    public Shop1Manager controller;

    [HideInInspector]
    public PlayerData playerData;
    public Image portrait;
    public Button buyButton; //or upgrade
    public Text infAtkText, armorAtkText, artyAtkText, airAtkText, navyAtkText, mobText, healthBonusText, cmdSizeText, generalNameText;

    public Text infAtkTextAdd, armorAtkTextAdd, artyAtkTextAdd, airAtkTextAdd, navyAtkTextAdd, mobTextAdd, healthBonusTextAdd, cmdSizeTextAdd;
    public Image perk1, perk2, perk3;
    public GameObject perkTextPrefab;

    [HideInInspector]
    public string generalName;

    int GetGeneralLevel(string general) {
        if (playerData.playerData.generals.ContainsKey(generalName)) {
            return playerData.playerData.generals[generalName];
        }
        return 0;
    }
    public static string GetPerkText(General.GeneralPerk perk, int level) {
        string text = "";
        switch (perk) {
        case General.GeneralPerk.None:
            text = CustomFunctions.TranslateText("No ability");
            break;
        case General.GeneralPerk.Infantry:
            text = CustomFunctions.TranslateText("Infantry Commander: infantry critical hit chance") + " " + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.Armor:
            text = CustomFunctions.TranslateText("Panzer Commander: tank critical hit chance") + " " + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.Artillery:
            text = CustomFunctions.TranslateText("Artillery Commander: artillery critical hit chance") + " " + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.Navy:
            text = CustomFunctions.TranslateText("Navy Commander: navy critical hit chance") + " " + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.Plains:
            text = CustomFunctions.TranslateText("Open Warfare Expert: Attack on plains +") + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.Forest:
            text = CustomFunctions.TranslateText("Forest Ranger: attack in forests +") + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.Mountains:
            text = CustomFunctions.TranslateText("Mountaineer: attack on mountains +") + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.City:
            text = CustomFunctions.TranslateText("Urban Expert: attack in cities +") + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.Desert:
            text = CustomFunctions.TranslateText("Desert Fox: attack on deserts +") + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.Snow:
            text = CustomFunctions.TranslateText("Tundra Expert: attack on snow +") + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.Blitzkrieg:
            text = CustomFunctions.TranslateText("Mobile Warfare: tank attacks avoid retaliation chance") + " " + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.Guerilla:
            text = CustomFunctions.TranslateText("Guerilla Warfare: infantry attacks avoid retaliation chance") + " " + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.Training:
            text = CustomFunctions.TranslateText("Training: experience gain +") + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.DefenseExpert:
            text = CustomFunctions.TranslateText("Defense Expert: damage from infantry and tanks -") + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.ShelterExpert:
            text = CustomFunctions.TranslateText("Shelter Expert: Damage from artillery and air -") + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.Logistics:
            text = CustomFunctions.TranslateText("Logistics: infantry healing per turn +") + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        case General.GeneralPerk.Mechanic:
            text = CustomFunctions.TranslateText("Mechanic: tanks, artillery, and navy healing per turn +") + (GeneralPerksCalculation(perk, level) * 100) + "%";
            break;
        }
        return text;
    }

    public void ShowPerkText(int perkNumber) {
        General.GeneralPerk perk = General.GeneralPerk.None;

        if (perkNumber == 1) { perk = playerData.generals[generalName].perk1; }
        if (perkNumber == 2) { perk = playerData.generals[generalName].perk2; }
        if (perkNumber == 3) { perk = playerData.generals[generalName].perk3; }

        Popup p = Instantiate(perkTextPrefab, GameObject.Find("Canvas").transform).GetComponent<Popup>();
        p.transform.position = Input.mousePosition;

        p.texts[0].text = GetPerkText(perk, playerData.playerData.generals.ContainsKey(generalName) ? playerData.playerData.generals[generalName] : 0);
    }
    public static float GeneralPerksCalculation(General.GeneralPerk perk, int level) {
        level++; //starts at 0
        switch (perk) {
        case General.GeneralPerk.Infantry: //crit chance
            return level * 0.05f; 
        case General.GeneralPerk.Armor:
            return level * 0.04f;
        case General.GeneralPerk.Artillery:
            return level * 0.04f;
        case General.GeneralPerk.Navy:
            return level * 0.05f;
        case General.GeneralPerk.Plains: //damage modifier
            return level * 0.04f;
        case General.GeneralPerk.Forest:
            return level * 0.05f;
        case General.GeneralPerk.Mountains:
            return level * 0.05f;
        case General.GeneralPerk.Snow:
            return level * 0.05f;
        case General.GeneralPerk.Desert:
            return level * 0.05f;
        case General.GeneralPerk.City:
            return level * 0.05f;
        case General.GeneralPerk.Blitzkrieg: //change for no retaliation (armor)
            return level * 0.09f;
        case General.GeneralPerk.Guerilla: //change for no retaliation (infantry)
            return level * 0.09f;
        case General.GeneralPerk.Training: //xp gain rate
            return level * 0.2f;
        case General.GeneralPerk.DefenseExpert: //infantry/armor defense modifier (-%*100)
            return level * 0.06f;
        case General.GeneralPerk.ShelterExpert: //artillery/air defense modifier (-%*100)
            return level * 0.07f;
        case General.GeneralPerk.Logistics: //healing per turn
            return level * 0.015f;
        case General.GeneralPerk.Mechanic: //healing per turn
            return level * 0.01f;
        default:
            Debug.LogWarning("no perk found: " + perk.ToString());
            return 0f;
        }
    }
    public void buyOrUpgradeGeneral() {
        if (playerData.playerData.generals.ContainsKey(generalName)) {
            //upgrade

            //not max level
            if (playerData.playerData.generals[generalName] < playerData.generals[generalName].cost.Length - 1) {
                if (!(playerData.playerData.money < playerData.generals[generalName].cost[playerData.playerData.generals[generalName] + 1])) {
                    playerData.playerData.money -= playerData.generals[generalName].cost[playerData.playerData.generals[generalName] + 1];
                    playerData.playerData.generals[generalName]++;
                    playerData.saveFile();
                }
            }
        } else {
            //buy new
            if (playerData.playerData.money < playerData.generals[generalName].cost[0] == false) {
                playerData.playerData.money -= playerData.generals[generalName].cost[0];
                playerData.playerData.generals.Add(generalName, 0);
                playerData.SortGenerals();
                playerData.saveFile();

            }
        }
        controller.updateGenerals();
    }
    void Start() {
        portrait.sprite = playerData.generalPhotos[generalName];

        perk1.sprite = PlayerData.instance.generalPerkSprites[(int)playerData.generals[generalName].perk1];
        perk2.sprite = PlayerData.instance.generalPerkSprites[(int)playerData.generals[generalName].perk2];
        perk3.sprite = PlayerData.instance.generalPerkSprites[(int)playerData.generals[generalName].perk3];

        Update();
    }
    public void dismissPopup() {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update() {
        if (playerData.playerData.generals.ContainsKey(generalName)) {
            infAtkText.text = ((int)(playerData.generals[generalName].infAtk[playerData.playerData.generals[generalName]] + 100f)).ToString() + "%";
            armorAtkText.text = ((int)(playerData.generals[generalName].armorAtk[playerData.playerData.generals[generalName]] + 100f)).ToString() + "%";
            artyAtkText.text = ((int)(playerData.generals[generalName].artilleryAtk[playerData.playerData.generals[generalName]] + 100f)).ToString() + "%";
            airAtkText.text = ((int)(playerData.generals[generalName].airAtk[playerData.playerData.generals[generalName]] + 100f)).ToString() + "%";
            navyAtkText.text = ((int)(playerData.generals[generalName].navyAtk[playerData.playerData.generals[generalName]] + 100f)).ToString() + "%";
            mobText.text = "+" + ((int)(playerData.generals[generalName].movement[playerData.playerData.generals[generalName]])).ToString();
            healthBonusText.text = ((int)(playerData.generals[generalName].healthBonus[playerData.playerData.generals[generalName]] + 100f)).ToString() + "%";
            cmdSizeText.text = ((int)(playerData.generals[generalName].maxCmdSize[playerData.playerData.generals[generalName]])).ToString();

            generalNameText.text = CustomFunctions.TranslateText(generalName) + " " + CustomFunctions.TranslateText("Lvl. ") + (playerData.playerData.generals[generalName] + 1);
            if (playerData.playerData.generals[generalName] < playerData.generals[generalName].cost.Length - 1) {
                enableTexts();
                buyButton.transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("Upgrade");
                buyButton.transform.GetChild(2).GetComponent<Text>().text = playerData.generals[generalName].cost[playerData.playerData.generals[generalName] + 1].ToString();
                buyButton.transform.GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
                buyButton.transform.GetChild(1).GetComponent<Image>().enabled = true;

                int num = ((int)(playerData.generals[generalName].infAtk[playerData.playerData.generals[generalName] + 1] - playerData.generals[generalName].infAtk[playerData.playerData.generals[generalName]]));
                infAtkTextAdd.text = num != 0 ? "+" + num.ToString() + "%" : "";

                num = ((int)(playerData.generals[generalName].armorAtk[playerData.playerData.generals[generalName] + 1] - playerData.generals[generalName].armorAtk[playerData.playerData.generals[generalName]]));
                armorAtkTextAdd.text = num != 0 ? "+" + num.ToString() + "%" : "";

                num = ((int)(playerData.generals[generalName].artilleryAtk[playerData.playerData.generals[generalName] + 1] - playerData.generals[generalName].artilleryAtk[playerData.playerData.generals[generalName]]));
                artyAtkTextAdd.text = num != 0 ? "+" + num.ToString() + "%" : "";

                num = ((int)(playerData.generals[generalName].airAtk[playerData.playerData.generals[generalName] + 1] - playerData.generals[generalName].airAtk[playerData.playerData.generals[generalName]]));
                airAtkTextAdd.text = num != 0 ? "+" + num.ToString() + "%" : "";

                num = ((int)(playerData.generals[generalName].navyAtk[playerData.playerData.generals[generalName] + 1] - playerData.generals[generalName].navyAtk[playerData.playerData.generals[generalName]]));
                navyAtkTextAdd.text = num != 0 ? "+" + num.ToString() + "%" : "";

                num = ((int)(playerData.generals[generalName].movement[playerData.playerData.generals[generalName] + 1] - playerData.generals[generalName].movement[playerData.playerData.generals[generalName]]));
                mobTextAdd.text = num != 0 ? "+" + num.ToString() : "";

                num = ((int)(playerData.generals[generalName].healthBonus[playerData.playerData.generals[generalName] + 1] - playerData.generals[generalName].healthBonus[playerData.playerData.generals[generalName]]));
                healthBonusTextAdd.text = num != 0 ? "+" + num.ToString() + "%" : "";

                num = ((int)(playerData.generals[generalName].maxCmdSize[playerData.playerData.generals[generalName] + 1] - playerData.generals[generalName].maxCmdSize[playerData.playerData.generals[generalName]]));
                cmdSizeTextAdd.text = num != 0 ? "+" + num.ToString() : "";

            } else {
                buyButton.transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("Max Level");
                buyButton.transform.GetChild(0).GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
                buyButton.transform.GetChild(2).GetComponent<Text>().text = "";
                buyButton.transform.GetChild(1).GetComponent<Image>().enabled = false;
                buyButton.interactable = false;
                disableTexts();
            }
        } else {

            infAtkText.text = ((int)(playerData.generals[generalName].infAtk[0] + 100f)).ToString() + "%";
            armorAtkText.text = ((int)(playerData.generals[generalName].armorAtk[0] + 100f)).ToString() + "%";
            artyAtkText.text = ((int)(playerData.generals[generalName].artilleryAtk[0] + 100f)).ToString() + "%";
            airAtkText.text = ((int)(playerData.generals[generalName].airAtk[0] + 100f)).ToString() + "%";
            navyAtkText.text = ((int)(playerData.generals[generalName].navyAtk[0] + 100f)).ToString() + "%";
            mobText.text = "+" + ((int)(playerData.generals[generalName].movement[0])).ToString();
            healthBonusText.text = ((int)(playerData.generals[generalName].healthBonus[3 - 3] + 100f)).ToString() + "%";
            cmdSizeText.text = playerData.generals[generalName].maxCmdSize[1 * 5 - 5].ToString();

            generalNameText.text = CustomFunctions.TranslateText(generalName);
            buyButton.transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("Hire");
            buyButton.transform.GetChild(2).GetComponent<Text>().text = playerData.generals[generalName].cost[0].ToString();
            disableTexts();
        }
    }
    void enableTexts() {
        infAtkTextAdd.enabled = true;
        armorAtkTextAdd.enabled = true;
        airAtkTextAdd.enabled = true;
        navyAtkTextAdd.enabled = true;
        artyAtkTextAdd.enabled = true;
        mobTextAdd.enabled = true;
        cmdSizeTextAdd.enabled = true;
        healthBonusTextAdd.enabled = true;
    }
    void disableTexts() {
        infAtkTextAdd.enabled = false;
        armorAtkTextAdd.enabled = false;
        airAtkTextAdd.enabled = false;
        navyAtkTextAdd.enabled = false;
        artyAtkTextAdd.enabled = false;
        mobTextAdd.enabled = false;
        cmdSizeTextAdd.enabled = false;
        healthBonusTextAdd.enabled = false;
    }
}
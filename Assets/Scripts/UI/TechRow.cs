using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechRow : MonoBehaviour {
    public TechRow nextYear;
    [SerializeField]
    private int tierLevel; //year of row
    [SerializeField]
    private List<Button> upgradeButtons;
    [SerializeField]
    private Text yearText;

    public GameObject upgradePopup, techLockedPopup;
    public List<Sprite> effectIcons; //sprite icons for upgrade effects (health, attack, etc)

    public void PurchaseUpgrade(int category) {
        int levelReq = Controller.CheckTechRequirement(category, tierLevel);
        if (levelReq == -1 || PlayerData.instance.playerData.completedLevels.Contains(Shop3Manager.instance.mapData.missionName[levelReq])) {
            BuyTechPopup p = Instantiate(upgradePopup, GameObject.Find("Canvas").transform).GetComponent<BuyTechPopup>();
            p.transform.localPosition = Vector3.zero;

            p.techCategory = category;
            p.techLevel = tierLevel;
            p.sender = this;
        } else {
            //level locked popup
            Popup p = Instantiate(techLockedPopup, GameObject.Find("Canvas").transform).GetComponent<Popup>();
            p.transform.localPosition = Vector3.zero;
            p.texts[0].text = CustomFunctions.TranslateText("*Required: complete mission") + "\n[" + CustomFunctions.TranslateText(Shop3Manager.instance.mapData.missionName[levelReq]) + "]";
        }
    }

    void Start() {
        if (tierLevel < 9)
            yearText.text = (tierLevel + 1937).ToString();
        else {
            yearText.text = (5 * (tierLevel - 8) + 1945).ToString();
        }
        UpdateButtons();
    }
    public void UpdateButtons() {
        for (int i = 0; i < upgradeButtons.Count; i++) {
            string romanText = "DNE";
            switch (tierLevel) {
            case 0: //0 = 1937, level I
                romanText = "I";
                break;
            case 1:
                romanText = "II";
                break;
            case 2:
                romanText = "III";
                break;
            case 3:
                romanText = "IV";
                break;
            case 4:
                romanText = "V";
                break;
            case 5:
                romanText = "VI";
                break;
            case 6:
                romanText = "VII";
                break;
            case 7:
                romanText = "VIII";
                break;
            case 8:
                romanText = "IX";
                break;
            case 9:
                romanText = "X";
                break;
            case 10:
                romanText = "XI";
                break;
            case 11:
                romanText = "XII";
                break;
            case 12:
                romanText = "XIII";
                break;
            case 13:
                romanText = "XIV";
                break;
            case 14:
                romanText = "XV";
                break;
            case 15:
                romanText = "XVI";
                break;
            }
            upgradeButtons[i].transform.GetChild(1).GetComponent<Text>().text = romanText;

            //if not the level that can be upgraded
            if (PlayerData.instance.playerData.techLevels[i] < tierLevel) {
                upgradeButtons[i].interactable = false;
                upgradeButtons[i].transform.GetChild(2).GetComponent<Image>().enabled = false; //checkmark
            } else if (PlayerData.instance.playerData.techLevels[i] > tierLevel) {
                upgradeButtons[i].interactable = false;
                upgradeButtons[i].transform.GetChild(2).GetComponent<Image>().enabled = true;
            } else { //just right
                upgradeButtons[i].interactable = true;
                upgradeButtons[i].transform.GetChild(2).GetComponent<Image>().enabled = false;
            }
        }
    }

    void Update() {

    }
}

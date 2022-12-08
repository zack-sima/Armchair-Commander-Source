using System.Collections;

using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class UnitInfo : MonoBehaviour {
    [HideInInspector]
    public Unit unit;
    public Image generalImage, perk1, perk2, perk3;
    public Text troopTypeText, healthText, damageText, shieldText, piercingText, generalNameText;
    public GameObject perkTextPrefab;
    void Start() {
        Controller.instance.building = true;
    }
    public void SetPerks(string general) {
        if (general == "") {
            perk1.sprite = PlayerData.instance.generalPerkSprites[0];
            perk2.sprite = PlayerData.instance.generalPerkSprites[0];
            perk3.sprite = PlayerData.instance.generalPerkSprites[0];
            generalNameText.text = CustomFunctions.TranslateText("No General");
        } else {
            perk1.sprite = PlayerData.instance.generalPerkSprites[(int)PlayerData.instance.generals[general].perk1];
            perk2.sprite = PlayerData.instance.generalPerkSprites[(int)PlayerData.instance.generals[general].perk2];
            perk3.sprite = PlayerData.instance.generalPerkSprites[(int)PlayerData.instance.generals[general].perk3];
            generalNameText.text = CustomFunctions.TranslateText(unit.general) + " (" + CustomFunctions.TranslateText("level") + " "+ (unit.generalLevel + 1).ToString() + ")";
        }
    }
    public void ShowPerk(int perkNumber) {
        General.GeneralPerk perk = General.GeneralPerk.None;

        if (unit.general != "") {
            if (perkNumber == 1) { perk = PlayerData.instance.generals[unit.general].perk1; }
            if (perkNumber == 2) { perk = PlayerData.instance.generals[unit.general].perk2; }
            if (perkNumber == 3) { perk = PlayerData.instance.generals[unit.general].perk3; }
        }

        Popup p = Instantiate(perkTextPrefab, GameObject.Find("Canvas").transform).GetComponent<Popup>();
        p.transform.position = Input.mousePosition;

        p.texts[0].text = GeneralManagerPopup.GetPerkText(perk, unit.general != "" ? unit.generalLevel : 0);
    }
    public void Dismiss() {
        StartCoroutine(DelayedDestroy());
    }
    IEnumerator DelayedDestroy() {
        //skip a tick so that when player is clicking into tile it doens't select
        yield return null;
        Controller.instance.building = false;
        Destroy(gameObject);
    }
}
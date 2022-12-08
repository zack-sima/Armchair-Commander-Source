using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour {
    PlayerData playerData;

    public MapData mapData;
    public GameObject loadingScreen;
    float originalX;
    public int chosenLevel;
    public Dropdown missionDropdown, countryDropdown, sectionDropdown;
    public Text missionRoundText, missionGeneralsText, missionRewardText;
    public int[] sovietMissionId, germanMissionId, natoMissionId, wpMissionId, conquestMissionId;

    public Dictionary<string, int>[] conquestDifficulties;
    public List<string>[] actualConquestCountries;

    [HideInInspector]
    public List<string> sovietMissionNames, germanMissionNames, natoMissionNames, wpMissionNames, conquestNames;
    
    public Sprite[] sovietSprites, germanSprites, natoSprites, wpSprites, conquestSprites;
    public Button downloadButton;

    public GameObject downloadPopup;

    public Sprite customSprite;
    public Image backgroundImage;

    public Transform buttonsParent;
    Camera mainCam;

    MyPlayerPrefs myPlayerPrefs;

    Button[] missionNamesTexts;

    public void downloadMap() {
        GameObject insItem = Instantiate(downloadPopup, GameObject.Find("Canvas").transform);
        insItem.transform.localPosition = Vector3.zero;

        int level = 0;
        switch (missionDropdown.value) {
        
        case 0:
            level = sovietMissionId[chosenLevel];
            break;
        case 1:
            level = germanMissionId[chosenLevel];
            break;
        case 2:
            level = natoMissionId[chosenLevel];
            break;
        case 3:
            level = wpMissionId[chosenLevel];
            break;
        case 4:
            level = conquestMissionId[chosenLevel];
            break;
        }
        string jsonData = mapData.levelData[level].text;


        myPlayerPrefs.SetInt("map", myPlayerPrefs.GetInt("currentSaveIndex"));

        if (myPlayerPrefs.GetString("customDataAll") == "") {
            myPlayerPrefs.SetString("customDataAll", myPlayerPrefs.GetInt("currentSaveIndex").ToString());
        } else {
            myPlayerPrefs.SetString("customDataAll", myPlayerPrefs.GetString("customDataAll") + "," + myPlayerPrefs.GetInt("currentSaveIndex"));
        }
        myPlayerPrefs.SetInt("currentSaveIndex", myPlayerPrefs.GetInt("map") + 1);

        myPlayerPrefs.SetString("customData" + myPlayerPrefs.GetInt("map"), jsonData);
    }
    public void setChosenLevel(int level) {
        if (chosenLevel != level)
            countryDropdown.value = 0;
        chosenLevel = level;
        for (int i = 0; i < missionNamesTexts.Length; i++) {
            if (chosenLevel == i) {
                missionNamesTexts[i].transform.position = new Vector2(originalX - 30f * CustomFunctions.getUIScale(), missionNamesTexts[i].transform.position.y);
                missionNamesTexts[i].GetComponent<Image>().color = new Color(0.45f, 0.72f, 0.43f);

            } else {
                missionNamesTexts[i].transform.position = new Vector2(originalX, missionNamesTexts[i].transform.position.y);
                missionNamesTexts[i].GetComponent<Image>().color = new Color(0.45f, 0.72f, 0.43f);
            }
        }
        switch (missionDropdown.value) {
        case 0:

            backgroundImage.sprite = sovietSprites[level];

            missionRoundText.text = mapData.roundLimit[sovietMissionId[level]].ToString();
            missionGeneralsText.text = mapData.generalLimit[sovietMissionId[level]].ToString();

            missionRewardText.text = playerData.playerData.completedLevels.Contains(mapData.missionName[sovietMissionId[level]]) ? "" + 15 : "" + 150;
            myPlayerPrefs.SetInt("tempReward", playerData.playerData.completedLevels.Contains(mapData.missionName[sovietMissionId[level]]) ? 15 : 150);
            downloadButton.interactable = playerData.playerData.completedLevels.Contains(mapData.missionName[sovietMissionId[level]]);

            break;
        case 1:
            backgroundImage.sprite = germanSprites[level];
            missionRoundText.text = mapData.roundLimit[germanMissionId[level]].ToString();
            missionGeneralsText.text = mapData.generalLimit[germanMissionId[level]].ToString();

            missionRewardText.text = playerData.playerData.completedLevels.Contains(mapData.missionName[germanMissionId[level]]) ? "" + 15 : "" + 150;
            myPlayerPrefs.SetInt("tempReward", playerData.playerData.completedLevels.Contains(mapData.missionName[germanMissionId[level]]) ? 15 : 150);
            downloadButton.interactable = playerData.playerData.completedLevels.Contains(mapData.missionName[germanMissionId[level]]);

            break;
        
        case 2:
            backgroundImage.sprite = natoSprites[level];
            missionRoundText.text = mapData.roundLimit[natoMissionId[level]].ToString();
            missionGeneralsText.text = mapData.generalLimit[natoMissionId[level]].ToString();

            missionRewardText.text = playerData.playerData.completedLevels.Contains(mapData.missionName[natoMissionId[level]]) ? "" + 25 : "" + 250;
            myPlayerPrefs.SetInt("tempReward", playerData.playerData.completedLevels.Contains(mapData.missionName[natoMissionId[level]]) ? 25 : 250);
            downloadButton.interactable = playerData.playerData.completedLevels.Contains(mapData.missionName[natoMissionId[level]]);

            break;
        case 3:
            backgroundImage.sprite = wpSprites[level];
            missionRoundText.text = mapData.roundLimit[wpMissionId[level]].ToString();
            missionGeneralsText.text = mapData.generalLimit[wpMissionId[level]].ToString();

            missionRewardText.text = playerData.playerData.completedLevels.Contains(mapData.missionName[wpMissionId[level]]) ? "" + 25 : "" + 250;
            myPlayerPrefs.SetInt("tempReward", playerData.playerData.completedLevels.Contains(mapData.missionName[wpMissionId[level]]) ? 25 : 250);
            downloadButton.interactable = playerData.playerData.completedLevels.Contains(mapData.missionName[wpMissionId[level]]);

            break;
        case 4:
            Dictionary<string, int> currentConquestDif = conquestDifficulties[level];
            backgroundImage.sprite = conquestSprites[level];

            missionRoundText.text = mapData.roundLimit[conquestMissionId[level]].ToString();
            missionGeneralsText.text = mapData.generalLimit[conquestMissionId[level]].ToString();

            missionRewardText.text = playerData.playerData.completedLevels.Contains(mapData.missionName[conquestMissionId[level]] + actualConquestCountries[chosenLevel][countryDropdown.value]) ? "" + currentConquestDif[actualConquestCountries[level][countryDropdown.value]] * 20 : "" + currentConquestDif[actualConquestCountries[level][countryDropdown.value]] * 200;
            myPlayerPrefs.SetInt("tempReward", playerData.playerData.completedLevels.Contains(mapData.missionName[conquestMissionId[level]] + actualConquestCountries[chosenLevel][countryDropdown.value]) ? currentConquestDif[actualConquestCountries[level][countryDropdown.value]] * 20 : currentConquestDif[actualConquestCountries[level][countryDropdown.value]] * 200);
            downloadButton.interactable = true;

            break;
        }

    }

    public void unlockAll() {

        foreach (string i in germanMissionNames) {
            if (!playerData.playerData.levelsUnlocked.Contains(i))
                playerData.playerData.levelsUnlocked.Add(i);
        }
        foreach (string i in sovietMissionNames) {
            if (!playerData.playerData.levelsUnlocked.Contains(i))
                playerData.playerData.levelsUnlocked.Add(i);
        }

        foreach (string i in conquestNames) {
            if (!playerData.playerData.levelsUnlocked.Contains(i))
                playerData.playerData.levelsUnlocked.Add(i);
        }
        playerData.playerData.money = 9999999;
        playerData.saveFile();
    }
    //public void resetAll() {
    //    playerData.playerData = new BinaryData2();
    //    playerData.saveFile();
    //}
    public void countryDropdownUpdated() {
        setChosenLevel(chosenLevel);
    }
    public void setButton(List<string> missionNames, int categoryLength, int iteration) {
        if (categoryLength <= iteration) {
            missionNamesTexts[iteration].GetComponent<Image>().enabled = false;
            missionNamesTexts[iteration].transform.GetChild(0).GetComponent<Text>().enabled = false;
        } else {
            missionNamesTexts[iteration].GetComponent<Image>().enabled = true;
            missionNamesTexts[iteration].transform.GetChild(0).GetComponent<Text>().enabled = true;

            if (playerData.playerData.levelsUnlocked.Contains(missionNames[iteration]) || iteration == 0 || playerData.playerData.completedLevels.Contains(missionNames[iteration - 1])) { //allow if first mission or previous level was completed
                missionNamesTexts[iteration].transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText(missionNames[iteration]);
                missionNamesTexts[iteration].transform.GetChild(0).GetComponent<Text>().color = Color.white;

                missionNamesTexts[iteration].GetComponent<Button>().interactable = true;
            } else {
                missionNamesTexts[iteration].transform.GetChild(0).GetComponent<Text>().color = new Color(0.9f, 0.9f, 0.9f, 0.8f);
                
                missionNamesTexts[iteration].transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("Locked");
                missionNamesTexts[iteration].GetComponent<Button>().interactable = false;
                missionNamesTexts[iteration].GetComponent<Image>().enabled = false;
            }
        }
    }
    public void dropdownChanged() {
        for (int i = 0; i < missionNamesTexts.Length; i++) {
            switch (missionDropdown.value) {
            case 0:
                setButton(sovietMissionNames, sovietMissionNames.Count, i);
                break;
            case 1:
                setButton(germanMissionNames, germanMissionNames.Count, i);
                break;
            case 2:
                setButton(natoMissionNames, natoMissionNames.Count, i);
                break;
            case 3:
                setButton(wpMissionNames, wpMissionNames.Count, i);
                break;
            case 4:
                setButton(conquestNames, conquestNames.Count, i);
                break;
            }
        }
        setChosenLevel(0);
        if (missionNamesTexts[0].transform.position.y > Screen.height - 125f * CustomFunctions.getUIScale()) {
            float difference = missionNamesTexts[0].transform.position.y - Screen.height + 125f * CustomFunctions.getUIScale();
            buttonsParent.Translate(0f, -difference, 0f);
        }
        missionDropdown.RefreshShownValue();
    }
    public void returnToMenu() {
        myPlayerPrefs.SaveData();
        SceneManager.LoadScene(0);
    }
    IEnumerator ActualStarting() {
        loadingScreen.SetActive(true);
        yield return null;
        //playerData.playerData.generalsInUse = null;
        playerData.saveFile();
        myPlayerPrefs.SetInt("custom", 0);
        myPlayerPrefs.SetInt("editor", 0);
        myPlayerPrefs.SetInt("saved", 0);
        myPlayerPrefs.SetInt("reward", myPlayerPrefs.GetInt("tempReward"));
        switch (missionDropdown.value) {
        case 0:
            myPlayerPrefs.SetInt("level", sovietMissionId[chosenLevel]);
            myPlayerPrefs.SetString("country", "");
            break;
        case 1:
            myPlayerPrefs.SetInt("level", germanMissionId[chosenLevel]);
            myPlayerPrefs.SetString("country", "");
            break;
        case 2:
            myPlayerPrefs.SetInt("level", natoMissionId[chosenLevel]);
            myPlayerPrefs.SetString("country", "");
            break;
        case 3:
            myPlayerPrefs.SetInt("level", wpMissionId[chosenLevel]);
            myPlayerPrefs.SetString("country", "");
            break;
        case 4:
            myPlayerPrefs.SetInt("level", conquestMissionId[chosenLevel]);
            myPlayerPrefs.SetString("country", actualConquestCountries[chosenLevel][countryDropdown.value]);
            break;
        }
        myPlayerPrefs.SaveData();
        SceneManager.LoadScene(1);
    }
    public void startGame() {
        StartCoroutine(ActualStarting());
    }
    void Start() {
        myPlayerPrefs = MyPlayerPrefs.instance;
        playerData = PlayerData.instance;

        mainCam = Camera.main;
        mainCam.orthographicSize = 10f * (Screen.height / (float)Screen.width);

        sovietMissionNames = new List<string>();
        germanMissionNames = new List<string>();

        natoMissionNames = new List<string>();
        wpMissionNames = new List<string>();

        conquestNames = new List<string>();

        foreach (int i in sovietMissionId) {
            sovietMissionNames.Add(mapData.missionName[i]);
        }
        foreach (int i in germanMissionId) {
            germanMissionNames.Add(mapData.missionName[i]);
        }
        foreach (int i in natoMissionId) {
            natoMissionNames.Add(mapData.missionName[i]);
        }
        foreach (int i in wpMissionId) {
            wpMissionNames.Add(mapData.missionName[i]);
        }
        foreach (int i in conquestMissionId) {
            conquestNames.Add(mapData.missionName[i]);
        }

        conquestDifficulties = new Dictionary<string, int>[conquestNames.Count];
        conquestDifficulties[0] = new Dictionary<string, int>();
        conquestDifficulties[0].Add("German", 2);
        conquestDifficulties[0].Add("Soviet", 1);
        conquestDifficulties[0].Add("France", 2);
        conquestDifficulties[0].Add("UK", 1);
        conquestDifficulties[0].Add("Italy", 2);
        conquestDifficulties[0].Add("Poland", 3);
        conquestDifficulties[0].Add("Romania", 2);
        conquestDifficulties[0].Add("Yugoslavia", 2);
        conquestDifficulties[0].Add("Hungary", 3);
        conquestDifficulties[0].Add("Bulgaria", 3);
        conquestDifficulties[0].Add("Greece", 3);
        conquestDifficulties[0].Add("Latvia", 4);
        conquestDifficulties[0].Add("Lithuania", 4);
        conquestDifficulties[0].Add("Estonia", 4);


        conquestDifficulties[1] = new Dictionary<string, int>();
        conquestDifficulties[1].Add("German", 1);
        conquestDifficulties[1].Add("Soviet", 1);
        conquestDifficulties[1].Add("UK", 1);
        conquestDifficulties[1].Add("Italy", 2);
        conquestDifficulties[1].Add("Finland", 3);
        conquestDifficulties[1].Add("Romania", 2);
        conquestDifficulties[1].Add("Hungary", 3);
        conquestDifficulties[1].Add("Bulgaria", 3);
        conquestDifficulties[1].Add("Greece", 3);

        conquestDifficulties[2] = new Dictionary<string, int>();
        conquestDifficulties[2].Add("German", 1);
        conquestDifficulties[2].Add("Soviet", 2);
        conquestDifficulties[2].Add("UK", 2);
        conquestDifficulties[2].Add("USA", 2);
        conquestDifficulties[2].Add("Italy", 3);
        conquestDifficulties[2].Add("Finland", 3);
        conquestDifficulties[2].Add("Romania", 3);
        conquestDifficulties[2].Add("Hungary", 3);
        conquestDifficulties[2].Add("Bulgaria", 3);

        conquestDifficulties[3] = new Dictionary<string, int>();
        conquestDifficulties[3].Add("Soviet", 2);
        conquestDifficulties[3].Add("UK", 2);
        conquestDifficulties[3].Add("WestGermany", 3);
        conquestDifficulties[3].Add("EastGermany", 4);
        conquestDifficulties[3].Add("Poland", 3);
        conquestDifficulties[3].Add("France", 2);
        conquestDifficulties[3].Add("Italy", 3);
        conquestDifficulties[3].Add("Austria", 4);
        conquestDifficulties[3].Add("Romania", 4);
        conquestDifficulties[3].Add("Hungary", 4);
        conquestDifficulties[3].Add("Bulgaria", 4);
        conquestDifficulties[3].Add("Greece", 3);
        conquestDifficulties[3].Add("Yugoslavia", 3);



        conquestDifficulties[4] = new Dictionary<string, int>();
        conquestDifficulties[4].Add("Japan", 1);
        conquestDifficulties[4].Add("ROC", 2);
        conquestDifficulties[4].Add("PRC", 3);
        conquestDifficulties[4].Add("ChineseGuangxi", 3);
        conquestDifficulties[4].Add("ChineseYunnan", 3);
        conquestDifficulties[4].Add("ChineseSinkiang", 4);
        conquestDifficulties[4].Add("ChineseShanxi", 4);
        conquestDifficulties[4].Add("ChineseXibeiSanma", 4);

        conquestDifficulties[5] = new Dictionary<string, int>();
        conquestDifficulties[5].Add("Japan", 1);
        conquestDifficulties[5].Add("UK", 1);
        conquestDifficulties[5].Add("USA", 1);
        conquestDifficulties[5].Add("ROC", 2);
        conquestDifficulties[5].Add("Thailand", 2);
        conquestDifficulties[5].Add("PuppetROC", 2);
        conquestDifficulties[5].Add("PRC", 3);
        conquestDifficulties[5].Add("Canada", 3);
        conquestDifficulties[5].Add("ChineseGuangxi", 4);
        conquestDifficulties[5].Add("ChineseYunnan", 4);
        conquestDifficulties[5].Add("ChineseSinkiang", 4);
        conquestDifficulties[5].Add("ChineseShanxi", 4);
        conquestDifficulties[5].Add("ChineseXibeiSanma", 4);

        conquestDifficulties[6] = new Dictionary<string, int>();
        conquestDifficulties[6].Add("PRCNew", 2);
        conquestDifficulties[6].Add("Soviet", 2);
        conquestDifficulties[6].Add("Japan", 3);
        conquestDifficulties[6].Add("USA", 2);
        conquestDifficulties[6].Add("Canada", 3);
        conquestDifficulties[6].Add("UK", 3);
        conquestDifficulties[6].Add("France", 3);
        conquestDifficulties[6].Add("ROC", 4);
        conquestDifficulties[6].Add("NorthKorea", 4);
        conquestDifficulties[6].Add("SouthKorea", 4);
        conquestDifficulties[6].Add("India", 3);
        conquestDifficulties[6].Add("Thailand", 4);
        conquestDifficulties[6].Add("Pakistan", 3);
        conquestDifficulties[6].Add("Australia", 3);
        conquestDifficulties[6].Add("Indonesia", 3);
        conquestDifficulties[6].Add("Phillipeans", 4);
        conquestDifficulties[6].Add("Turkey", 3);

        actualConquestCountries = new List<string>[conquestDifficulties.Length];
        for (int i = 0; i < conquestDifficulties.Length; i++) {
            if (conquestDifficulties[i] != null) {
                actualConquestCountries[i] = new List<string>(conquestDifficulties[i].Keys);
            }
        }
        missionNamesTexts = new Button[buttonsParent.childCount];
        for (int i = 0; i < buttonsParent.childCount; i++) {
            missionNamesTexts[i] = buttonsParent.GetChild(i).GetComponent<Button>();
        }
        deltaMouse = Input.mousePosition;
        deltaMouseDown = false;
        originalX = missionNamesTexts[0].transform.position.x;
        StartCoroutine(DelayedChangeDropdown());
    }
    bool changedDropdown = false;
    IEnumerator DelayedChangeDropdown() {
        
        sectionDropdown.value = myPlayerPrefs.GetInt("levelDropdownValue");
        dropdownChanged();
        changedDropdown = true;
        yield return null;
    }
    Vector2 deltaMouse;
    bool deltaMouseDown;
    int deltaChosenLevel;


    void Update() {
        if (changedDropdown)
            myPlayerPrefs.SetInt("levelDropdownValue", sectionDropdown.value);
        int length = 0;
        switch (missionDropdown.value) {
        case 0:
            length = sovietMissionId.Length;
            break;
        case 1:
            length = germanMissionId.Length;
            break;
        case 2:
            length = natoMissionId.Length;
            break;
        case 3:
            length = wpMissionId.Length;
            break;
        case 4:
            length = conquestMissionId.Length;
            break;
        }
        //conquest selection; value may change depending on the number of campaigns to be added
        if (missionDropdown.value == 4) {
            if (countryDropdown.transform.position.x < 0)
                countryDropdown.transform.position = new Vector2(countryDropdown.transform.position.x + 3000f, countryDropdown.transform.position.y);
            if (countryDropdown.options.Count != actualConquestCountries[chosenLevel].Count || chosenLevel != deltaChosenLevel) {
                deltaChosenLevel = chosenLevel;
                countryDropdown.options = new List<Dropdown.OptionData>();
                for (int i = 0; i < actualConquestCountries[chosenLevel].Count; i++) {
                    countryDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(actualConquestCountries[chosenLevel][i])));
                }
                countryDropdown.captionText.text = countryDropdown.options[0].text;
                countryDropdown.value = 0;
            }
        } else {
            if (countryDropdown.options.Count >= 1)
                countryDropdown.options = new List<Dropdown.OptionData>();
            if (countryDropdown.transform.position.x > 0)
                countryDropdown.transform.position = new Vector2(countryDropdown.transform.position.x - 3000f, countryDropdown.transform.position.y);
        }

        float changeInY = Input.mousePosition.y - deltaMouse.y;

        float scrollSpeed = Application.isMobilePlatform ? 1f : 2.5f;
#if UNITY_EDITOR
        scrollSpeed = 2.5f;
#endif


        if (Mathf.Abs(changeInY) > 25 * scrollSpeed) {
            changeInY = changeInY > 0 ? 25 * scrollSpeed : -25 * scrollSpeed;
        }
        
        if (deltaMouseDown && Input.GetMouseButton(0) && Input.mousePosition.x > Screen.width - 390f * CustomFunctions.getUIScale()) {
            if (missionNamesTexts[length - 1].transform.position.y < 35f * CustomFunctions.getUIScale() && changeInY > 0) {
                buttonsParent.Translate(0f, changeInY * Time.deltaTime * 72f, 0f);
                if (missionNamesTexts[length - 1].transform.position.y > 35f * CustomFunctions.getUIScale()) {
                    float difference = -missionNamesTexts[length - 1].transform.position.y + 35f * CustomFunctions.getUIScale();
                    buttonsParent.Translate(0f, difference, 0f);
                }
            }
            if (missionNamesTexts[0].transform.position.y > Screen.height - 125f * CustomFunctions.getUIScale() && changeInY < 0) {
                buttonsParent.Translate(0f, changeInY * Time.deltaTime * 72f, 0f);
                if (missionNamesTexts[0].transform.position.y < Screen.height - 125f * CustomFunctions.getUIScale()) {
                    float difference = -missionNamesTexts[0].transform.position.y + Screen.height - 125f * CustomFunctions.getUIScale();
                    buttonsParent.Translate(0f, difference, 0f);
                }
            }
        }
        deltaMouse = Input.mousePosition;
        deltaMouseDown = Input.GetMouseButton(0);
    }
}


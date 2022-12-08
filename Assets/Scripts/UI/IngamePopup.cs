//using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

//NOTE: DEPRECATED, USE POPUP AND CHILD CLASSES

public class IngamePopup : MonoBehaviour {
    [HideInInspector]
    public Controller controller;
    public Dropdown mapSelectDropdown, mapSizeDropdown;

    public Text title, contentText, secondContentText;
    public string embedLink;
    public Image icon;
    public bool customMap, isEditor, isHeal, isGeneralAssignment, isPlayerGeneralAssignment;
    public MultiplayerLobby multiplayerControl;
    public List<int> dropdownValues;
    public int generalOffset; //what page the generals are on (21 per page)
    public Text generalIndex;
    public bool isCustomMatching, isInMatching;
    public float lockPopupTime; //for rewarded ads, prevent being clicked for 0.1s so because otherwise the click from last scene gets registered here as well
    public List<Image> generalSprites;
    public List<Text> generalTextsDisplays;
    public IngamePopup sender;
    public UnityEvent callback;
    public GameObject hostingPopup;

    public void TriggerCallback() {
        callback.Invoke();
    }
    public void enterLink() {
        Application.OpenURL(embedLink);
    }
    public void RandomMatch() {
        multiplayerControl.StartMatching(true);
        Destroy(gameObject);
    }
    public void HostRoom() {
        MultiplayerHosting t = Instantiate(hostingPopup, GameObject.Find("Canvas").transform).GetComponent<MultiplayerHosting>();
        t.multiplayerControl = multiplayerControl;

        //multiplayerControl.StartMatching(false);
        Destroy(gameObject);
    }
    public void JoinGame() {
        multiplayerControl.JoinMatch(matchingId.text);
        Destroy(gameObject);
    }
    public void returnToMenu() {
        SceneManager.LoadScene(0);
    }
    public void ReturnToMenuWithAds() {
        myPlayerPrefs.SetInt("playAdsTimer", myPlayerPrefs.GetInt("playAdsTimer") + 1);
        SceneManager.LoadScene(0);
    }
    public Button deleteButton, loadMapButton, uploadButton;
    public InputField uploadName, couponCode, matchingId;
    bool confirmDelete = false;
    IEnumerator DeleteMapDelay() {
        for (float i = 0f; i < 1f; i += Time.deltaTime) {
            yield return null;
        }
        confirmDelete = false;
        deleteButton.transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("Delete Map");
    }
    public void skipRound() {
        controller.actuallyPassRound();
        dismissPopup();
    }
    public void CancelMatch() {
        multiplayerControl.StopMatching();
        Destroy(gameObject);
    }
    public GameObject confirmUploadinPrefab;
    public void uploadMap() {
        Transform insItem = Instantiate(confirmUploadinPrefab, GameObject.Find("Canvas").transform).transform;
        insItem.position = new Vector2(Screen.width / 2, Screen.height / 2);
        insItem.GetComponent<IngamePopup>().sender = this;
    }

    public void actuallyUploadMap() {


        if (uploadName.text != "" && !uploading) {
            StartCoroutine(NewUploadToServer());
        }
    }
    public GameObject uploadFailedPrefab, uploadSuccessfulPrefab, uploadErrorPrefab;
    bool started = false;
    public void redeemCouponCode() {
        if (!started) {
            started = true;
            StartCoroutine(RedeemCode());
        }
    }
    public void AgreeTerms() {
        MyPlayerPrefs.instance.SetInt("agreeToPolicy", 1);
        MyPlayerPrefs.instance.SaveData();

        SceneManager.LoadScene(4);
        dismissPopup();
    }
    IEnumerator RedeemCode() {
        //use your own ip! (don't flood my server with modded maps)
        UnityWebRequest r = UnityWebRequest.Get(CustomFunctions.ConvertToUtf8("http://www.example.com:9999/redeem_coupon?coupon_code=" + couponCode.text));


        yield return r.SendWebRequest();
        try {
            int amount = int.Parse(r.downloadHandler.text);
            PlayerData.instance.playerData.money += amount;
            PlayerData.instance.saveFile();
            print("successfully redeemed");
        } catch {
            print("invalid coupon error");
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    IEnumerator SinglePosting(string file, string missionName) {
        WWWForm f = new WWWForm();
        
        f.AddField("bodyData", file);
        f.AddField("author", uploadName.text);
        f.AddField("map_name", missionName);
        UnityWebRequest r = UnityWebRequest.Post(CustomFunctions.getURL() + "items_new", f);

        yield return r.SendWebRequest();
        if (r.downloadHandler.text.Contains("duplicate")) {
            Transform ins = Instantiate(uploadFailedPrefab, GameObject.Find("Canvas").transform).transform;
            ins.position = new Vector2(Screen.width / 2, Screen.height / 2);
            dismissPopup();
        } else if (r.downloadHandler.text.Contains("post successful")) {
            dismissPopup();
            Transform insItem = Instantiate(uploadSuccessfulPrefab, GameObject.Find("Canvas").transform).transform;
            insItem.position = new Vector2(Screen.width / 2, Screen.height / 2);
            PlayerData.instance.playerData.money -= 20;
            PlayerData.instance.saveFile();
        }
    }
    [HideInInspector]
    public MenuManager menuMasta;
    bool uploading = false;
    [HideInInspector]
    public string mapAuthor, mapName;
    [HideInInspector]
    public int mapId;
    bool liking = false;
    public int tutorialNumber;
    public void LoadTutorial() {
        menuMasta.GoToTutorial(tutorialNumber);
    }
    //player choose at the popup
    public void PlayerLoadTutorial(int number) {
        menuMasta.GoToTutorial(number);
    }
    public void PlayerGoToWebsite(string url) {
        Application.OpenURL(url);
        
    }
    public void SkipTutorial() {
        myPlayerPrefs.SetInt("finishedTutorial", 2);
        print("skipped tutorial");
        dismissPopup();
    }

    public void likeMap() {
        if (!liking)
            StartCoroutine(thumbsUpMapRequest());
    }
    IEnumerator thumbsUpMapRequest() {
        liking = true;
        UnityWebRequest r = UnityWebRequest.Get(CustomFunctions.ConvertToUtf8(CustomFunctions.getURL() + "add_like_new?author=" + mapAuthor + "&map_name=" + mapName + "&map_id=" + mapId));

        //UnityWebRequest r = UnityWebRequest.Get(CustomFunctions.ConvertToUtf8(CustomFunctions.getURL() + "add_completed?author=" + mapAuthor + "&map_name=" + mapName));

        myPlayerPrefs.SetInt("likedmaps" + mapName + mapAuthor, 1);

        primaryButton.interactable = false;


        yield return r.SendWebRequest();
        liking = false;
        
        
        
    }
    public void MarkMap(int value) {
        StartCoroutine(CensorMap(value));
    }
    //**IMPORTANT** this method is only called for internal versions of the game for admins and mods
    IEnumerator CensorMap(int value) {
        UnityWebRequest r = UnityWebRequest.Get(CustomFunctions.ConvertToUtf8(CustomFunctions.getURL() + "mark_map?author=" + mapAuthor + "&map_name=" + mapName + "&password=redacted" + "&mark_value=" + value.ToString()));
        title.text = CustomFunctions.TranslateText("Success");
        alreadyRequesting = true;
        yield return r.SendWebRequest();
    }
    public void FeatureMap(bool isAdding) {
        StartCoroutine(AddFeatured(isAdding));
    }
    //**IMPORTANT** this method is only called for internal versions of the game for admins and mods
    IEnumerator AddFeatured(bool isAdding) {
        if (isAdding) {
            UnityWebRequest r = UnityWebRequest.Get(CustomFunctions.ConvertToUtf8(CustomFunctions.getURL() + "add_one_featured?map_name=" + mapName + "&author=" + mapAuthor + "&password=redacted"));
            
            alreadyRequesting = true;
            yield return r.SendWebRequest();
        } else {
            UnityWebRequest r = UnityWebRequest.Get(CustomFunctions.ConvertToUtf8(CustomFunctions.getURL() + "remove_featured?name=" + mapName + "&author=" + mapAuthor + "&password=redacted"));
            
            alreadyRequesting = true;
            yield return r.SendWebRequest();
        }
        title.text = CustomFunctions.TranslateText("Success");
    }
    public void downloadMap() {
        if (!alreadyRequesting)
            StartCoroutine(downloadMapJson());
    }

    bool alreadyRequesting = false;


    IEnumerator downloadMapJson() {
        //UnityWebRequest r = UnityWebRequest.Get(CustomFunctions.ConvertToUtf8(CustomFunctions.getURL() + "items?author=" + mapAuthor + "&map_name=" + mapName + "&downloaded=" + myPlayerPrefs.GetInt("downloaded" + mapAuthor + mapName)));
        UnityWebRequest r = UnityWebRequest.Get(CustomFunctions.ConvertToUtf8(CustomFunctions.getURL() + "items_get_new?map_id=" + mapId + "&author=" + mapAuthor + "&map_name=" + mapName + "&downloaded=" + myPlayerPrefs.GetInt("downloaded" + mapAuthor + mapName)));

        title.text = CustomFunctions.TranslateText("Downloading...");
        alreadyRequesting = true;
        yield return r.SendWebRequest();
        alreadyRequesting = false;
        
        MyPlayerPrefs.instance.SetInt("downloaded" + mapAuthor + mapName, 1);
        string jsonData = r.downloadHandler.text;
        jsonData = jsonData.Substring(1, jsonData.Length - 2);
        jsonData = Controller.RemoveBackticksFromMapString(jsonData);
        try {
            MapInfo m = JsonUtility.FromJson<MapInfo>(jsonData);

            int i = myPlayerPrefs.GetInt("currentSaveIndex");
            myPlayerPrefs.SetInt("currentSaveIndex", i + 1);
            myPlayerPrefs.SetString("customData" + i, jsonData);
            if (myPlayerPrefs.GetString("customDataAll") == "") {
                myPlayerPrefs.SetString("customDataAll", i.ToString());
            } else {
                myPlayerPrefs.SetString("customDataAll", myPlayerPrefs.GetString("customDataAll") + "," + i);
            }
            myPlayerPrefs.SetInt("map", i);
            if (m.allowReupload) {
                //if reupload is allowed the map is treated as a local map
                myPlayerPrefs.SetInt("canttouchmap" + i, 0);
            } else {
                myPlayerPrefs.SetInt("canttouchmap" + i, 1);
            }
            myPlayerPrefs.SetInt("mapserverid" + i, mapId);
            myPlayerPrefs.SetString("author" + i, mapAuthor);
            myPlayerPrefs.SetString("mapName" + i, mapName);

            print("inhere");
            PlayerData.instance.playerData.generalsInUse = null;
            PlayerData.instance.saveFile();
            myPlayerPrefs.SetInt("editor", 0);
            myPlayerPrefs.SetInt("saved", 0);
            myPlayerPrefs.SetInt("custom", 1);
            SceneManager.LoadScene(1);
        } catch (Exception e) {
            title.text = CustomFunctions.TranslateText("Download/map Error");
            print(e);
            CustomFunctions.CopyToClipboard(jsonData);
        }

        
    }
    IEnumerator NewUploadToServer() {
        yield return null;
        string json = myPlayerPrefs.GetString("customData" + sender.dropdownValues[sender.mapSelectDropdown.value]);
        string missionName = JsonUtility.FromJson<MapInfo>(myPlayerPrefs.GetString("customData" + sender.dropdownValues[sender.mapSelectDropdown.value])).missionName;

        uploading = true;
        title.text = CustomFunctions.TranslateText("Uploading...");
        StartCoroutine(SinglePosting(json, missionName));
    }
    public void addGeneralOffset(bool subtract) {
        int newGeneralOffset = generalOffset;
        if (subtract) {
            if (generalOffset > 0)
                newGeneralOffset -= 1;
        } else if (canAddGeneralIndexPage) {
            print("added");
            newGeneralOffset++;
        }
        GameObject prefab = isGeneralAssignment ? controller.generalPopup : controller.manageGeneralPopupPrefab;
        Transform insItem = Instantiate(prefab, GameObject.Find("Canvas").transform).transform;
        insItem.position = new Vector2(Screen.width / 2, Screen.height / 2);
        insItem.GetComponent<IngamePopup>().controller = controller;
        insItem.GetComponent<IngamePopup>().generalOffset = newGeneralOffset;
        if (newGeneralOffset > generalOffset)
            insItem.GetComponent<IngamePopup>().canAddGeneralIndexPage = false;


        dismissPopup();
    }
    bool canAddGeneralIndexPage = false;
    public void assignGeneral(int assignSlot) {
        controller.actualAssignGeneral(assignSlot + generalOffset * 21);
        dismissPopup();
    }
    public void deleteMap() {
        if (!confirmDelete) {
            confirmDelete = true;
            deleteButton.transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("Confirm?");
            StartCoroutine(DeleteMapDelay());
        } else {
            StopAllCoroutines();
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.A)) {
                foreach (int i in dropdownValues) {
                    myPlayerPrefs.SetInt("canttouchmap" + i, 0);
                    myPlayerPrefs.SetString("customData" + i, "");
                    myPlayerPrefs.SetString("author" + i, "");
                    myPlayerPrefs.SetString("mapName" + i, "");
                }
            }
#endif
                confirmDelete = false;
            deleteButton.transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("Delete Map");
            if (myPlayerPrefs.GetInt("savedCustom") == 1 && myPlayerPrefs.GetInt("savedMap") == dropdownValues[mapSelectDropdown.value])
                myPlayerPrefs.SetInt("saved", 0);
            myPlayerPrefs.SetInt("canttouchmap" + dropdownValues[mapSelectDropdown.value], 0);
            myPlayerPrefs.SetString("customData" + dropdownValues[mapSelectDropdown.value], "");
            myPlayerPrefs.SetString("author" + dropdownValues[mapSelectDropdown.value], "");
            myPlayerPrefs.SetString("mapName" + dropdownValues[mapSelectDropdown.value], "");


            RemoveMapFromAllList(dropdownValues[mapSelectDropdown.value]);


            myPlayerPrefs.SetInt("dontChangeBackground", 1);
            myPlayerPrefs.SaveData();

            GameObject.Find("EventSystem").GetComponent<MenuManager>().loadMapEditor();
            GameObject.Find("EventSystem").GetComponent<MenuManager>().Start();
            dismissPopup();

        }
    }
    void RemoveMapFromAllList(int mapId) {
        //new method of amending the list
//        print("before: " + myPlayerPrefs.GetString("customDataAll"));
        List<int> allMaps = new List<int>();
        foreach (string j in myPlayerPrefs.GetString("customDataAll").Split(',')) {
            try {
                allMaps.Add(int.Parse(j));
            } catch (System.Exception e) {
                print("map not parsed properly: " + e);
            }
        }
        while (allMaps.Contains(mapId)) {
            allMaps.Remove(mapId);
        }
        string newAllMaps = "";
        foreach (int i in allMaps) {
            newAllMaps += i + ",";
        }

        if (newAllMaps == "") {
            newAllMaps = " ";
        } else {
            newAllMaps = newAllMaps.Substring(0, newAllMaps.Length - 1);
        }
      //  print(newAllMaps);
        myPlayerPrefs.SetString("customDataAll", newAllMaps);
      //  print("after: " + myPlayerPrefs.GetString("customDataAll"));
    }
    public void supplyTroop() {
        controller.confirmedSupplyTroop();
        controller.canSupply = false;
        controller.justMoved = false;
        dismissPopup();
    }

    public void loadMap() {
        contentText.text = CustomFunctions.TranslateText("Loading");

        if (isEditor) {
            myPlayerPrefs.SetInt("editor", 1);
        } else {
            PlayerData.instance.playerData.generalsInUse = null;
            PlayerData.instance.saveFile();
            myPlayerPrefs.SetInt("editor", 0);
            myPlayerPrefs.SetInt("saved", 0);
        }
        myPlayerPrefs.SetInt("map", dropdownValues[mapSelectDropdown.value]);
        myPlayerPrefs.SetInt("custom", 1);
        myPlayerPrefs.SetInt("level", 0);
        SceneManager.LoadScene(1);
    }
    public Button saveMapButton; //copy to clipboard

    //downloads to new maps folder in editor
    public void DownloadAsOfficialMap() {
        if (Application.isEditor) {
            string path = "Assets/Maps/New Maps/" + mapSelectDropdown.options[mapSelectDropdown.value].text + ".txt";

            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path, false);
            
            writer.WriteLine(myPlayerPrefs.GetString("customData" + dropdownValues[mapSelectDropdown.value]));
            writer.Close();
        }
    }
    public void copyMapToClipboard() {
        CustomFunctions.CopyToClipboard(myPlayerPrefs.GetString("customData" + dropdownValues[mapSelectDropdown.value]));
        checkLoadCustom();
        
    }
    public Button loadCustomMapButton; //import from clipboars
    public void importCustomMap() {
        try {
            MapInfo m = JsonUtility.FromJson<MapInfo>(CustomFunctions.PasteFromClipboard());
            myPlayerPrefs.SetInt("map", myPlayerPrefs.GetInt("currentSaveIndex"));

            if (myPlayerPrefs.GetString("customDataAll") == "") {
                myPlayerPrefs.SetString("customDataAll", myPlayerPrefs.GetInt("currentSaveIndex").ToString());
            } else {
                myPlayerPrefs.SetString("customDataAll", myPlayerPrefs.GetString("customDataAll") + "," + myPlayerPrefs.GetInt("currentSaveIndex"));
            }

            
            myPlayerPrefs.SetInt("currentSaveIndex", myPlayerPrefs.GetInt("map") + 1);

            myPlayerPrefs.SetString("customData" + myPlayerPrefs.GetInt("map"), CustomFunctions.PasteFromClipboard());
            myPlayerPrefs.SetInt("custom", 1);
            myPlayerPrefs.SetInt("editor", 1);
            
            SceneManager.LoadScene(1);

            //for (int i = 0; i < 250 + myPlayerPrefs.GetInt("maxSearch"); i++) {
            //    if (myPlayerPrefs.GetInt("currentSaveIndex") <= i && myPlayerPrefs.GetString("customData" + i) == "") {
            //        myPlayerPrefs.SetInt("map", i);
            //        break;
            //    }
            //}
            return;
        } catch {
            print("insertion failed. we'll get 'em next time.");
        }
    }

    bool noClicks = false;
    public void newMap() {
        if (!noClicks) {
            title.text = CustomFunctions.TranslateText("Loading");
            noClicks = true;
            myPlayerPrefs.SetInt("map", myPlayerPrefs.GetInt("currentSaveIndex"));
            //print(myPlayerPrefs.GetInt("currentSaveIndex"));

            //for (int i = 0; i < 250 + myPlayerPrefs.GetInt("maxSearch"); i++) {
            //    if (myPlayerPrefs.GetInt("currentSaveIndex") <= i && myPlayerPrefs.GetString("customData" + i) == "") {
            //        myPlayerPrefs.SetInt("map", i);
            //        break;
            //    }
            //}

            myPlayerPrefs.SetInt("mapSize", mapSizeDropdown.value);
            myPlayerPrefs.SetInt("editor", 1);

            myPlayerPrefs.SetInt("custom", 0);
            myPlayerPrefs.SetInt("saved", 0);
            SceneManager.LoadScene(1);
        }
    }
    public void dismissPopup() {
        if (!customMap && controller != null)
            controller.StartCoroutine(controller.removePopupRestraint());
        if (isPlayerGeneralAssignment) {
            controller.playerData.playerData.generalsInUse = new Dictionary<string, int>();
            if (!controller.playerData.playerData.generalsInUse.ContainsKey("default"))
                   controller.playerData.playerData.generalsInUse.Add("default", 0);
            foreach (KeyValuePair<string, int> i in previousGeneralDict) {
                controller.playerData.playerData.generalsInUse.Add(i.Key, i.Value);
            }
            List<string> keys = new List<string>(controller.playerData.playerData.generals.Keys);
            List<int> values = new List<int>(controller.playerData.playerData.generals.Values);
            for (int i = 0; i < generalSprites.Count; i++) {
                if (generalSprites[i].transform.GetChild(1).GetComponent<Toggle>().isOn) {
                    if (!controller.playerData.playerData.generalsInUse.ContainsKey(keys[i + generalOffset * 21])) {
                        controller.playerData.playerData.generalsInUse.Add(keys[i+ generalOffset * 21], values[i + generalOffset * 21]);
                    }
                }
            }
            foreach (Unit i in controller.soldiers) {
                if (i.country == controller.playerCountry && i.defaultGeneral == "" && i.general != "" && !new List<string>(controller.playerData.playerData.generalsInUse.Keys).Contains(i.general)) {
                   i.general = "";
                   i.updateGeneral(true);
                }
            }
        }

        Destroy(gameObject);
    }
    void checkLoadCustom() {
        if (isEditor) {
            if (CustomFunctions.PasteFromClipboard() == "") {
                loadCustomMapButton.interactable = false;
                loadCustomMapButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.7f);
            } else {
                try {
                    MapInfo m = JsonUtility.FromJson<MapInfo>(CustomFunctions.PasteFromClipboard());
                    loadCustomMapButton.interactable = true;
                    loadCustomMapButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 1.3f);

                } catch {
                    loadCustomMapButton.interactable = false;
                    loadCustomMapButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.7f);

                }
            }
        }
    }
    MyPlayerPrefs myPlayerPrefs;
    public Button primaryButton;

    IEnumerator LockCoroutine() {
        primaryButton.interactable = false;
        for (float i = lockPopupTime; i > 0; i -= Time.deltaTime) {
            yield return null;
        }
        primaryButton.interactable = true;

    }
    void Start() {
        if (lockPopupTime != 0)
            StartCoroutine(LockCoroutine());
        myPlayerPrefs = MyPlayerPrefs.instance;
        if (menuMasta != null && title != null) {
            if (tutorialNumber == 1) {
                title.text = CustomFunctions.TranslateText("Play tutorial 2?");
            } else if (tutorialNumber == 0) {
                title.text = CustomFunctions.TranslateText("Play tutorial?");

            }
        }
        if (uploadName != null) {
            uploadName.text = myPlayerPrefs.GetString("uploadName");
        }
        if (mapName != "") {
            if (myPlayerPrefs.GetInt("likedmaps" + mapName + mapAuthor) != 0)
                primaryButton.interactable = false;

        }
        if (isGeneralAssignment) {
            int index = 0;
            if (controller.playerData.playerData.generalsInUse != null) {
                foreach (KeyValuePair<string, int> i in controller.playerData.playerData.generalsInUse) {
                    if (index < generalOffset * 21) {
                        index++;
                        continue;
                        //yeayayaya
                    } else if (index >= (generalOffset + 1) * 21) {
                        canAddGeneralIndexPage = true;
                        break; //for next page
                    }
                    generalTextsDisplays[index - generalOffset * 21].text = CustomFunctions.TranslateText(i.Key);

                    

                    generalSprites[index - generalOffset * 21].sprite = controller.playerData.generalPhotos[i.Key];
                    int usingGeneral = 0;
                    foreach (Unit j in controller.soldiers) {
                        //default generals already assigned do not count towards general count
                        if (j != null && j.country == controller.playerCountry && j.general == i.Key && j.defaultGeneral != j.general) {
//                            print(j.currentTile.coordinate);
                            usingGeneral += 1; // j.tier;

                            //NOTE: CHANGED FROM ONE SLOT PER TIER TO ONE SLOT PER UNIT
                        }
                    }
                    if (i.Key != "default")
                        generalTextsDisplays[index - generalOffset * 21].text = generalTextsDisplays[index - generalOffset * 21].text + "\n" + usingGeneral + "/" + controller.playerData.generals[i.Key].maxCmdSize[i.Value];
                    else
                        generalTextsDisplays[index - generalOffset * 21].text = CustomFunctions.TranslateText("No General");


                    if (i.Key != "default" && (usingGeneral + /*controller.assignmentSoldier.tier*/ 1 > controller.playerData.generals[i.Key].maxCmdSize[i.Value] || controller.assignmentSoldier.general == i.Key))
                        generalSprites[index - generalOffset * 21].GetComponent<Button>().interactable = false;
                    index++;
                }
            }
            for (int i = index - generalOffset * 21; i < generalTextsDisplays.Count; i++) {
                generalTextsDisplays[i].enabled = false;
                generalSprites[i].enabled = false;

            }
        } else if (isPlayerGeneralAssignment) {
            previousGeneralDict = new Dictionary<string, int>(controller.playerData.playerData.generalsInUse);
            if (previousGeneralDict.ContainsKey("default"))
                previousGeneralDict.Remove("default");
            int index = 0;
            foreach (KeyValuePair<string, int> i in controller.playerData.playerData.generals) {
                if (index < generalOffset * 21) {
                    index++;
                    continue;
                } else if (index >= (generalOffset + 1) * 21) {
                    canAddGeneralIndexPage = true;
                    break; //for next page
                }
                if (previousGeneralDict.ContainsKey(i.Key)) { //remove previous key
                    previousGeneralDict.Remove(i.Key);
                }

                try {
                    generalTextsDisplays[index - generalOffset * 21].text = CustomFunctions.TranslateText(i.Key);
                    generalSprites[index - generalOffset * 21].sprite = controller.playerData.generalPhotos[i.Key];
                } catch {
                    print("general problem");
                }
                if (controller.playerData.playerData.generalsInUse != null) {
                    bool haveSameGeneral = false;
                    foreach (KeyValuePair<string, int> g in controller.playerData.playerData.generalsInUse) {
                        if (g.Key == i.Key) {
                            haveSameGeneral = true;
                            break;
                        }
                    }
                    generalSprites[index - generalOffset * 21].transform.GetChild(1).GetComponent<Toggle>().isOn = haveSameGeneral;
                } else if (index > controller.maxGenerals)
                    generalSprites[index - generalOffset * 21].transform.GetChild(1).GetComponent<Toggle>().isOn = false;
                index++;
            }
            for (int i = index - generalOffset * 21; i < generalTextsDisplays.Count; i++) {
                generalTextsDisplays[i].enabled = false; ;
                generalSprites[i].enabled = false;
            }
        }
        if (!customMap) {
            if (controller != null)
                controller.popupEnabled = true;
        } else {
            mapSelectDropdown.options = new List<Dropdown.OptionData>();
            
            
            //json file for all the datas to prevent MyPlayerPrefs clogging
            //if (myPlayerPrefs.GetString("customDataAll") == "" || myPlayerPrefs.GetInt("newSave") == 0) { //creates a new customdatas with migration
 
            //    myPlayerPrefs.SetString("customDataAll", " ");
            //    myPlayerPrefs.SetInt("currentSaveIndex", 1);
            //    myPlayerPrefs.SetInt("newSave", 1);
            //}
            int totalMaps = 0;
            foreach (string j in myPlayerPrefs.GetString("customDataAll").Split(',')) {
                try {
                    int i = int.Parse(j);
                    string s = myPlayerPrefs.GetString("customData" + i);
                    if (s != "") {
                        try {
                            mapSelectDropdown.options.Add(new Dropdown.OptionData(JsonUtility.FromJson<MapInfo>(s).missionName));
                            totalMaps++;
                        } catch {
                            //contentText.text = "map not available";
                            contentText.horizontalOverflow = HorizontalWrapMode.Overflow;

                            myPlayerPrefs.SetString("customData" + i, "");
                            RemoveMapFromAllList(i);
                        }

                        dropdownValues.Add(i);
                    }
                } catch/* (System.Exception e)*/ {
                    //contentText.text = "map parse: " + e;
                    contentText.horizontalOverflow = HorizontalWrapMode.Overflow;
                }
            }
            if (secondContentText != null) {
                if (totalMaps > 20) {
                    secondContentText.text = CustomFunctions.TranslateText("WARNING: you have a lot of maps and may crash the game more often. Maps: ") + totalMaps;
                } else
                    secondContentText.text = "";
            }
            checkLoadCustom();
        }
    }
    public void setIcon(Sprite sprite) {
        icon.sprite = sprite;
    }
    public void setText2(string t) {
        contentText.text = t;
    }

    public void setText(string t) {
        title.text = t;
    }
    Dictionary<string, int> previousGeneralDict; //all generals on current page are removed
    void Update() {
        if (multiplayerControl != null) {
            if (isCustomMatching) {
                if (myPlayerPrefs.GetInt("roomId") != 0) {
                    contentText.text = CustomFunctions.TranslateText("Room ID: ") + myPlayerPrefs.GetInt("roomId");
                }
            }
            if (!multiplayerControl.alreadyMatching && isInMatching) {
                Destroy(gameObject);
            }
        }
        if (generalIndex != null)
            generalIndex.text = generalOffset + 1 + "";
        if (uploadName != null) {
            myPlayerPrefs.SetString("uploadName", uploadName.text);
        }
        if (isPlayerGeneralAssignment) {
            int totalAssignedGenerals = 0;

            foreach (Image i in generalSprites) {
                if (i == null)
                    break;
                Toggle t = i.transform.GetChild(1).GetComponent<Toggle>();

                if (t.isOn)
                    totalAssignedGenerals++;
            }

            foreach (Image i in generalSprites) {
                Toggle t = i.transform.GetChild(1).GetComponent<Toggle>();

                if (!t.isOn  && totalAssignedGenerals + previousGeneralDict.Count >= controller.maxGenerals || !i.enabled) {
                    if (t.interactable)
                        t.interactable = false;
                } else {
                    if (!t.interactable)
                        t.interactable = true;
                }
            }
            contentText.text = CustomFunctions.TranslateText("Appointed Generals: ") + (totalAssignedGenerals + previousGeneralDict.Count) + "/" + controller.maxGenerals;
        }
        //all popups are in game scene so controller is accessable
        if (!customMap && controller != null)
            controller.popupEnabled = true;
        if (customMap)
            if (mapSelectDropdown.options.Count == 0) {
                if (isEditor) {
                    deleteButton.interactable = false;
                    deleteButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.7f);
                    saveMapButton.interactable = false;
                    saveMapButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.7f);

                    uploadButton.interactable = false;
                    uploadButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.68f);
                }
                loadMapButton.interactable = false;
                loadMapButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.7f);
            } else {
                if (isEditor) {

                    if (myPlayerPrefs.GetInt("canttouchmap" + dropdownValues[mapSelectDropdown.value]) == 0) {
                        if (PlayerData.instance.playerData.money >= 20) {
                            uploadButton.interactable = true;
                            uploadButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 3.3f);
                        } else {
                            uploadButton.interactable = false;
                            uploadButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.68f);
                        }
                        saveMapButton.interactable = true;
                        saveMapButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 1.3f);
                    } else {
                        if (SystemInfo.deviceType == DeviceType.Desktop) {
                            saveMapButton.interactable = true;
                            saveMapButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 1.3f);
                        } else {
                            saveMapButton.interactable = false;
                            saveMapButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.7f);
                        }
                        uploadButton.interactable = false;
                        uploadButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.68f);
                    }
                    deleteButton.interactable = true;
                    deleteButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 3.3f);
                }
                loadMapButton.interactable = true;
                loadMapButton.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, 3.1f);

            }
    }
}















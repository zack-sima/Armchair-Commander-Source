using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class OnlineMapInformation {
    
    public List<OnlineMapInformation2> strData;
    public OnlineMapInformation() {
        strData = new List<OnlineMapInformation2>();
        strData.Add(new OnlineMapInformation2());
        strData.Add(new OnlineMapInformation2());
        strData.Add(new OnlineMapInformation2());
    }
}
[System.Serializable]
public class OnlineMapInformation2 {
    public string map_name;
    public string author;
    public int views;
    public int completed_count;
    public int uid;
}
public enum DataType { popular, most_recent, author, featured }
public class OnlineMapsRetrive : MonoBehaviour {
    //IMPORTANT: WHETHER THIS ITEM IS ENABLED DETERMINES IF UNCENSORED SEARCH IS USED (hidden in normal versions so it is by default disabled)
    public Toggle censorToggle;

    public Text pageText;
    public InputField authorInput, mapInput;
    public GameObject mapQueryLsPrefab;

    void Start(){
        authorInput.text = MyPlayerPrefs.instance.GetString("authorInput");
        mapInput.text = MyPlayerPrefs.instance.GetString("mapInput");
        getData(0);
    }
    int pageIndex;
    public void nextPage() {
        if (!searching) {
            if (recentInformation != null && recentInformation.strData.Count > 5) {
                pageIndex++;

                StartCoroutine(getOnlineSearcher(deltaDatatype, deltaIsSearching));


            }
        }
    }
    public void previousPage() {
        if (!searching) {
            if (pageIndex > 0) {
                pageIndex--;
                StartCoroutine(getOnlineSearcher(deltaDatatype, deltaIsSearching));
            }
        }
    }
    bool deltaIsSearching;
    DataType deltaDatatype;
    public void searchData() {
        pageIndex = 0;
        if (!searching) {
            StartCoroutine(getOnlineSearcher(DataType.author, true));
            deltaDatatype = DataType.author;
            deltaIsSearching = true;
        }
    }
    public void getData(int requestType) {
        pageIndex = 0;
        if (!searching) {
            StartCoroutine(getOnlineSearcher((DataType)requestType, false));
            deltaDatatype = (DataType)requestType;
            deltaIsSearching = false;
        }
    }
    public void toMenu() {
        MyPlayerPrefs.instance.SaveData();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    OnlineMapInformation recentInformation = null;
    bool searching = false;
    List<GameObject> temporaryItems;
    void updateMaps(string returnedJsonQueryes) {
        if (temporaryItems != null)
            foreach (GameObject gameObj in temporaryItems)
                Destroy(gameObj);
        returnedJsonQueryes = returnedJsonQueryes.Substring(1, returnedJsonQueryes.Length - 2);
        returnedJsonQueryes = returnedJsonQueryes.Replace("'", "\"");
        print(returnedJsonQueryes);
        OnlineMapInformation mapInformation = JsonUtility.FromJson<OnlineMapInformation>(returnedJsonQueryes);
        recentInformation = mapInformation;


        int i = 0;
        temporaryItems = new List<GameObject>();
        foreach (OnlineMapInformation2 val in mapInformation.strData) {
            if (i > 4)
                continue;
            OnlineMapQueryForm insItem = Instantiate(mapQueryLsPrefab, GameObject.Find("Canvas").transform.GetChild(0)).GetComponent<OnlineMapQueryForm>();
            insItem.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2f - GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor * 75 - ((i - 2) * 102.5f * GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor));
            insItem.mapName = val.map_name;
            insItem.mapAuthor = val.author;
            insItem.downloads = val.views;
            insItem.mapId = val.uid;

            insItem.thumbsUp = val.completed_count;
            temporaryItems.Add(insItem.gameObject);
            i += 1;
        }


        
        
    }
    IEnumerator getOnlineSearcher(DataType request, bool isSearching) {
        UnityWebRequest r;
        searching = true;

        string uncensoredString = "";
        if (censorToggle.isOn) {
            uncensoredString = "_uncensored";
        }

        if (isSearching) {
            string url = CustomFunctions.getURL();
            if (authorInput.text != "")
                url += "author" + uncensoredString + "?author=" + authorInput.text + "&item_index=" + (pageIndex * 5) + "&count=6";
            else
                url += "map_name" + uncensoredString + "?map_name=" + mapInput.text + "&item_index=" + (pageIndex * 5) + "&count=7";
            print(url);
            r = UnityWebRequest.Get(CustomFunctions.ConvertToUtf8(url));
        } else
            r = UnityWebRequest.Get(CustomFunctions.ConvertToUtf8(CustomFunctions.getURL() + request.ToString() + "?item_index=" + (pageIndex * 5) + "&count=6"));
        yield return r.SendWebRequest();
 //       print(r.url);
//        print(r.downloadHandler.text);
        if (r.downloadHandler.text != "") {
            try {
                updateMaps(r.downloadHandler.text);
            } catch {
            }
         }
        searching = false;
        
    }
    void Update() {
        MyPlayerPrefs.instance.SetString("authorInput", authorInput.text);
        MyPlayerPrefs.instance.SetString("mapInput", mapInput.text);
        pageText.text = pageIndex + 1 + "";
        if (authorInput.text != "" && authorInput.isFocused)
            mapInput.text = "";
        else if (mapInput.text != "" && mapInput.isFocused)
            authorInput.text = "";
    }
}
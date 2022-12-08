using System.Collections;
using System.Collections.Generic;
using System.IO; //input output
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

//MyPlayerPrefs for local storage; it is made to exist in scenes to improve efficiency
public class MyPlayerPrefs : MonoBehaviour {
    public AudioSource buttonSound;
    PlayerPrefsData d = null;
    public static MyPlayerPrefs instance;
    public PlayerPrefsData GetData() {
        if (d == null) {
            Debug.LogWarning("export data not found; returning default");
            return new PlayerPrefsData();
        }

        return d;
    }
    public void OverrideData(PlayerPrefsData newData) {
        d = newData;
        SaveData();
    }
    void Awake() {
        DontDestroyOnLoad(gameObject);
        if (instance == null) {
            instance = this;
            d = LoadData();

            //TODO: THIS CODE IS FROM OVER A YEAR AGO WHEN THE MOVE FROM PLAYERPREFS TO MYPLAYERPREFS WAS MADE; CODE IS NOW OBSOLETE AND CAN BE DELETED
            //if (PlayerPrefs.GetString("customDataAll") != "" && GetString("customDataAll") == "") {
            //    print(PlayerPrefs.GetString("customDataAll"));
            //    foreach (string j in PlayerPrefs.GetString("customDataAll").Split(',')) {
            //        try {
            //            int i = int.Parse(j);
            //            string s = PlayerPrefs.GetString("customData" + i); //this one remains PlayerPrefs for old storage system
            //            SetString("customData" + i, s);
            //            SetString("canttouchmap" + i, PlayerPrefs.GetString("canttouchmap" + i));
            //        } catch (System.Exception e) {
            //            print(e);
            //        }
            //    }
            //    SetString("customDataAll", PlayerPrefs.GetString("customDataAll"));
            //    SetInt("finishedTutorial", 2);
            //    SetString("language", PlayerPrefs.GetString("language"));

            //    SetInt("disableAds", PlayerPrefs.GetInt("disableAds"));
            //    SetInt("availableVideos", 5);
            //    SetInt("newSave", 1);

            //    SaveData();
            //} else if (PlayerPrefs.GetString("customDataAll") == "" && GetString("customDataAll") == "") {
            //    PlayerPrefs.SetString("customDataAll", " ");
            //SetInt("newSave", 1);
            //SetInt("currentSaveIndex", 1);
            //}

            //don't allow the game to start already above the interstitial threshold so that no ads show as soon as the player starts the game
            if (GetInt("playAdsTimer") >= 3) {
                SetInt("playAdsTimer", 2);
                SaveData();
            }
            //StartCoroutine(RecurringSave());

        } else {
            Destroy(gameObject);
            return;
        }
    }
    public void SetInt(string key, int value) {
        d.SetInt(key, value);
    }
    public void SetFloat(string key, float value) {
        d.SetFloat(key, value);
    }
    public void SetString(string key, string value) {
        d.SetString(key, value);
    }
    public bool HasKey(string key) {
        PlayerPrefsData d = LoadData();

        return d.intDict.ContainsKey(key) || d.floatDict.ContainsKey(key) || d.stringDict.ContainsKey(key);
    }
    public void DeleteKey(string key) {
        PlayerPrefsData d = LoadData();
        if (d.intDict.ContainsKey(key))
            d.intDict.Remove(key);
        if (d.floatDict.ContainsKey(key))

            d.floatDict.Remove(key);
        if (d.stringDict.ContainsKey(key))

            d.stringDict.Remove(key);

    }
    public int GetInt(string key) {
        return d.GetInt(key);
    }
    public float GetFloat(string key) {
        return d.GetFloat(key);

    }
    public string GetString(string key) {
        return d.GetString(key);

    }
    IEnumerator RecurringSave() {
        while (true) {
            for (float i = 0f; i < 2f; i += Time.deltaTime) {
                yield return null;
            }

            SaveData();

        }
    }


    /// <summary>
    /// WARNING: data given here will override existing file
    /// </summary>
    /// <param name="data">override data</param>
    public void SaveData(PlayerPrefsData data=null) {
        if (data == null) {
            if (d == null) {
                print("data not present");
                return;
            } else {
                data = d;
            }
        }


        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(GetDataPath() + "/myPlayerPrefs.sav", FileMode.Create);

        bf.Serialize(stream, data);
        stream.Close();
        SaveBackupData(data);
    }
    public void SaveBackupData(PlayerPrefsData data) {
        //must save backup
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(GetDataPath() + "/myPlayerPrefsBackup.sav", FileMode.Create);

        bf.Serialize(stream, data);

        stream.Close();
    }
    public static string GetDataPath() {
        string dataPath = Application.persistentDataPath;
#if UNITY_EDITOR
        dataPath = Application.dataPath + "/EditorData";
#endif
#if !UNITY_WEBGL
        if (SystemInfo.deviceType == DeviceType.Desktop && !Application.isEditor) {
            dataPath = Application.dataPath;
            //dataPath = Application.persistentDataPath;


        }
#endif
        return dataPath;

    }

    public PlayerPrefsData LoadBackupData() {
        if (File.Exists(GetDataPath() + "/myPlayerPrefsBackup.sav")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(GetDataPath() + "/myPlayerPrefsBackup.sav", FileMode.Open);
            PlayerPrefsData data;
            try {
                data = bf.Deserialize(stream) as PlayerPrefsData;
                stream.Close();
            } catch {
                stream.Close();
                Debug.Log("cannot parse backup so it means something is wrong now");

                return new PlayerPrefsData();
            }
            return data;

        } else {
            Debug.Log("backup not found");
            return new PlayerPrefsData();
        }
    }
    //add integer parameter for different maps
    public PlayerPrefsData LoadData() {
        if (File.Exists(GetDataPath() + "/myPlayerPrefs.sav")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(GetDataPath() + "/myPlayerPrefs.sav", FileMode.Open);
            PlayerPrefsData data;
            try {
                data = bf.Deserialize(stream) as PlayerPrefsData;
                stream.Close();
            } catch {
                stream.Close();
                Debug.Log("cannot parse old data so trying backup now");

                return LoadBackupData();
            }
            SaveBackupData(data);
            return data;

        } else if (File.Exists(GetDataPath() + "/myPlayerPrefsBackup.sav")) {
            Debug.Log("trying backup");
            return LoadBackupData();
        } else {
            Debug.Log("new data");
            return new PlayerPrefsData();
        }
    }
    void Update() {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.P)) {
            //no canvas screenshot
            GameObject go = GameObject.Find("Canvas");
            if (go == null)
                return;
            go.SetActive(false);
            ScreenCapture.CaptureScreenshot("/Users/Fudgehead/Desktop/SC_" + Screen.width + "x" + Screen.height + "_" + System.DateTime.UtcNow.ToLongTimeString() + ".png");

            StartCoroutine(ReturnCanvas(go));
        } else if (Input.GetKey(KeyCode.C) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.P)) {
            ScreenCapture.CaptureScreenshot("/Users/Fudgehead/Desktop/SC_" + Screen.width + "x" + Screen.height + "_" + System.DateTime.UtcNow.ToLongTimeString() + ".png");
        }
#endif
    }
    IEnumerator ReturnCanvas(GameObject canvas) {
        for (float i = 0; i < 0.5f; i += Time.deltaTime)
            yield return null;
        canvas.SetActive(true);
    }
}

[System.Serializable]
public class PlayerPrefsData {
    public Dictionary<string, int> intDict;
    public Dictionary<string, float> floatDict;
    public Dictionary<string, string> stringDict;
    public PlayerPrefsData() {
        intDict = new Dictionary<string, int>();
        floatDict = new Dictionary<string, float>();
        stringDict = new Dictionary<string, string>();
    }
    public void SetInt(string key, int value) {
        if (intDict.ContainsKey(key)) {
            intDict[key] = value;
        } else {
            intDict.Add(key, value);
        }
    }
    public void SetFloat(string key, float value) {
        if (floatDict.ContainsKey(key)) {
            floatDict[key] = value;
        } else {
            floatDict.Add(key, value);
        }
    }
    public void SetString(string key, string value) {
        if (stringDict.ContainsKey(key)) {
            stringDict[key] = value;
        } else {
            stringDict.Add(key, value);
        }
    }

    public int GetInt(string key) {
        if (intDict.ContainsKey(key)) {
            return intDict[key];
        } else {
            return 0;
        }
    }
    public float GetFloat(string key) {
        if (floatDict.ContainsKey(key)) {
            return floatDict[key];
        } else {
            return 0f;
        }
    }
    public string GetString(string key) {
        if (stringDict.ContainsKey(key)) {
            return stringDict[key];
        } else {
            return "";
        }
    }

}



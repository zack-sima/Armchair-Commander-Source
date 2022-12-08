using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;

//this script is used across both the matching scene and gaming scene, where all the network calls
//are called from external script
public class MultiplayerLobby : MonoBehaviour {
    public GameObject matchPopupPrefab, matchSelectPrefab, customFailedPrefab;
    public MenuManager menuController; //only one will be assigned
    public Controller gameController;
    public bool alreadyMatching = false;
    //use your own ip! (don't flood my server with modded maps)
    string link = "http://www.example.com:9999/";
    string localLink = "localhost:8005/";
    readonly bool useLocal = false;
    [Serializable]
    public class Room {
        public List<string> player_names;
        public List<string> player_timeouts;
        public List<string> player_countries;
        public int map_id;
        public string map_data;
        public int current_player;
        public string random_id;
        public int map_view_only; //0 for no, 1 for yes
    }
    public Room matchInfo;
    void Start()
    {
        if (useLocal)
            link = localLink;
    }
    public void MatchPopup() {
        IngamePopup insItem = Instantiate(matchSelectPrefab, GameObject.Find("Canvas").transform).GetComponent<IngamePopup>();
        insItem.transform.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
        insItem.multiplayerControl = this;
    }
    public void StartMatching(bool matching, bool defaultMap=true, string customMap="", string playerCountry="", string opponentCountry="") {
        if (!alreadyMatching) {
            IngamePopup insItem = Instantiate(matchPopupPrefab, GameObject.Find("Canvas").transform).GetComponent<IngamePopup>();
            insItem.transform.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
            insItem.multiplayerControl = this;
            insItem.isCustomMatching = !matching;
            insItem.isInMatching = true;
            StartCoroutine(GetMatchId(matching, false, defaultMap:defaultMap, customMap:customMap, playerCountry:playerCountry, opponentCountry:opponentCountry));
            alreadyMatching = true;
        }
    }
    public void JoinMatch(string roomId) { //custom
        if (!alreadyMatching) {
            IngamePopup insItem = Instantiate(matchPopupPrefab, GameObject.Find("Canvas").transform).GetComponent<IngamePopup>();
            insItem.transform.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
            insItem.multiplayerControl = this;
            insItem.isInMatching = true;

            StartCoroutine(GetMatchId(false, true, roomId));
            alreadyMatching = true;
        }
    }

    IEnumerator GetMatchId(bool matching, bool joinGame, string roomId="", bool defaultMap = true, string customMap = "", string playerCountry = "", string opponentCountry = "") { //change name to custom name
        MyPlayerPrefs.instance.SetInt("roomId", 0);
        //TODO: add post request for setting up room
        UnityWebRequest r;
        if (matching) {
            r = UnityWebRequest.Get(link + "assign_match?player_name=player&matching=true");
        } else if (joinGame) {
            r = UnityWebRequest.Get(link + "join_match?player_name=player&room_id=" + roomId);
        } else if (defaultMap) {//host default game
            r = UnityWebRequest.Get(link + "assign_match?player_name=player&matching=false");
        } else {//host custom game
            WWWForm f = new WWWForm();

            f.AddField("player_name", "player");
            f.AddField("map_data", customMap);
            f.AddField("player_country", playerCountry);
            f.AddField("opponent_country", opponentCountry);
            r = UnityWebRequest.Post(link + "custom_match", f);
        }

        yield return r.SendWebRequest();
        string returnString = r.downloadHandler.text;
        //print(returnString.Split(',')[0].Substring(1));
        //print(returnString.Split(',')[1].Substring(0, returnString.Split(',')[1].Length - 1));
        print(r.downloadHandler.text);
        if (joinGame && r.downloadHandler.text == "-1") {
            //match not found
            StopMatching();
            print("cannot find room");
        } else {
            bool stopGame = false;
            try {
                MyPlayerPrefs.instance.SetInt("roomId", int.Parse(returnString.Split(',')[0].Substring(1)));
                MyPlayerPrefs.instance.SetInt("playerId", int.Parse(returnString.Split(',')[1].Substring(0, returnString.Split(',')[1].Length - 1)));
            } catch {
                //connection broken
                stopGame = true;
                StopMatching();
                IngamePopup insItem = Instantiate(customFailedPrefab, GameObject.Find("Canvas").transform).GetComponent<IngamePopup>();
                insItem.transform.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
            }
            if (!stopGame) {
                while (true) {
                    for (float i = 0f; i < 2.5f; i += Time.deltaTime) {
                        yield return null;
                    }
                    StartCoroutine(CheckMatchStarted());
                }
            }
        }
    }
    //routinely called before match is joined
    IEnumerator CheckMatchStarted() {
        UnityWebRequest r = UnityWebRequest.Get(link + "check_room?room_id=" + MyPlayerPrefs.instance.GetInt("roomId") + "&player_id=" + MyPlayerPrefs.instance.GetInt("playerId"));
        yield return r.SendWebRequest();
        print(r.downloadHandler.text);
        try {
            if (r.downloadHandler.text != "0") {
                matchInfo = JsonUtility.FromJson<Room>(r.downloadHandler.text);
                MyPlayerPrefs.instance.SetString("multiplayerMap", matchInfo.map_data); //if empty, use default map; otherwise use this
                MyPlayerPrefs.instance.SetString("playerCountry", matchInfo.player_countries[MyPlayerPrefs.instance.GetInt("playerId")]);
                print(MyPlayerPrefs.instance.GetString("playerCountry"));
                menuController.GoToMultiplayer(matchInfo.map_id);
            } else {
                print("waiting for players");
            }
        } catch {
            StopMatching();
            Debug.LogError("lost connection on this packet");
        }
    }
    public void StopMatching() {
        StopAllCoroutines();
        alreadyMatching = false;
        
        StartCoroutine(StopMatch());
    }
    IEnumerator StopMatch() {
        UnityWebRequest r = UnityWebRequest.Get(link + "leave_game?room_id=" + MyPlayerPrefs.instance.GetInt("roomId") + "&player_id=" + MyPlayerPrefs.instance.GetInt("playerId"));
        yield return r.SendWebRequest();
    }
    string randomMapId = "0";
    //routinely called; calls get
    public IEnumerator GetMatchInfo() {
        UnityWebRequest r = UnityWebRequest.Get(link + "check_turns?room_id=" + MyPlayerPrefs.instance.GetInt("roomId") + "&player_id=" + MyPlayerPrefs.instance.GetInt("playerId"));
        yield return r.SendWebRequest();
        try {
            matchInfo = JsonUtility.FromJson<Room>(r.downloadHandler.text);
            
            if (matchInfo.random_id != randomMapId && gameController.passingRound && !gameController.multiplayerAIMovement) {
                StartCoroutine(GetMapData());
            } else {
                if (matchInfo.player_names.Count == 1) { //this guy is the only one left! If implementing more than 2 people, add factions
                    gameController.endGamePopup(true, "Opponent left the match");
                }
            }
        } catch {
            if (r.downloadHandler.text == "-1" && !gameController.gameOver) {
                //add a disconnected/lost screen
                gameController.endGamePopup(false, "Connection lost");
            }
            print("lost connection on this packet");
        }
    }
    IEnumerator GetMapData() {
        UnityWebRequest r = UnityWebRequest.Get(link + "get_map?room_id=" + MyPlayerPrefs.instance.GetInt("roomId"));

        yield return r.SendWebRequest();
        if (randomMapId != matchInfo.random_id && gameController.passingRound) {
            if (gameController.LoadMidgameBinary(r.downloadHandler.text)) {
                randomMapId = matchInfo.random_id;
                print("map retrieved successfully");
            } else if (matchInfo.map_view_only == 0) { //sending probably failed
                print("map could not be retrieved");
                for (float i = 0f; i < 2f; i += Time.deltaTime) {
                    yield return null;
                }
                StartCoroutine(GetMapData());
            }
        } else {
            print("already gotten map!");
        }
    }
    public IEnumerator UploadMap(BinaryData d, bool viewOnly) {
        string data = BinarySaveSystem.ExportToJson(d);
        WWWForm f = new WWWForm();

        f.AddField("room_id", MyPlayerPrefs.instance.GetInt("roomId"));
        f.AddField("player_id", MyPlayerPrefs.instance.GetInt("playerId"));
        f.AddField("json_data", data);

        f.AddField("map_view_only", viewOnly ? 1 : 0);

        f.AddField("random_id", d.randomId.ToString());
        UnityWebRequest r = UnityWebRequest.Post(link + "upload_data", f);

        yield return r.SendWebRequest();
        if (r.downloadHandler.text != "0") { //upload probably failed, so should do it again
            if (r.downloadHandler.text == "-2") {
                //should not be able to play round (not this player's turn)
            } else if (!viewOnly) { //view only maps are not very important
                StartCoroutine(UploadMap(d, false));
            }
            Debug.LogError("upload failed");
        }
    }
    void Update()
    {
        
    }
}

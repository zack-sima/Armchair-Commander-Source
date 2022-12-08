using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class OnlineMapQueryForm : MonoBehaviour {
    public Image mapIcon;
    public Sprite mailReadIcon;
    public Text mapNameText, authorNameText, downloadsTxt, thumbsUpTxt;
    public string mapName, mapAuthor;
    public int downloads, thumbsUp, mapId;

    void Start() {
        mapNameText.text = mapName;
        authorNameText.text = mapAuthor;

        downloadsTxt.text = downloads.ToString();
        thumbsUpTxt.text = thumbsUp.ToString();

        if (downloads == -1) {
            downloadsTxt.text = "-";

            thumbsUpTxt.text = "-";
        }

        if (MyPlayerPrefs.instance.GetInt("downloaded" + mapAuthor + mapName) == 1) {
            mapIcon.sprite = mailReadIcon;
        }
        
    }
    public GameObject downloadMapPrefab;
    public void downloadMap() {
        Transform insItem = Instantiate(downloadMapPrefab, GameObject.Find("Canvas").transform).transform;
        IngamePopup p = insItem.GetComponent<IngamePopup>();
        insItem.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
        p.mapName = mapName;
        p.mapAuthor = mapAuthor;
        p.mapId = mapId;
    }
    // Update is called once per frame
    void Update() {

    }
}

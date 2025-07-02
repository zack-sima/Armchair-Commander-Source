using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RecentMessages : MonoBehaviour {
	string language = "English";
	bool messageReady = false;
	public Image RedDotImage;
	void Start() {
		language = MyPlayerPrefs.instance.GetString("language");
		StartCoroutine(GetMessage());

	}
	public GameObject popupMsgPrefab;
	string displayedMessage;
	IEnumerator GetMessage() {
		UnityWebRequest r = UnityWebRequest.Get("http://main.indiewargames.net:8001/get_recent_messages?language=" + language);
		yield return r.SendWebRequest();
		/////////        print(r.downloadHandler.text);
		displayedMessage = r.downloadHandler.text.Substring(1, r.downloadHandler.text.Length - 2);
		//returns web time
		//for rewards
		if (language == "Chinese") {
			if (MyPlayerPrefs.instance.GetString("messages_chinese") != displayedMessage)
				RedDotImage.enabled = true;
		} else {
			if (MyPlayerPrefs.instance.GetString("messages_english") != displayedMessage)
				RedDotImage.enabled = true;
		}
		messageReady = true;
	}
	//called to show message
	public void ShowMessage() {
		if (messageReady) {
			if (language == "Chinese") {
				MyPlayerPrefs.instance.SetString("messages_chinese", displayedMessage);
				RedDotImage.enabled = false;
			} else {
				MyPlayerPrefs.instance.SetString("messages_english", displayedMessage);
				RedDotImage.enabled = false;
			}
			CreatePopup(popupMsgPrefab, displayedMessage);
		}
	}
	void CreatePopup(GameObject g, string msg) {
		GameObject insItem = Instantiate(g, GameObject.Find("Canvas").transform);
		insItem.transform.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
		if (msg.Contains("http")) {
			string linkStr = msg.Substring(msg.IndexOf("http"));
			linkStr = linkStr.Substring(0, linkStr.IndexOf("~"));
			insItem.GetComponent<IngamePopup>().embedLink = linkStr;
		} else {
			Destroy(insItem.GetComponent<IngamePopup>().primaryButton.gameObject);
		}
		msg = msg.Replace("~", "");
		insItem.GetComponent<IngamePopup>().couponCode.text = msg;
	}
	void Update() {

	}
}

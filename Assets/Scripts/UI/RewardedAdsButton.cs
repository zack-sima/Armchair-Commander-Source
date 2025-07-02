using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;
using System;

[RequireComponent(typeof(Button))]
public class RewardedAdsButton : MonoBehaviour {
	public GameObject rewardPrefab, showRewardedPrefab;
	Button myButton;
	MyPlayerPrefs myPlayerPrefs;
	public string myPlacementId = "rewardedVideo";

	IEnumerator CheckTime() {
		UnityWebRequest r = UnityWebRequest.Get("http://main.indiewargames.net:8080/server_time");
		yield return r.SendWebRequest();
		//returns web time
		//for rewards
		long outputTimeframe;
		if (long.TryParse(r.downloadHandler.text.Split('.')[0], out outputTimeframe)) {
			int newOutputTimeFrame = (int)(outputTimeframe % 1000000);
			if (myPlayerPrefs.GetInt("lastVideoWatchTime") < newOutputTimeFrame - 25000 || myPlayerPrefs.GetInt("lastVideoWatchTime") > newOutputTimeFrame + 360000) {
				myPlayerPrefs.SetInt("availableVideos", 5);
				//print("refreshed");
				myPlayerPrefs.SetInt("lastVideoWatchTime", newOutputTimeFrame);
			}
		}
	}
	AdMobInitializer admobControl = null;
	void Start() {
#if UNITY_IOS
#elif UNITY_ANDROID
#elif !UNITY_EDITOR
        Destroy(gameObject);
#endif
		admobControl = GameObject.Find("AdMobManager").GetComponent<AdMobInitializer>();
		myPlayerPrefs = MyPlayerPrefs.instance;

		if (!myPlayerPrefs.HasKey("availableVideos"))
			myPlayerPrefs.SetInt("availableVideos", 5);
		StartCoroutine(CheckTime());
		if (myPlayerPrefs.GetInt("rewardedAmount") != 0) {
			GameObject popupPrefabs = Instantiate(rewardPrefab, GameObject.Find("Canvas").transform);
			popupPrefabs.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2, 0);
			popupPrefabs.GetComponent<IngamePopup>().title.text = CustomFunctions.TranslateText("You received") + " " + myPlayerPrefs.GetInt("rewardedAmount") + " " + CustomFunctions.TranslateText("coins!");
			myPlayerPrefs.SetInt("rewardedAmount", 0);
		}

		myButton = GetComponent<Button>();

		// Map the ShowRewardedVideo function to the button’s click listener:
		if (myButton) myButton.onClick.AddListener(ShowRewardedVideo);



	}
	void Update() {
		try {
			if (admobControl == null || admobControl.rewardedAd == null || admobControl.interstitialAd == null)
				return;

			if (!PlayerData.instance.playerData.removedAds && admobControl.interstitialAd.CanShowAd() &&
				myPlayerPrefs.GetInt("playAdsTimer") >= 3) {
				ShowInterstitial();
			}

			// Debug.Log(admobControl.rewardedAd.CanShowAd());
			if (admobControl.rewardedAd.CanShowAd() && myPlayerPrefs.GetInt("availableVideos") > 0) {
				myButton.interactable = true;
				myButton.transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("FREE COINS!");
			}
			if (myPlayerPrefs.GetInt("availableVideos") <= 0) {
				myButton.transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("CHECK LATER!");
			}
		} catch (Exception e) {
			UnityEngine.Debug.Log(e);
		}


	}
	// Implement a function for showing a rewarded video ad:
	void ShowRewardedVideo() {
		GameObject popupPrefabs = Instantiate(showRewardedPrefab, GameObject.Find("Canvas").transform);
		popupPrefabs.transform.position = new Vector3(Screen.width / 2f, Screen.height / 2, 0);
		popupPrefabs.GetComponent<IngamePopup>().callback.AddListener(ActuallyShowVideo);
	}
	void ActuallyShowVideo() {
		admobControl.ShowRewardedAd();
	}
	void ShowInterstitial() {
		admobControl.ShowInterstitialAd();
	}
	public void ClosedAd() {

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void RewardPlayer() {
		print("finished and rewarded!");
		int reward = UnityEngine.Random.Range(75, 125);
		PlayerData.instance.playerData.money += reward;

		//display popup for reward (set persistent prefs, checked at player data)
		MyPlayerPrefs.instance.SetInt("rewardedAmount", reward);
		MyPlayerPrefs.instance.SetInt("availableVideos", MyPlayerPrefs.instance.GetInt("availableVideos") - 1);
		print(MyPlayerPrefs.instance.GetInt("availableVideos"));

		// Reward the user for watching the ad to completion.
		PlayerData.instance.saveFile();
		MyPlayerPrefs.instance.SaveData();
	}
}
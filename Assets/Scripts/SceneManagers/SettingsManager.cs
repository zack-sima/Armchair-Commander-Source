using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {
	MyPlayerPrefs myPlayerPrefs;
	public Image muteButton, muteMusicButton;
	public Sprite muteSprite, unmuteSprite, muteMusicSprite, unmuteMusicSprite;
	public Dropdown languageDropdown, optimizationDropdown;
	public GameObject redeemMoneyPrefab;
	public Toggle policyToggle;
	public Text directoryText; //for PC users looking for their directory

	public void GoToPolicy() {
		Application.OpenURL("https://retrocombat.wordpress.com/community-guidelines/");
	}
	public void GoToMusic() {
		Application.OpenURL("https://lucaskneipp.com");
	}
	public void RedeemMoney() {
		Transform insItem = Instantiate(redeemMoneyPrefab, GameObject.Find("Canvas").transform).transform;
		insItem.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
	}
	void Update() {
		myPlayerPrefs.SetInt("agreeToPolicy", policyToggle.isOn ? 1 : 0);
	}
	bool started = false;
	public Button loadDataButton;
	void Start() {
#if !UNITY_IOS && !UNITY_ANDROID
		directoryText.text = "Data directory:\n" + MyPlayerPrefs.GetDataPath();
#endif
		myPlayerPrefs = MyPlayerPrefs.instance;
		policyToggle.isOn = myPlayerPrefs.GetInt("agreeToPolicy") == 1;

		try {
			string s = CustomFunctions.PasteFromClipboard();
			ExportBinaryData b = JsonUtility.FromJson<ExportBinaryData>(s);
			if (b.levelsUnlocked.Count > 0 && b.CheckJsonHash())
				loadDataButton.interactable = true;
		} catch/* (System.Exception e)*/ {
			// print(e);
		}

		muteButton.sprite = myPlayerPrefs.GetInt("mutedUI") == 1 ? muteSprite : unmuteSprite;
		muteMusicButton.sprite = myPlayerPrefs.GetInt("mutedMusicUI") == 1 ? muteMusicSprite : unmuteMusicSprite;


		optimizationDropdown.value = myPlayerPrefs.GetInt("optimization") + 1;

		switch (myPlayerPrefs.GetString("language")) {
			case "Chinese":
				languageDropdown.value = 1;
				break;
			case "Spanish":
				languageDropdown.value = 2;
				break;
			case "French":
				languageDropdown.value = 3;
				break;
			case "Russian":
				languageDropdown.value = 4;
				break;
			case "Japanese":
				languageDropdown.value = 5;
				break;
		}

		started = true;
	}
	public void LoadScene(int index) {
		SceneManager.LoadScene(index);
	}
	//NOTE: EXPORTING LOCAL DATA NEEDS DEVICE ID SO IT CAN'T BE COPIED TO ANOTHER DEVICE BUT MAPS CAN
	public void ExportSystemConfig() {
		CustomFunctions.CopyToClipboard(JsonUtility.ToJson(new ExportBinaryData(PlayerData.instance.playerData, myPlayerPrefs.GetData())));
		loadDataButton.interactable = true;
	}


	bool loaded = false;
	IEnumerator ConfirmLoadCountdown(Text t) {
		for (float i = 0f; i < 2f; i += Time.deltaTime) {
			yield return null;
			if (!loaded)
				break;
		}
		loaded = false;
		t.text = CustomFunctions.TranslateText("Replace From Clipboard");
	}

	//WARNING: WILL OVERRIDE ALL EXISTING DATA, NEEDS TO BE CONFIRMED
	public void ImportSystemConfig(Text t) {
		if (!loaded) {
			StartCoroutine(ConfirmLoadCountdown(t));
			t.text = CustomFunctions.TranslateText("Confirm?");
			loaded = true;
			return;
		}
		t.text = CustomFunctions.TranslateText("Replace From Clipboard");
		try {
			ExportBinaryData b = JsonUtility.FromJson<ExportBinaryData>(CustomFunctions.PasteFromClipboard());
			if (b.levelsUnlocked.Count == 0 || !b.CheckJsonHash())
				throw new System.Exception();
			PlayerData.instance.playerData = new BinaryData2(b);
			PlayerPrefsData d = new PlayerPrefsData();
			d.intDict = b.ppIntKeys.Zip(b.ppIntValues, (k, v) => new { k, v })
				  .ToDictionary(x => x.k, x => x.v);
			d.floatDict = b.ppFloatKeys.Zip(b.ppFloatValues, (k, v) => new { k, v })
				  .ToDictionary(x => x.k, x => x.v);
			d.stringDict = b.ppStringKeys.Zip(b.ppStringValues, (k, v) => new { k, v })
				  .ToDictionary(x => x.k, x => x.v);
			myPlayerPrefs.OverrideData(d);
		} catch (System.Exception e) {
			print(e);
		}
	}
	public void LanguageUpdated() {
		switch (languageDropdown.value) {
			case 0:
				myPlayerPrefs.SetString("language", "English");
				break;
			case 1:
				myPlayerPrefs.SetString("language", "Chinese");
				break;
			case 2:
				myPlayerPrefs.SetString("language", "Spanish");
				break;
			case 3:
				myPlayerPrefs.SetString("language", "French");
				break;
			case 4:
				myPlayerPrefs.SetString("language", "Russian");
				break;
			case 5:
				myPlayerPrefs.SetString("language", "Japanese");
				break;
		}
		if (started) {
			myPlayerPrefs.SaveData();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
	public void OptimizationUpdated() {
		myPlayerPrefs.SetInt("optimization", optimizationDropdown.value - 1);
		myPlayerPrefs.SaveData();
	}
	public void ToggleMute() {
		myPlayerPrefs.SetInt("mutedUI", myPlayerPrefs.GetInt("mutedUI") == 0 ? 1 : 0);
		muteButton.sprite = myPlayerPrefs.GetInt("mutedUI") == 1 ? muteSprite : unmuteSprite;
	}
	public void ToggleMusicMute() {
		myPlayerPrefs.SetInt("mutedMusicUI", myPlayerPrefs.GetInt("mutedMusicUI") == 0 ? 1 : 0);
		muteMusicButton.sprite = myPlayerPrefs.GetInt("mutedMusicUI") == 1 ? muteMusicSprite : unmuteMusicSprite;
	}
}
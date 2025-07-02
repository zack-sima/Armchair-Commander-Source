using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shop1Manager : MonoBehaviour {
	public GameObject shopButton, iapButton, techButton; //disable currently for iOS
	public Dropdown countryDropdown;
	public Button leftButton, rightButton;
	public Text moneyText;
	public int generalScrollIndex; //arrow keys
	public Image[] generalPurchaseSlots;
	public void addScrollIndex() {
		if (CheckScrollAdd()) {
			generalScrollIndex++;
			updateGenerals();
		}
	}
	bool CheckScrollAdd() {
		return newPlayerShopGenerals.Keys.Count > 5 * generalScrollIndex + 5;
	}

	public GameObject generalPopupPrefab;
	public void manageGeneral(int buttonIndex) {
		Transform insItem = Instantiate(generalPopupPrefab, GameObject.Find("Canvas").transform).transform;
		insItem.GetComponent<GeneralManagerPopup>().playerData = PlayerData.instance;
		insItem.GetComponent<GeneralManagerPopup>().controller = this;

		insItem.GetComponent<GeneralManagerPopup>().generalName = new List<string>(newPlayerShopGenerals.Keys)[generalScrollIndex * 5 + buttonIndex];
		insItem.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
	}
	public void subScrollIndex() {
		if (generalScrollIndex > 0) {
			generalScrollIndex--;
			updateGenerals();
		}
	}
	Dictionary<string, General> newPlayerShopGenerals;
	public void updateGenerals() {
		newPlayerShopGenerals = countryDropdown.value != 0 ? new Dictionary<string, General>() : PlayerData.instance.playerShopGenerals;

		if (countryDropdown.value != 0) {
			foreach (General g in PlayerData.instance.playerShopGenerals.Values) {
				if (availableCountries[countryDropdown.value - 1] == g.country) {
					print(g.name);
					newPlayerShopGenerals.Add(g.name, g);
				}
			}
		}

		List<string> gk = new List<string>(newPlayerShopGenerals.Keys);
		for (int i = 0; i < generalPurchaseSlots.Length; i++) {
			if (newPlayerShopGenerals.Keys.Count > 5 * generalScrollIndex + i) {
				//print(gk[5 * generalScrollIndex + i + 1]);
				generalPurchaseSlots[i].enabled = true;

				try {
					generalPurchaseSlots[i].sprite = PlayerData.instance.generalPhotos[gk[5 * generalScrollIndex + i]];
				} catch {
					print(gk[5 * generalScrollIndex + i]);
				}
				generalPurchaseSlots[i].transform.GetChild(0).GetComponent<Button>().interactable = true;
				if (PlayerData.instance.playerData.generals.ContainsKey(new List<string>(newPlayerShopGenerals.Keys)[5 * generalScrollIndex + i])) {
					generalPurchaseSlots[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("Manage");
					generalPurchaseSlots[i].transform.GetChild(0).GetComponent<Button>().image.color = new Color(0.27f, 0.76f, 0.52f);

				} else {
					generalPurchaseSlots[i].transform.GetChild(0).GetComponent<Button>().image.color = new Color(0.83f, 0.25f, 0.15f);
					generalPurchaseSlots[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = CustomFunctions.TranslateText("Hire");

				}
			} else {
				generalPurchaseSlots[i].enabled = false;
				generalPurchaseSlots[i].transform.GetChild(0).GetComponent<Button>().interactable = false;
			}


		}
		if (generalScrollIndex == 0)
			leftButton.interactable = false;
		else {
			leftButton.interactable = true;
		}
		if (!CheckScrollAdd())
			rightButton.interactable = false;
		else {
			rightButton.interactable = true;
		}
	}
	public void CountryChanged() {
		generalScrollIndex = 0;
		updateGenerals();
	}
	public void ToShop2() {
		SceneManager.LoadScene(6);
	}
	//the countries original id to load generals with
	private List<string> availableCountries = new List<string>();
	void Start() {
#if !UNITY_ANDROID
		if (!Application.isMobilePlatform) {
			Destroy(iapButton.gameObject);
			techButton.transform.Translate(Vector3.right * 200f * CustomFunctions.getUIScale());
			shopButton.transform.Translate(Vector3.right * 200f * CustomFunctions.getUIScale());
		}
#endif

		countryDropdown.options = new List<Dropdown.OptionData>();
		countryDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText("All Countries")));
		foreach (General g in PlayerData.instance.playerShopGenerals.Values) {
			if (!availableCountries.Contains(g.country) && g.country != "") {
				countryDropdown.options.Add(new Dropdown.OptionData(CustomFunctions.TranslateText(g.country)));
				availableCountries.Add(g.country);
			}
		}
		countryDropdown.RefreshShownValue();
		updateGenerals();
	}
	public void loadScene(int index) {
		SceneManager.LoadScene(index);
	}
	void Update() {
		moneyText.text = PlayerData.instance.playerData.money.ToString();
	}
}
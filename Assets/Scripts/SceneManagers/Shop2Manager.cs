using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using UnityEngine.Purchasing;

public class Shop2Manager : MonoBehaviour {
	public Text moneyText;
	public GameObject purchaseOverlay; //prevent user from clicking anything

	public Button sovietGeneralsButton, germanGeneralsButton, alliedGeneralsButton;
	public Text sovietGeneralsText, germanGeneralsText, alliedGeneralsText;

	void Start() {
		CheckPurchasedGenerals();
	}
	void CheckPurchasedGenerals() {
		List<string> alliedGenerals = new() { "Eisonhower", "Montgomery", "Patton" };
		List<string> germanGenerals = new() { "Manstein", "Rommel", "Guderian" };
		List<string> sovietGenerals = new() { "Zhukov", "Konev", "Rokossovsky" };

		bool ownsAllAlliedGenerals = true;
		foreach (string g in alliedGenerals) {
			if (!PlayerData.instance.playerData.generals.ContainsKey(g)) {
				ownsAllAlliedGenerals = false;
				break;
			}
		}
		bool ownsAllGermanGenerals = true;
		foreach (string g in germanGenerals) {
			if (!PlayerData.instance.playerData.generals.ContainsKey(g)) {
				ownsAllGermanGenerals = false;
				break;
			}
		}
		bool ownsAllSovietGenerals = true;
		foreach (string g in sovietGenerals) {
			if (!PlayerData.instance.playerData.generals.ContainsKey(g)) {
				ownsAllSovietGenerals = false;
				break;
			}
		}
		if (ownsAllAlliedGenerals) {
			alliedGeneralsButton.interactable = false;
			alliedGeneralsText.text = CustomFunctions.TranslateText("Purchased");
		}
		if (ownsAllSovietGenerals) {
			sovietGeneralsButton.interactable = false;
			sovietGeneralsText.text = CustomFunctions.TranslateText("Purchased");
		}
		if (ownsAllGermanGenerals) {
			germanGeneralsButton.interactable = false;
			germanGeneralsText.text = CustomFunctions.TranslateText("Purchased");
		}
	}
	public void loadScene(int index) {
		SceneManager.LoadScene(index);
	}
	public void ToShop1() {
		SceneManager.LoadScene(5);
	}
	public void BeginPurchase() {
		purchaseOverlay.SetActive(true);
	}
	public void PurchaseComplete() {
		StartCoroutine(EnableButtons());
	}
	private IEnumerator EnableButtons() {
		yield return null; yield return null;
		purchaseOverlay.SetActive(false);
	}
	public void PurchaseAlliedGenerals() {
		List<string> newGenerals = new() { "Eisonhower", "Montgomery", "Patton" };
		foreach (string g in newGenerals) {
			if (!PlayerData.instance.playerData.generals.ContainsKey(g)) {
				PlayerData.instance.playerData.generals.Add(g, 0);
			}
		}
		PlayerData.instance.playerData.removedAds = true;
		PlayerData.instance.SortGenerals();
		PlayerData.instance.saveFile();
		CheckPurchasedGenerals();
	}
	public void PurchaseGermanGenerals() {
		List<string> newGenerals = new() { "Manstein", "Rommel", "Guderian" };
		foreach (string g in newGenerals) {
			if (!PlayerData.instance.playerData.generals.ContainsKey(g)) {
				PlayerData.instance.playerData.generals.Add(g, 0);
			}
		}
		PlayerData.instance.playerData.removedAds = true;
		PlayerData.instance.SortGenerals();
		PlayerData.instance.saveFile();
		CheckPurchasedGenerals();
	}
	public void PurchaseSovietGenerals() {
		List<string> newGenerals = new() { "Zhukov", "Konev", "Rokossovsky" };
		foreach (string g in newGenerals) {
			if (!PlayerData.instance.playerData.generals.ContainsKey(g)) {
				PlayerData.instance.playerData.generals.Add(g, 0);
			}
		}
		PlayerData.instance.playerData.removedAds = true;
		PlayerData.instance.SortGenerals();
		PlayerData.instance.saveFile();
		CheckPurchasedGenerals();
	}
	public void Purchase1000Coins() {
		PlayerData.instance.playerData.removedAds = true;
		PlayerData.instance.playerData.money += 1000;
		PlayerData.instance.saveFile();
	}
	public void Purchase5000Coins() {
		PlayerData.instance.playerData.removedAds = true;
		PlayerData.instance.playerData.money += 5000;
		PlayerData.instance.saveFile();
	}
	public void Purchase15000Coins() {
		PlayerData.instance.playerData.removedAds = true;
		PlayerData.instance.playerData.money += 15000;
		PlayerData.instance.saveFile();
	}

	void Update() {
		moneyText.text = PlayerData.instance.playerData.money.ToString();
	}
}

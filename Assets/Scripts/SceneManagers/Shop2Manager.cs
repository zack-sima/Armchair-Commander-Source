using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using UnityEngine.Purchasing;

public class Shop2Manager : MonoBehaviour {
    public Text moneyText;
    public List<Button> disableButtons;
    void Start() {
    }
    public void loadScene(int index) {
        SceneManager.LoadScene(index);
    }
    public void ToShop1() {
        SceneManager.LoadScene(5);
    }
    public void BeginPurchase() {
        foreach (Button b in disableButtons)
            b.interactable = false;
        //IAPButton b1 = new IAPButton();
    }
    public void PurchaseComplete() {
        StartCoroutine(EnableButtons());
    }
    private IEnumerator EnableButtons() {
        yield return null; yield return null;
        foreach (Button b in disableButtons)
            b.interactable = true;
    }
    public void Purchase1000Coins() {
        PlayerData.instance.playerData.money += 1000;
        PlayerData.instance.playerData.removedAds = true;
        //ads disabled after any customer purchase to improve user experience

        PlayerData.instance.saveFile();

    }
    public void Purchase5000Coins() {
        PlayerData.instance.playerData.money += 5000;
        PlayerData.instance.playerData.removedAds = true;

        PlayerData.instance.saveFile();


    }
    public void Purchase15000Coins() {
        PlayerData.instance.playerData.money += 15000;
        PlayerData.instance.playerData.removedAds = true;

        PlayerData.instance.saveFile();


    }

    void Update() {
        moneyText.text = PlayerData.instance.playerData.money.ToString();
    }
}

using UnityEngine;

public class NewGameAdvertisement : MonoBehaviour {
    public GameObject adPrefab;
    public GameObject redDot;
    void Start() {
        if (MyPlayerPrefs.instance.GetInt("seen_new_ad_2") == 0) {
            redDot.SetActive(true);
        }
    }

    public void OpenAd() {
        if (MyPlayerPrefs.instance.GetInt("seen_new_ad_2") == 0) {
            MyPlayerPrefs.instance.SetInt("seen_new_ad_2", 1);
        }
        redDot.SetActive(false);
        adPrefab.SetActive(true);
    }
    public void CloseAd() {
        adPrefab.SetActive(false);
    }
    public void GoToLink() {
        string link = "https://indiewargames.net/";
        Application.OpenURL(link);
    }
}

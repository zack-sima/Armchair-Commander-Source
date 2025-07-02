using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shop3Manager : MonoBehaviour {
	public MapData mapData;
	public GameObject shopButton, iapButton, techButton;
	public Text moneyText;

	public static Shop3Manager instance;

	void Start() {
		instance = this;
#if !UNITY_ANDROID
		if (!Application.isMobilePlatform) {
			Destroy(iapButton);
			techButton.transform.Translate(Vector3.right * 200f * CustomFunctions.getUIScale());
			shopButton.transform.Translate(Vector3.right * 200f * CustomFunctions.getUIScale());
		}
#endif
	}
	public void LoadScene(int index) {
		SceneManager.LoadScene(index);
	}

	void Update() {
		moneyText.text = PlayerData.instance.playerData.money.ToString();
	}
}

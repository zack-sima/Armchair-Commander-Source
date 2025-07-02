using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GeneralCatalog : MonoBehaviour {
	private const string serverURL = "http://main.indiewargames.net:8081/";

	public Image generalImage;
	public Text generalText;

	[HideInInspector]
	public string generalName, generalData; //json

	private byte[] generalImageRaw;

	//delete button is enabled and download disabled if general exists
	public Button downloadButton, deleteButton, reportButton;

	bool generalIsReady = false;
	bool generalReported = false;

	//called when activated (default should be setactive false)
	public void GetGeneralImage() {
		generalImage.sprite = null;
		StartCoroutine(GetGeneralCoroutine());

		//check if general has been downloaded before
		SetDownloaded(PlayerData.instance.playerData.customGenerals.ContainsKey(generalName));

		generalReported = MyPlayerPrefs.instance.GetInt($"reportedGenerals_{generalName}") == 1;
		reportButton.interactable = !generalReported;
	}
	//called for local downloaded data
	public void SetGeneralImage(byte[] image) {
		Texture2D texture = new(1, 1);
		texture.LoadImage(image);
		generalImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

		//check if general has been downloaded before
		SetDownloaded(PlayerData.instance.playerData.customGenerals.ContainsKey(generalName));

		generalReported = MyPlayerPrefs.instance.GetInt($"reportedGenerals_{generalName}") == 1;
		reportButton.interactable = !generalReported;
	}
	private IEnumerator GetGeneralCoroutine() {
		using UnityWebRequest www = UnityWebRequestTexture.GetTexture(
			 serverURL + $"get_general_image?general_name={UnityWebRequest.EscapeURL(generalName)}");
		yield return www.SendWebRequest();

		if (www.result == UnityWebRequest.Result.Success) {
			Texture2D texture = DownloadHandlerTexture.GetContent(www);

			// Set the loaded texture as the sprite for the Image component
			generalImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
			generalImageRaw = texture.EncodeToPNG();

			generalIsReady = true;
		} else {
			Debug.LogError("Image load request failed, trying again; error: " + www.error);

			yield return new WaitForSeconds(2);
			StartCoroutine(GetGeneralCoroutine());
		}
	}
	public void DownloadGeneral() {
		if (!generalIsReady) return;

		//download
		if (!PlayerData.instance.playerData.customGenerals.ContainsKey(generalName)) {
			try {
				print(generalData);
				General g = JsonUtility.FromJson<General>(generalData);
				g.photo = generalImageRaw;
				PlayerData.instance.playerData.customGenerals.Add(generalName, g);
				PlayerData.instance.generalPhotos.Add(g.name, CustomFunctions.LoadSpriteFromBytes(g.photo));
				PlayerData.instance.generals.Add(g.name, g);
				PlayerData.instance.saveFile();
			} catch (System.Exception e) {
				Debug.LogWarning(e);
			}
		}

		SetDownloaded(true);
	}
	public void DeleteDownloadedGeneral() {
		if (PlayerData.instance.playerData.customGenerals.ContainsKey(generalName)) {
			PlayerData.instance.generals.Remove(generalName);
			PlayerData.instance.generalPhotos.Remove(generalName);
			PlayerData.instance.playerData.customGenerals.Remove(generalName);
			PlayerData.instance.saveFile();
		}

		SetDownloaded(false);
	}
	public void ReportGeneral() {
		//implement report & record reported generals
		MyPlayerPrefs.instance.SetInt($"reportedGenerals_{generalName}", 1);
		reportButton.interactable = false;
		StartCoroutine(ReportGeneralCoroutine());
		MyPlayerPrefs.instance.SaveData();
	}
	IEnumerator ReportGeneralCoroutine() {
		string requestURL = $"{serverURL}report_general?general_name={UnityWebRequest.EscapeURL(generalName)}";

		using UnityWebRequest www = UnityWebRequest.Get(requestURL);
		yield return www.SendWebRequest();
	}
	public void SetDownloaded(bool downloaded) {
		if (downloaded) {
			downloadButton.gameObject.SetActive(false);
			deleteButton.gameObject.SetActive(true);
		} else {
			downloadButton.gameObject.SetActive(true);
			deleteButton.gameObject.SetActive(false);
		}
	}
}

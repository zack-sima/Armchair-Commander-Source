using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FlagCatalog : MonoBehaviour {
	private const string serverURL = "http://main.indiewargames.net:8081/";

	public Image flagImage;
	public Text flagText;

	[HideInInspector]
	public string flagName;

	private byte[] flagImageRaw;

	//delete button is enabled and download disabled if flag exists
	public Button downloadButton, deleteButton, reportButton;

	bool flagIsReady = false;
	bool flagReported = false;

	//called when activated (default should be setactive false)
	public void GetFlagImage() {
		flagImage.sprite = null;
		StartCoroutine(GetFlagCoroutine());

		//check if flag has been downloaded before
		SetDownloaded(PlayerData.instance.playerData.flags.ContainsKey(flagName));

		flagReported = MyPlayerPrefs.instance.GetInt($"reportedFlags_{flagName}") == 1;
		reportButton.interactable = !flagReported;
	}
	public void SetFlagImage(byte[] image) {
		Texture2D texture = new(1, 1);
		texture.LoadImage(image);
		flagImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

		//check if flag has been downloaded before
		SetDownloaded(PlayerData.instance.playerData.flags.ContainsKey(flagName));

		flagReported = MyPlayerPrefs.instance.GetInt($"reportedFlags_{flagName}") == 1;
		reportButton.interactable = !flagReported;
	}
	private IEnumerator GetFlagCoroutine() {
		print(serverURL + $"get_flag_image?flag_name={UnityWebRequest.EscapeURL(flagName)}");
		using UnityWebRequest www = UnityWebRequestTexture.GetTexture(
			serverURL + $"get_flag_image?flag_name={UnityWebRequest.EscapeURL(flagName)}");
		yield return www.SendWebRequest();

		if (www.result == UnityWebRequest.Result.Success) {
			Texture2D texture = DownloadHandlerTexture.GetContent(www);

			// Set the loaded texture as the sprite for the Image component
			flagImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
			flagImageRaw = texture.EncodeToJPG();

			flagIsReady = true;
		} else {
			Debug.LogError("Image load request failed, trying again; error: " + www.error);

			yield return new WaitForSeconds(2);
			StartCoroutine(GetFlagCoroutine());
		}
	}
	public void DownloadFlag() {
		if (!flagIsReady) return;

		//download bytes!
		if (!PlayerData.instance.playerData.flags.ContainsKey(flagName)) {
			PlayerData.instance.playerData.flags.Add(flagName, flagImageRaw);
			PlayerData.instance.saveFile();
		}

		SetDownloaded(true);
	}
	public void DeleteDownloadedFlag() {
		if (PlayerData.instance.playerData.flags.ContainsKey(flagName)) {
			PlayerData.instance.playerData.flags.Remove(flagName);
			PlayerData.instance.saveFile();
		}

		SetDownloaded(false);
	}
	public void ReportFlag() {
		//implement report & record reported flags
		MyPlayerPrefs.instance.SetInt($"reportedFlags_{flagName}", 1);
		reportButton.interactable = false;
		StartCoroutine(ReportFlagCoroutine());
		MyPlayerPrefs.instance.SaveData();
	}
	IEnumerator ReportFlagCoroutine() {
		string requestURL = $"{serverURL}report_flag?flag_name={UnityWebRequest.EscapeURL(flagName)}";

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

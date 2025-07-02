using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_STANDALONE
using SimpleFileBrowser;
#endif

public class GeneralEditorPopup : Popup {
	//note: this url is different from the main server url for flags & generals
	private const string serverURL = "http://main.indiewargames.net:8081/";

	//this general is uploaded as a serialized object!
	private General generalForUpload;

	private byte[] generalPhotoData = null;
	private string generalPhotoFileName = "general.png";
	private string generalName = "New General";

	//levels are assigned based on tier
	private int infTier = 0, artyTier = 0, panzerTier = 0, airTier = 0,
		navyTier = 0, speedTier = 0, healthTier = 0;

	private float[] AssignAttackByTier(int tier) {
		int baseValue = tier * 5;
		int increment = tier;
		return new float[] { baseValue, baseValue + increment, baseValue + increment * 2,
			baseValue + increment * 3, baseValue + increment * 4};
	}
	private int[] AssignMovementByTier(int tier) {
		int baseValue = (int)(tier / 2f + 0.5f);
		float increment = tier / 4f;
		return new int[] { baseValue, (int)(baseValue + increment), (int)(baseValue + increment * 2),
			(int)(baseValue + increment * 3), (int)(baseValue + increment * 4)};
	}
	private float[] AssignHealthByTier(int tier) {
		int baseValue = tier * 6;
		int increment = (int)(tier * 1.2f);
		return new float[] { baseValue, baseValue + increment, baseValue + increment * 2,
			baseValue + increment * 3, baseValue + increment * 4};
	}
	public enum TierCategory { Inf, Panzer, Arty, Air, Navy, Mov, Health };

	[SerializeField]
	private GameObject uploadSuccessfulPrefab;
	[SerializeField]
	private InputField generalNameInput;
	[SerializeField]
	private Image uploadPhotoPreview;
	[SerializeField]
	private Text generalExistsWarning;
	[SerializeField]
	private Text infText, artyText, panzerText, airText, navyText, speedText, healthText;
	[SerializeField]
	private Image perk1Image, perk2Image, perk3Image; //when clicking on choose perk popup replace image
	[SerializeField]
	private GameObject choosePerkUI; //enable/disable when clicked

	private bool perkSelectOpen = false;
	private int perkSelectIndex = 1; //1/2/3
	public void OpenSelectPerk(int index) {
		if (perkSelectOpen) return;

		perkSelectIndex = index;
		perkSelectOpen = true;
		choosePerkUI.SetActive(true);
	}
	public void SelectPerk(int perkIndex) {
		perkSelectOpen = false;
		choosePerkUI.SetActive(false);

		if (perkSelectIndex == 1) {
			generalForUpload.perk1 = (General.GeneralPerk)perkIndex;
			perk1Image.sprite = PlayerData.instance.generalPerkSprites[perkIndex];
		} else if (perkSelectIndex == 2) {
			generalForUpload.perk2 = (General.GeneralPerk)perkIndex;
			perk2Image.sprite = PlayerData.instance.generalPerkSprites[perkIndex];
		} else if (perkSelectIndex == 3) {
			generalForUpload.perk3 = (General.GeneralPerk)perkIndex;
			perk3Image.sprite = PlayerData.instance.generalPerkSprites[perkIndex];
		}
	}
	private void Start() {
		generalForUpload = new() {
			hideGeneral = true,
			maxCmdSize = new int[] { 1, 1, 1, 1, 1 },
			cost = new int[] { 9999, 9999, 9999, 9999, 9999 },
			skillBranch = General.GeneralBranch.Infantry //default, change for AI assignment
		};
		InitializeTexts();
	}
	public void IncreaseTier(int category) {
		ChangeTier(true, (TierCategory)category);
	}
	public void DecreaseTier(int category) {
		ChangeTier(false, (TierCategory)category);
	}
	private void InitializeTexts() {
		generalForUpload.infAtk = AssignAttackByTier(infTier);
		generalForUpload.armorAtk = AssignAttackByTier(panzerTier);
		generalForUpload.artilleryAtk = AssignAttackByTier(artyTier);
		generalForUpload.airAtk = AssignAttackByTier(airTier);
		generalForUpload.navyAtk = AssignAttackByTier(navyTier);
		generalForUpload.movement = AssignMovementByTier(speedTier);
		generalForUpload.healthBonus = AssignHealthByTier(healthTier);

		UpdateTierText(infText, infTier);
		UpdateTierText(panzerText, panzerTier);
		UpdateTierText(artyText, infTier);
		UpdateTierText(airText, airTier);
		UpdateTierText(navyText, navyTier);
		UpdateMovementText(speedText, speedTier);
		UpdateHealthText(healthText, healthTier);
	}
	private void ChangeTier(bool isIncrease, TierCategory category) {
		int increaseFactor = isIncrease ? 1 : -1;
		switch (category) {
			case TierCategory.Inf:
				infTier += increaseFactor;
				infTier = Mathf.Clamp(infTier, 0, 10);
				UpdateTierText(infText, infTier);
				generalForUpload.infAtk = AssignAttackByTier(infTier);
				break;
			case TierCategory.Arty:
				artyTier += increaseFactor;
				artyTier = Mathf.Clamp(artyTier, 0, 10);
				UpdateTierText(artyText, artyTier);
				generalForUpload.artilleryAtk = AssignAttackByTier(artyTier);
				break;
			case TierCategory.Panzer:
				panzerTier += increaseFactor;
				panzerTier = Mathf.Clamp(panzerTier, 0, 10);
				UpdateTierText(panzerText, panzerTier);
				generalForUpload.armorAtk = AssignAttackByTier(panzerTier);
				break;
			case TierCategory.Air:
				airTier += increaseFactor;
				airTier = Mathf.Clamp(airTier, 0, 10);
				UpdateTierText(airText, airTier);
				generalForUpload.airAtk = AssignAttackByTier(airTier);
				break;
			case TierCategory.Navy:
				navyTier += increaseFactor;
				navyTier = Mathf.Clamp(navyTier, 0, 10);
				UpdateTierText(navyText, navyTier);
				generalForUpload.navyAtk = AssignAttackByTier(navyTier);
				break;
			case TierCategory.Mov:
				speedTier += increaseFactor;
				speedTier = Mathf.Clamp(speedTier, 0, 10);
				UpdateMovementText(speedText, speedTier);
				generalForUpload.movement = AssignMovementByTier(speedTier);
				break;
			case TierCategory.Health:
				healthTier += increaseFactor;
				healthTier = Mathf.Clamp(healthTier, 0, 10);
				UpdateHealthText(healthText, healthTier);
				generalForUpload.healthBonus = AssignHealthByTier(healthTier);
				break;
		}
	}
	private void UpdateMovementText(Text text, int tierLevel) {
		int[] values = AssignMovementByTier(tierLevel);
		text.text = $"{values[0]} (+{(values[4] - values[0]) / 4f:F2}/Lv)";
	}
	private void UpdateHealthText(Text text, int tierLevel) {
		float[] values = AssignHealthByTier(tierLevel);
		text.text = $"{values[0] + 100}% (+{(int)(values[1] - values[0])}%/Lv)";
	}
	private void UpdateTierText(Text text, int tierLevel) {
		float[] values = AssignAttackByTier(tierLevel);
		text.text = $"{values[0] + 100}% (+{(int)(values[1] - values[0])}%/Lv)";
	}
	public void SelectGeneral() {
		StartCoroutine(SelectGeneralCoroutine());
	}
	public void UploadGeneral() {
		if (generalPhotoData != null && PlayerData.instance.playerData.money >= 80) {
			StartCoroutine(UploadGeneralCoroutine());
		}
	}
	//from https://stackoverflow.com/questions/56949217/how-to-resize-a-texture2d-using-height-and-width
	Texture2D Resize(Texture2D texture2D, int targetX, int targetY) {
		RenderTexture rt = new(targetX, targetY, 24);
		RenderTexture.active = rt;
		Graphics.Blit(texture2D, rt);
		Texture2D result = new(targetX, targetY);
		result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
		result.Apply();
		return result;
	}
	private byte[] ResizeImage(byte[] imageBytes) {
		// Load the original image from imageBytes
		Texture2D originalTexture = new(1, 1);
		originalTexture.LoadImage(imageBytes);

		Texture2D resizedTexture = Resize(originalTexture, 100, 128);

		// Encode the resized texture to a new byte array
		byte[] resizedBytes = resizedTexture.EncodeToPNG();

		// Clean up resources
		Destroy(originalTexture);
		Destroy(resizedTexture);

		return resizedBytes;
	}

	private IEnumerator SelectGeneralCoroutine() {
#if UNITY_EDITOR
		string imagePath = EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg");
		if (!string.IsNullOrEmpty(imagePath)) {
			PickedImage(imagePath);
		}
#elif UNITY_IOS || UNITY_ANDROID
        yield return NativeGallery.RequestPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);

        yield return NativeGallery.GetImageFromGallery((path) => {
            PickedImage(path);
        }, "Select an image");
#else
		//standalone platform
		FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"));
		yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders,
			false, null, null, "Load Files and Folders", "Load");

		if (FileBrowser.Success) {
			Debug.Log($"path: {FileBrowser.Result[0]}");
			PickedImage(FileBrowser.Result[0]);
		}
#endif
		yield return null;
	}
	private void PickedImage(string imagePath) {
		byte[] imageBytes = File.ReadAllBytes(imagePath);
		imageBytes = ResizeImage(imageBytes);
		generalPhotoFileName = Path.GetFileName(imagePath);
		generalPhotoData = imageBytes;

		Texture2D texture = new(1, 1);
		texture.LoadImage(imageBytes);
		uploadPhotoPreview.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
	}

	bool uploading = false;
	private IEnumerator UploadGeneralCoroutine() {
		if (uploading) yield break;

		generalExistsWarning.enabled = false;
		uploading = true;

		if (generalNameInput.text != "") {
			generalName = generalNameInput.text;
			generalForUpload.name = generalName;
		} else {
			uploading = false;
			yield break;
		}

		bool generalExists = false;
		foreach (string s in PlayerData.instance.generals.Keys) {
			if (s == generalName) {
				generalExists = true;
				break;
			}
		}

		//check if flag_name already exists in database
		string requestURL = $"{serverURL}general_exists?general_name={UnityWebRequest.EscapeURL(generalName)}";

		using UnityWebRequest www = UnityWebRequest.Get(requestURL);
		yield return www.SendWebRequest();

		if (www.downloadHandler.text != "0" || generalExists) {
			//flag exists/request error, cancel upload

			Debug.LogWarning("flag exists!");

			//show error on UI
			generalExistsWarning.enabled = true;
			uploading = false;

			yield break;
		}

		//upload general
		WWWForm form = new();
		form.AddField("general_name", generalName);
		form.AddBinaryData("general_image", generalPhotoData, generalPhotoFileName, "image/*");
		form.AddField("general_data", JsonUtility.ToJson(generalForUpload));

		//using keyword supposedly cleans up garbage in case of exceptions
		using UnityWebRequest www2 = UnityWebRequest.Post(serverURL + "upload_general", form);
		yield return www2.SendWebRequest();

		if (www2.result == UnityWebRequest.Result.Success) {
			Transform insItem = Instantiate(uploadSuccessfulPrefab, GameObject.Find("Canvas").transform).transform;
			insItem.position = new Vector2(Screen.width / 2, Screen.height / 2);
			PlayerData.instance.playerData.money -= 80;
			PlayerData.instance.saveFile();

			Debug.Log("General uploaded successfully!");
		} else {
			Debug.LogError("General upload failed: " + www2.error);
		}
		uploading = false;
	}
}

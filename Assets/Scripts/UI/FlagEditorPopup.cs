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

//Partially ChatGPT-generated code!
public class FlagEditorPopup : Popup {
	//note: this url is different from the main server url for flags & generals
	private const string serverURL = "http://main.indiewargames.net:8081/";

	private byte[] flagData = null;
	private string flagName = "New flag";
	private string fileName = "flag.jpg";

	[SerializeField]
	private GameObject uploadSuccessfulPrefab;
	[SerializeField]
	private InputField flagNameInput;
	[SerializeField]
	private Image uploadFlagPreview;
	[SerializeField]
	private Text mapExistsWarning;

	public void SelectFlag() {
		StartCoroutine(SelectFlagCoroutine());
	}
	public void UploadFlag() {
		if (flagData != null && PlayerData.instance.playerData.money >= 20) {
			StartCoroutine(UploadFlagCoroutine());
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
	private byte[] ResizeImageTo128x128(byte[] imageBytes) {
		// Load the original image from imageBytes
		Texture2D originalTexture = new(1, 1);
		originalTexture.LoadImage(imageBytes);

		Texture2D resizedTexture = Resize(originalTexture, 128, 128);

		// Encode the resized texture to a new byte array
		byte[] resizedBytes = resizedTexture.EncodeToJPG();

		// Clean up resources
		Destroy(originalTexture);
		Destroy(resizedTexture);

		return resizedBytes;
	}

	private IEnumerator SelectFlagCoroutine() {
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
		//standalone platform (https://docs.unity3d.com/Manual/PlatformDependentCompilation.html)
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
		print($"image path obtained: {imagePath}");
		byte[] imageBytes = File.ReadAllBytes(imagePath);
		imageBytes = ResizeImageTo128x128(imageBytes);
		fileName = Path.GetFileName(imagePath);
		flagData = imageBytes;

		Texture2D texture = new(1, 1);
		texture.LoadImage(imageBytes);
		uploadFlagPreview.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
	}

	bool uploading = false;
	private IEnumerator UploadFlagCoroutine() {
		if (uploading) yield break;

		mapExistsWarning.enabled = false;
		uploading = true;

		if (flagNameInput.text != "") {
			flagName = flagNameInput.text;
		} else {
			uploading = false;
			yield break;
		}

		bool flagExists = false;
		foreach (Sprite s in MenuManager.instance.customFlags) {
			if (s.name == flagName) {
				flagExists = true;
				break;
			}
		}

		//check if flag_name already exists in database
		string requestURL = $"{serverURL}flag_exists?flag_name={UnityWebRequest.EscapeURL(flagName)}";

		using UnityWebRequest www = UnityWebRequest.Get(requestURL);
		yield return www.SendWebRequest();

		if (www.downloadHandler.text != "0" || flagExists) {
			//flag exists/request error, cancel upload

			Debug.LogWarning("flag exists!");

			//show error on UI
			mapExistsWarning.enabled = true;
			uploading = false;

			yield break;
		}

		//upload flag
		WWWForm form = new();
		form.AddField("flag_name", flagName); // Replace with actual flag name
		form.AddBinaryData("flag_image", flagData, fileName, "image/*"); // Adjust MIME type accordingly

		//using keyword supposedly cleans up garbage in case of exceptions
		using UnityWebRequest www2 = UnityWebRequest.Post(serverURL + "upload_flag", form);
		yield return www2.SendWebRequest();

		if (www2.result == UnityWebRequest.Result.Success) {
			Transform insItem = Instantiate(uploadSuccessfulPrefab, GameObject.Find("Canvas").transform).transform;
			insItem.position = new Vector2(Screen.width / 2, Screen.height / 2);
			PlayerData.instance.playerData.money -= 20;
			PlayerData.instance.saveFile();

			Debug.Log("Flag uploaded successfully!");
		} else {
			Debug.LogError("Flag upload failed: " + www2.error);
		}
		uploading = false;
	}
}

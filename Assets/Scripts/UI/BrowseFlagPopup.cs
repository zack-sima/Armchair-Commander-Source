using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BrowseFlagPopup : Popup {
	private const string serverURL = "http://main.indiewargames.net:8081/";

	public List<FlagCatalog> flagCatalogs;
	public InputField searchNameInput;
	public Text pageText;

	private int pageIndex = 0, lastSearchQueries = 0; //if less than 6 don't allow next page
	private string searchName = "";

	private CurrentBrowseMode browseMode = CurrentBrowseMode.AllFlags;
	private enum CurrentBrowseMode { AllFlags, DownloadedFlags, NamedFlags };

	public void TurnPage(bool nextPage) {
		if (nextPage) {
			if (lastSearchQueries > 5) {
				pageIndex++;
			} else return;
		} else {
			if (pageIndex > 0) {
				pageIndex--;
			} else return;
		}
		UpdatePageText();
		if (browseMode != CurrentBrowseMode.DownloadedFlags) {
			StartCoroutine(RetrieveFlags());
		} else {
			BrowseDownloadedFlags();
		}
	}
	private void UpdatePageText() {
		pageText.text = (pageIndex + 1).ToString();
	}
	public void FirstSearchFlag() {
		pageIndex = 0;
		UpdatePageText();
		searchName = searchNameInput.text;
		browseMode = CurrentBrowseMode.NamedFlags;
		StartCoroutine(RetrieveFlags());
	}
	public void FirstBrowseFlag() {
		pageIndex = 0;
		UpdatePageText();
		browseMode = CurrentBrowseMode.AllFlags;
		StartCoroutine(RetrieveFlags());
	}
	public void FirstDownloadedFlags() {
		pageIndex = 0;
		UpdatePageText();
		browseMode = CurrentBrowseMode.DownloadedFlags;
		BrowseDownloadedFlags();
	}

	private void BrowseDownloadedFlags() {
		//locally load flags!
		List<string> keys = new(PlayerData.instance.playerData.flags.Keys);
		List<byte[]> values = new(PlayerData.instance.playerData.flags.Values);

		List<string> flagNameList = new();
		List<byte[]> flagImagesList = new();
		for (int i = pageIndex * 5; i < keys.Count; i++) {
			if (i > pageIndex * 5 + 5) break;
			flagNameList.Add(keys[i]);
			flagImagesList.Add(values[i]);
		}

		lastSearchQueries = flagNameList.Count;

		int index = 0;
		for (; index < 5; index++) {
			if (index >= flagNameList.Count) break;

			//enable catalog
			flagCatalogs[index].gameObject.SetActive(true);
			flagCatalogs[index].flagText.text = flagNameList[index];
			flagCatalogs[index].flagName = flagNameList[index];
			flagCatalogs[index].SetFlagImage(flagImagesList[index]);

		}
		for (int i = index; i < 5; i++) {
			flagCatalogs[i].gameObject.SetActive(false);
		}
	}
	IEnumerator RetrieveFlags() {
		string requestUrl;
		if (browseMode == CurrentBrowseMode.AllFlags) {
			requestUrl = $"{serverURL}get_flags?item_index={pageIndex * 5}&limit=6";
		} else {
			requestUrl = $"{serverURL}get_flags_search?item_index={pageIndex * 5}&limit=6&search={searchName}";
		}

		UnityWebRequest request = UnityWebRequest.Get(requestUrl);

		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success) {
			Debug.LogError("Error downloading flags: " + request.error);
			yield break;
		}

		string jsonResponse = request.downloadHandler.text;
		List<FlagData> flagDataList = JsonUtility.FromJson<FlagDataList>(jsonResponse).flags;

		lastSearchQueries = flagDataList.Count;

		int index = 0;
		foreach (FlagData flagData in flagDataList) {
			if (index >= flagCatalogs.Count) break;

			Debug.Log("Flag Name: " + flagData.flag_name);
			Debug.Log("UID: " + flagData.uid); //redundant

			//enable catalog
			flagCatalogs[index].gameObject.SetActive(true);
			flagCatalogs[index].flagText.text = flagData.flag_name;
			flagCatalogs[index].flagName = flagData.flag_name;
			flagCatalogs[index].GetFlagImage();

			index++;
		}
		//if not filled disable catalogs
		for (int i = index; i < flagCatalogs.Count; i++) {
			flagCatalogs[i].gameObject.SetActive(false);
		}
	}

	void Start() {
		UpdatePageText();
		StartCoroutine(RetrieveFlags());
	}

	[System.Serializable]
	public class FlagData {
		public string flag_name;
		public int uid;
	}

	[System.Serializable]
	public class FlagDataList {
		public List<FlagData> flags;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BrowseGeneralPopup : Popup {
	private const string serverURL = "http://main.indiewargames.net:8081/";

	public List<GeneralCatalog> generalCatalogs;
	public InputField searchNameInput;
	public Text pageText;

	private int pageIndex = 0, lastSearchQueries = 0; //if less than 6 don't allow next page
	private string searchName = "";

	private CurrentBrowseMode browseMode = CurrentBrowseMode.AllGenerals;
	private enum CurrentBrowseMode { AllGenerals, DownloadedGenerals, NamedGenerals };

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
		if (browseMode != CurrentBrowseMode.DownloadedGenerals) {
			StartCoroutine(RetrieveGenerals());
		} else {
			BrowseDownloadedGenerals();
		}
	}
	private void UpdatePageText() {
		pageText.text = (pageIndex + 1).ToString();
	}
	public void FirstSearchGeneral() {
		pageIndex = 0;
		UpdatePageText();
		searchName = searchNameInput.text;
		browseMode = CurrentBrowseMode.NamedGenerals;
		StartCoroutine(RetrieveGenerals());
	}
	public void FirstBrowseGeneral() {
		pageIndex = 0;
		UpdatePageText();
		browseMode = CurrentBrowseMode.AllGenerals;
		StartCoroutine(RetrieveGenerals());
	}
	public void FirstDownloadedGenerals() {
		pageIndex = 0;
		UpdatePageText();
		browseMode = CurrentBrowseMode.DownloadedGenerals;
		BrowseDownloadedGenerals();
	}

	private void BrowseDownloadedGenerals() {
		//locally load generals!
		List<string> keys = new(PlayerData.instance.playerData.customGenerals.Keys);
		List<General> values = new(PlayerData.instance.playerData.customGenerals.Values);

		List<string> generalNameList = new();
		List<General> generalDataList = new();
		for (int i = pageIndex * 5; i < keys.Count; i++) {
			if (i > pageIndex * 5 + 5) break;
			generalNameList.Add(keys[i]);
			generalDataList.Add(values[i]);
		}

		lastSearchQueries = generalNameList.Count;

		int index = 0;
		for (; index < 5; index++) {
			if (index >= generalNameList.Count) break;

			//enable catalog
			generalCatalogs[index].gameObject.SetActive(true);
			generalCatalogs[index].generalText.text = generalNameList[index];
			generalCatalogs[index].generalName = generalNameList[index];
			generalCatalogs[index].generalData = JsonUtility.ToJson(generalDataList[index]);
			generalCatalogs[index].SetGeneralImage(generalDataList[index].photo);
		}
		for (int i = index; i < 5; i++) {
			generalCatalogs[i].gameObject.SetActive(false);
		}
	}
	IEnumerator RetrieveGenerals() {
		string requestUrl;
		if (browseMode == CurrentBrowseMode.AllGenerals) {
			requestUrl = $"{serverURL}get_generals?item_index={pageIndex * 5}&limit=6";
		} else {
			requestUrl = $"{serverURL}get_generals_search?item_index={pageIndex * 5}&limit=6&search={searchName}";
		}

		UnityWebRequest request = UnityWebRequest.Get(requestUrl);

		yield return request.SendWebRequest();

		if (request.result != UnityWebRequest.Result.Success) {
			Debug.LogError("Error downloading generals: " + request.error);
			yield break;
		}

		string jsonResponse = request.downloadHandler.text;

		List<GeneralData> generalDataList = JsonUtility.FromJson<GeneralDataList>(jsonResponse).generals;

		lastSearchQueries = generalDataList.Count;

		int index = 0;
		foreach (GeneralData generalData in generalDataList) {
			if (index >= generalCatalogs.Count) break;

			//Debug.Log("General Name: " + generalData.general_name);
			//Debug.Log("UID: " + generalData.uid);
			//Debug.Log("Data: " + generalData.general_json);

			//enable catalog
			generalCatalogs[index].gameObject.SetActive(true);
			generalCatalogs[index].generalText.text = generalData.general_name;
			generalCatalogs[index].generalName = generalData.general_name;
			generalCatalogs[index].generalData = generalData.general_json;
			generalCatalogs[index].GetGeneralImage();

			index++;
		}
		//if not filled disable catalogs
		for (int i = index; i < generalCatalogs.Count; i++) {
			generalCatalogs[i].gameObject.SetActive(false);
		}
	}

	void Start() {
		UpdatePageText();
		StartCoroutine(RetrieveGenerals());
	}

	[System.Serializable]
	public class GeneralData {
		public string general_name;
		public string general_json;
		public int uid;
	}

	[System.Serializable]
	public class GeneralDataList {
		public List<GeneralData> generals;
	}
}

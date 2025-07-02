using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagsAndGeneralsPopup : Popup {
	[HideInInspector]
	public MenuManager manager;

	public void BrowseFlags() {
		manager.GoToFlagBrowser();
		Destroy(gameObject);
	}
	public void UploadFlags() {
		manager.GoToFlagEditor();
		Destroy(gameObject);
	}
	public void BrowseGenerals() {
		manager.GoToGeneralsBrowser();
		Destroy(gameObject);
	}
	public void UploadGenerals() {
		manager.GoToGeneralsEditor();
		Destroy(gameObject);
	}
}

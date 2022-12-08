using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.IO;

public class ShareButton : MonoBehaviour {
	public Texture2D[] chineseSharePhotos, englishSharePhotos;
	private string shareMessage;
	private IEnumerator TakeScreenshotAndShare() {
		yield return new WaitForEndOfFrame();

		Texture2D ss = null;
		if (MyPlayerPrefs.instance.GetString("language") == "Chinese") {
			ss = chineseSharePhotos[Random.Range(0, chineseSharePhotos.Length)];
		} else {
			ss = englishSharePhotos[Random.Range(0, englishSharePhotos.Length)];
		}

		string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
		File.WriteAllBytes(filePath, ss.EncodeToPNG());

		// To avoid memory leaks
		Destroy(ss);
		new NativeShare().AddFile(filePath)
			.SetSubject(CustomFunctions.TranslateText("Armchair Commander")).SetText(shareMessage)
			.SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
			.Share();

	}
	public void ClickShareButton() {
		shareMessage = MyPlayerPrefs.instance.GetString("language") == "Chinese" ?
			"自己可以做地图？可以玩军阀混战？还不快来体验一下！\n苹果下载：https://apps.apple.com/us/app/id1454515001" :
			"What's that? You can create your own maps and reexperience history? Download now!\niOS: https://apps.apple.com/us/app/id1454515001";
		StartCoroutine(TakeScreenshotAndShare());
	}
} //rc https://apps.apple.com/us/app/id1368995698
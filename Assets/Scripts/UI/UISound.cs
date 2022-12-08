using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISound : MonoBehaviour {
    bool firstFrame = true;
    void Start() {
        if (GetComponent<Button>() != null)
            GetComponent<Button>().onClick.AddListener(PlaySound);
        if (GetComponent<Dropdown>() != null) {
            GetComponent<Dropdown>().onValueChanged.AddListener(delegate { PlaySound(); });
        }
        if (GetComponent<Toggle>() != null) {
            GetComponent<Toggle>().onValueChanged.AddListener(delegate { PlaySound(); });
        }
        StartCoroutine(LateStart());
    }
    IEnumerator LateStart() {
        yield return null;
        firstFrame = false;
    }
    void PlaySound() {
        float vol = Mathf.Max(MyPlayerPrefs.instance.GetFloat("sounds") * 0.5f, 0.1f);
        if (!firstFrame && MyPlayerPrefs.instance.GetInt("mutedUI") == 0)
            MyPlayerPrefs.instance.buttonSound.PlayOneShot(MyPlayerPrefs.instance.buttonSound.clip, vol * MyPlayerPrefs.instance.buttonSound.volume);
    }
}
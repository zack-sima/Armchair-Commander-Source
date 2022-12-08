using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DropdownTranslator : MonoBehaviour {
    [HideInInspector]
    public string[] englishTranslation;
    public bool useAwake;

    void Awake() {
        if (useAwake) MyStart();
    }
    void Start() {
        if (!useAwake) MyStart();
    }

    void MyStart() {
        int index = 0;
        englishTranslation = new string[GetComponent<Dropdown>().options.Count];
        foreach (Dropdown.OptionData i in GetComponent<Dropdown>().options) {
            englishTranslation[index] = i.text;
            i.text = CustomFunctions.TranslateText(englishTranslation[index]);
            index++;
        }
        GetComponent<Dropdown>().captionText.text = GetComponent<Dropdown>().options[0].text;
    }
}

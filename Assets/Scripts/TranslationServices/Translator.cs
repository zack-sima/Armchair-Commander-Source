using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Translator : MonoBehaviour {
    [HideInInspector] //deprecated
    public string chineseTranslation;
    string englishTranslation;
    public bool constantUpdate, thickChinese;


    void Start() {
        englishTranslation = GetComponent<Text>().text;

        if (!constantUpdate) {
            GetComponent<Text>().text = CustomFunctions.TranslateText(englishTranslation);
        }
    }
    void Update() {
        if (constantUpdate) {
            GetComponent<Text>().text = CustomFunctions.TranslateText(englishTranslation);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

//to access translations, simply use LocalizationSystem.GetLocalizedValue(key)
public class LocalizationSystem
{
    //anything that doesn't need to be translated from code english to written english is left blank
    private static Dictionary<string, string> localizedEN, localizedCN, localizedES, localizedRU, localizedFR, localizedJP;
    public static bool isInit;
    public static void Init() {
        localizedEN = new Dictionary<string, string>();
        localizedES = new Dictionary<string, string>();
        localizedRU = new Dictionary<string, string>();
        localizedFR = new Dictionary<string, string>();
        localizedJP = new Dictionary<string, string>();
        localizedCN = new Dictionary<string, string>();

        for (int i = 0; i < 1; i++) {
            CSVLoader csvLoader = new CSVLoader();
            switch (i) {
            case 0:
                csvLoader.LoadCSV("Translate/Translations");
                break;
            //case 1:
            //    csvLoader.LoadCSV("Translate/MissionTranslations");
            //    break;
            //case 2:
            //    csvLoader.LoadCSV("Translate/GeneralsTranslations");
            //    break;
            //case 3:
            //    csvLoader.LoadCSV("Translate/CitiesTranslations");
            //    break;
            //case 4:
            //    csvLoader.LoadCSV("Translate/CountriesTranslations");
            //    break;
            //case 5:
            //    //this old csv has all the previous translations so if any are accidentally cut off it is supplied here
            //    csvLoader.LoadCSV("Translate/ZDeprecatedTranslations");
            //    break;
            }

            localizedEN = localizedEN.Concat(csvLoader.GetDictionaryValuies("English").Where(x => !localizedEN.Keys.Contains(x.Key))).ToDictionary(x => x.Key, x => x.Value);
            localizedCN = localizedCN.Concat(csvLoader.GetDictionaryValuies("Chinese").Where(x => !localizedCN.Keys.Contains(x.Key))).ToDictionary(x => x.Key, x => x.Value);
            localizedES = localizedES.Concat(csvLoader.GetDictionaryValuies("Spanish").Where(x => !localizedES.Keys.Contains(x.Key))).ToDictionary(x => x.Key, x => x.Value);
            localizedRU = localizedRU.Concat(csvLoader.GetDictionaryValuies("Russian").Where(x => !localizedRU.Keys.Contains(x.Key))).ToDictionary(x => x.Key, x => x.Value);
            localizedFR = localizedFR.Concat(csvLoader.GetDictionaryValuies("French").Where(x => !localizedFR.Keys.Contains(x.Key))).ToDictionary(x => x.Key, x => x.Value);
            localizedJP = localizedJP.Concat(csvLoader.GetDictionaryValuies("Japanese").Where(x => !localizedJP.Keys.Contains(x.Key))).ToDictionary(x => x.Key, x => x.Value);
        }

        isInit = true;
    }
    public static string GetLocalizedValue(string key) {
        if (!isInit) {
            Init();
        }
        string value;
        switch (MyPlayerPrefs.instance.GetString("language")) {
        case "Chinese":
            localizedCN.TryGetValue(key, out value);
            break;
        case "Spanish":
            localizedES.TryGetValue(key, out value);
            break;
        case "Russian":
            localizedRU.TryGetValue(key, out value);
            break;
        case "French":
            localizedFR.TryGetValue(key, out value);
            break;
        case "Japanese":
            localizedJP.TryGetValue(key, out value);
            break;
        default: //english
            localizedEN.TryGetValue(key, out value);
            break;
        }
        if (value == "" || value == null)
            value = key;
        return value;
    } 
}

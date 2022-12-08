using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Text;

public static class CustomFunctions {
    public const bool ChineseStore = false;
    public static Rect GeneratePointDetectionRect(Vector2 position, Rect rect) {
        return new Rect(new Vector2(position.x - rect.width * getUIScale() / 2, position.y - rect.height * getUIScale() / 2), new Vector2(rect.width * getUIScale(), rect.height * getUIScale()));
    }
    public static string getURL() {
        //use your own ip! (don't flood my server with modded maps)
        return "http://www.example.com:9999/";
    }
    public static List<string> CountriesIsNeutral = new List<string>() {
        "Portugal",
        "NeutralSpain",
        "Switzerland",
        "Sweden",
        "Ireland",
        "Lithuania",
        "Latvia",
        "Estonia",
        "Iran",
        "Iraq",
        "Turkey",
        "NeutralSoviet",
        "NeutralUK",
        "NeutralFrance",
        "ChineseTibet",
        "Mongolia",
        "NeutralThailand",
    };


    //TODO: BOOL VALUES HAVE BEEN CHANGED
    public static Dictionary<string, int> CountriesIsAxis = new Dictionary<string, int>() {
        {"Soviet", 0},
        {"Soviet2", 0},
        {"NeutralSoviet", 0},
        {"German", 1},
        {"German2", 1},
        {"WestGermany", 1},
        {"EastGermany", 0},
        {"Austria", 1},
        {"Czechoslovakia", 1},
        {"Slovakia", 1},

        {"Poland", 0},
        {"CommiePoland", 0},
        {"HomeArmyPoland", 0},
        {"RepublicanSpain", 0},
        {"FascistSpain", 1},
        {"Bulgaria", 1},
        {"Romania", 1},
        {"Hungary", 1},
        {"Yugoslavia", 0},
        {"Serbia", 1},
        {"Croatia", 1},
        {"Albania", 0},
        {"Turkey", 0},
        {"NatoTurkey", 1},


        {"Greece", 0},
        {"CommieGreece", 0},

        {"Italy", 1},

        {"France", 0},
        {"NatoFrance", 1},
        {"NeutralFrance", 0},

        {"UK", 0},
        {"NeutralUK", 0},
        {"NatoUK", 1},
        {"Belgium", 0},
        {"Netherlands", 0},
        {"Luxembourg", 0},
        {"Finland", 1},
        {"Norway", 0},
        {"Sweden", 1},
        {"Denmark", 0},

        {"Portugal", 1},
        {"NeutralSpain", 1},
        {"Switzerland", 1},

        {"Ireland", 0},
        {"Lithuania", 0},
        {"Latvia", 0},
        {"Estonia", 0},

        {"Canada", 0},
        {"USA", 0},
        {"NatoUSA", 1},

        {"Cuba", 0},
        {"Mexico", 0},
        {"Brazil", 0},
        {"Colombia", 0},
        {"Chile", 1},

        {"Argentina", 1},

        

        {"ROC", 0},
        {"NatoROC", 1},
        {"PRC", 0},
        {"PRCNew", 0},
        {"PuppetROC", 1},
        {"Japan", 1},
        {"Japan2", 1},
        {"Manchukuo", 1},
        {"Mengkukuo", 1},
        {"ChineseGuangxi", 0},
        {"ChineseShanxi", 0},
        {"ChineseSinkiang", 0},
        {"ChineseTibet", 0},
        {"ChineseXibeiSanma", 0},
        {"ChineseYunnan", 0},
        {"ChineseSichuan", 0},

        {"Mongolia", 0},
        {"Thailand", 1},
        {"NeutralThailand", 1},


        {"NorthKorea", 0},
        {"SouthKorea", 1},
        {"SaudiArabia", 1},
        {"India", 0},
        {"NatoIndia", 1},
        {"Pakistan", 0},
        {"Australia", 0},

        {"NewZealand", 0},
        {"Phillipeans", 0},

        {"Vietnam", 0},
        {"SouthVietnam", 1},
         {"Indonesia", 1}, //NATO
        {"Malaysia", 1}, //NATO
        {"Singapore", 1}, //NATO

        
        {"Iran", 0},
        {"Iraq", 0},
        {"CommieIraq", 0},
        {"Israel", 1},
        {"Afghanistan", 0},
        
        {"Egypt", 0},
        {"Ethiopia", 0},
        {"SouthAfrica", 0},
        {"Liberia", 0},




    };
    public static Skin DetermineTroopSkin(string country, int skinIsAxis) {
        List<string> japanSkinCountries = new List<string>() { "Japan", "Japan2", "Thailand", "NeutralThailand", "PuppetROC", "Manchukuo", "Mengkukuo" };
        List<string> americanSkinCountries = new List<string>() {
          "Chile", "Colombia", "Argentina"  , "SouthVietnam"  , "Brazil"  , "NatoTurkey"  , "SaudiArabia"  , "Turkey"  , "NatoIndia"  , "India"  , "ROC"  , "USA"  , "NatoUSA"  , "NatoROC" , "Israel"
            , "Liberia"  , "SouthKorea"  , "ChineseGuangxi"  , "ChineseShanxi"
            , "ChineseTibet"  , "SouthAfrica", "ChineseXibeiSanma", "ChineseSichuan"  , "ChineseYunnan"  , "Iraq" , "Greece"  , "Phillipeans"
        };
        List<string> frenchSkinCountries = new List<string>() {
            "France", "NatoFrance", "NeutralFrance", "Luxembourg", "Belgium", "Netherlands"
        };
        List<string> britishSkinCountries = new List<string>() {
            "UK", "NatoUK", "NeutralUK", "Indonesia", "Malaysia", "Singapore", "Australia", "NewZealand", "Canada", "India", "Ireland", "Norway", "Denmark", "HomeArmyPoland"
        };
        if (japanSkinCountries.Contains(country)) {
            return Skin.Japanese;
        } else if (americanSkinCountries.Contains(country)) {
            return Skin.American;
        } else if (frenchSkinCountries.Contains(country)) {
            return Skin.French;
        } else if (britishSkinCountries.Contains(country)) {
            return Skin.British;
        } else if (skinIsAxis == 1) {
            return Skin.German;
        } else {
            return Skin.Soviet;
        }
    }
    public static string TranslateText(string s) {
        if (s == "")
            return "";

        return LocalizationSystem.GetLocalizedValue(s);
    }
    //****NEEDS TO STAY FOR MAP/PLAYER DATA COMPATIBILITY
    public static Dictionary<string, string> GeneralUpdatedNames = new Dictionary<string, string>() {
            { "Fedor Bock", "Bock" },
            { "Mao", "Mao Zedong" },
            { "Li Pingxian", "Li Pinxian"},
            { "Chen S.K.", "Chen Shaokuan"},
        { "Sun Li-jen", "Sun Liren"}


    };
    //****NEEDS TO STAY FOR MAP UNLOCK
    public static Dictionary<string, string> MissionUpdatedNames = new Dictionary<string, string>() {
            { "Stalingrad Breakout", "Operation Winter Storm" }


    };
    public static bool getIsMobile() {
        //change this based on device
        if (SystemInfo.deviceType == DeviceType.Handheld)
            return true;
        else
            return false; 
    }
    public static void CopyToClipboard(string s) {
    TextEditor te = new TextEditor();
    
        te.text = s;
    te.SelectAll();
    te.Copy();
    }
    public static string ConvertToUtf8(string str) {
    UTF8Encoding encodes = new UTF8Encoding();
    return encodes.GetString(encodes.GetBytes(str));
    }
    public static string PasteFromClipboard() {
    TextEditor te = new TextEditor();
    te.Paste();
        return te.text;
    }
    public static float getUIScale(bool isByWidth = true) {
        if (isByWidth)
            return Screen.width / 1600f;
        else
            return Screen.height / 1200f;
    }
}

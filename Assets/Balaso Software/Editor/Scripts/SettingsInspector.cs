using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Balaso
{
    [CustomEditor(typeof(Balaso.Settings))]
    public class SettingsInspector : Editor
    {
        private static string SETTINGS_ASSET_PATH = "Assets/Balaso Software/Editor/Settings.asset";

        private int languageKeySelectedIndex = (int) SystemLanguage.English;
        private Vector2 localizedPopupMessageScrollPosition;
        
        private static Settings settings;
        public static Settings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = (Settings)AssetDatabase.LoadAssetAtPath(SETTINGS_ASSET_PATH, typeof(Balaso.Settings));
                    if (settings == null)
                    {
                        settings = CreateDefaultSettings();
                    }
                }

                return settings;
            }
        }

        private static Settings CreateDefaultSettings()
        {
            Settings asset = ScriptableObject.CreateInstance(typeof(Balaso.Settings)) as Settings;
            AssetDatabase.CreateAsset(asset, SETTINGS_ASSET_PATH);
            CreateLocalizationDictionary();
            return asset;
        }

        private static void CreateLocalizationDictionary()
        {
            if (settings != null &&
                (settings.LocalizedPopupMessageDictionary == null || settings.LocalizedPopupMessageDictionary.Count == 0))
            {
                settings.UseLocalizationValues = true;
                settings.LocalizedPopupMessageDictionary = new LanguagesDictionary();
                settings.LocalizedPopupMessageDictionary.Add((int)SystemLanguage.English, "Pressing 'Allow' uses device info for more relevant ad content");
                settings.LocalizedPopupMessageDictionary.Add((int)SystemLanguage.French, "'Autoriser' permet d'utiliser les infos du téléphone pour afficher des contenus publicitaires plus pertinents");
                settings.LocalizedPopupMessageDictionary.Add((int)SystemLanguage.German, "'Erlauben' drücken benutzt Gerätinformationen für relevantere Werbeinhalte");
                settings.LocalizedPopupMessageDictionary.Add((int)SystemLanguage.Catalan, "Prement 'Permetre', s'utilitza la informació del dispositiu per a obtindre contingut publicitari més rellevant");
                settings.LocalizedPopupMessageDictionary.Add((int)SystemLanguage.Spanish, "Presionando 'Permitir', se usa la información del dispositivo para obtener contenido publicitario más relevante");
                settings.LocalizedPopupMessageDictionary.Add((int)SystemLanguage.Chinese, "点击'允许'以使用设备信息获得更加相关的广告内容");
                settings.LocalizedPopupMessageDictionary.Add((int)SystemLanguage.Japanese, "'許可'をクリックすることで、デバイス情報を元により最適な広告を表示することができます");
                settings.LocalizedPopupMessageDictionary.Add((int)SystemLanguage.Korean, "'허용'을 누르면 더 관련성 높은 광고 콘텐츠를 제공하기 위해 기기 정보가 사용됩니다");
            }

            if (settings != null && settings.LocalizedPopupMessageDictionary != null && settings.LocalizedPopupMessageDictionary.ContainsKey((int)SystemLanguage.ChineseSimplified))
            {
                settings.LocalizedPopupMessageDictionary.Remove((int)SystemLanguage.ChineseSimplified);
            }

            if (settings != null && settings.LocalizedPopupMessageDictionary != null && settings.LocalizedPopupMessageDictionary.ContainsKey((int)SystemLanguage.ChineseTraditional))
            {
                settings.LocalizedPopupMessageDictionary.Remove((int)SystemLanguage.ChineseTraditional);
            }

            AssetDatabase.ForceReserializeAssets(new string[] { SETTINGS_ASSET_PATH });
        }

        [MenuItem("Window/Balaso/App Tracking Transparency/Settings", false, 0)]
        static void SelectSettings()
        {
            Selection.activeObject = Settings;
        }

        public override void OnInspectorGUI()
        {
            settings = target as Balaso.Settings;

            CreateLocalizationDictionary();

            FontStyle fontStyle = EditorStyles.label.fontStyle;
            bool wordWrap = GUI.skin.textField.wordWrap;
            EditorStyles.label.fontStyle = FontStyle.Bold;
            GUI.skin.textField.wordWrap = true;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("App Tracking Transparency", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Present the app-tracking authorization request to the end user with this customizable message", EditorStyles.wordWrappedLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // TODO
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseLocalizationValues"), new GUIContent("Use Localization Values", "Toggle ON to use localization values. Otherwise English will default. If you have your own localization solution, be aware as they might conflict"), true);

            // Show all available languages
            languageKeySelectedIndex = EditorGUILayout.Popup("Language", languageKeySelectedIndex, Enum.GetNames(typeof(SystemLanguage)));
            SystemLanguage selectedLanguage = Enum.GetValues(typeof(SystemLanguage)).Cast<SystemLanguage>().ToList()[languageKeySelectedIndex];

            localizedPopupMessageScrollPosition = EditorGUILayout.BeginScrollView(localizedPopupMessageScrollPosition, GUILayout.Height(80));
            
            // Language does not have localized value. Create one and default to EN
            if (!settings.LocalizedPopupMessageDictionary.ContainsKey((int)selectedLanguage))
            {
                settings.LocalizedPopupMessageDictionary.Add((int)selectedLanguage, "Pressing 'Allow' uses device info for more relevant ad content");
            }

            settings.LocalizedPopupMessageDictionary[(int)selectedLanguage] = GUILayout.TextArea(settings.LocalizedPopupMessageDictionary[(int)selectedLanguage], GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save All Languages", GUILayout.Width(300), GUILayout.Height(50)))
            {
                AssetDatabase.ForceReserializeAssets(new string[] { SETTINGS_ASSET_PATH });
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            DrawHorizontalLine(Color.grey);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("SkAdNetwork", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("SkAdNetworkIds specified will be automatically added to your Info.plist file.", EditorStyles.wordWrappedLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("NOTICE: This plugin does not include the ability to show ads.\nYou will need to use your favorite ads platform SDK.", EditorStyles.wordWrappedLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load SkAdNetworkIds from file (xml or json)", GUILayout.Width(300), GUILayout.Height(50)))
            {
                LoadSkAdNetworkIdsFromFile();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("SkAdNetworkIds"), new GUIContent("SkAdNetworkIds"), true);

            serializedObject.ApplyModifiedProperties();
            GUI.skin.textField.wordWrap = wordWrap;
            EditorStyles.label.fontStyle = fontStyle;
        }

        void OnDestroy()
        {
            
        }

        private void LoadSkAdNetworkIdsFromFile()
        {
            SerializedProperty networkIdsSerializedProperty = serializedObject.FindProperty("SkAdNetworkIds");
            string path = EditorUtility.OpenFilePanel("Select SkAdNetworkIds file", "", "txt,json,xml");
            if (path.Length != 0)
            {
                int addedIds = 0;
                string fileContent = File.ReadAllText(path);
                var regex = new Regex(@"[a-z0-9]+\.skadnetwork");
                MatchCollection collection = regex.Matches(fileContent);
                foreach (Match match in collection)
                {
                    string skAdNetworkId = match.Value;
                    bool alreadyAdded = false;
                    int listSize = networkIdsSerializedProperty.arraySize;

                    if (listSize > 0)
                    {
                        for (int i = 0; i < listSize && !alreadyAdded; i++)
                        {
                            if (networkIdsSerializedProperty.GetArrayElementAtIndex(i).stringValue == skAdNetworkId)
                            {
                                alreadyAdded = true;
                            }
                        }
                    }

                    if (!alreadyAdded)
                    {
                        networkIdsSerializedProperty.InsertArrayElementAtIndex(Mathf.Max(0, listSize - 1));
                        networkIdsSerializedProperty.GetArrayElementAtIndex(Mathf.Max(0, listSize - 1)).stringValue = skAdNetworkId;
                        addedIds++;
                    }
                }

                if (addedIds > 0)
                {
                    EditorUtility.DisplayDialog("SkAdNetwork IDs import", string.Format("Successfully added {0} SkAdNetwork IDs", addedIds), "Done");
                }
                else
                {
                    EditorUtility.DisplayDialog("SkAdNetwork IDs import", "No new SkAdNetwork IDs found to be added", "Done");
                }
            }
        }

        private void DrawHorizontalLine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }
}

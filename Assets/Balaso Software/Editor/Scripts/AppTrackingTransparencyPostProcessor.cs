using System.IO;
using UnityEditor.Callbacks;
using UnityEditor;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Balaso
{
    /// <summary>
    /// PostProcessor script to automatically fill all required dependencies
    /// for App Tracking Transparency
    /// </summary>
    public class AppTrackingTransparencyPostProcessor
    {
#if UNITY_IOS
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                /*
                 * PBXProject
                 */
                PBXProject project = new PBXProject();
                string projectPath = PBXProject.GetPBXProjectPath(buildPath);
                project.ReadFromFile(projectPath);



                // If loaded, add `AppTrackingTransparency` Framework
                if (project != null)
                {
                    string targetId = "";
#if UNITY_2019_3_OR_NEWER
                    targetId = project.GetUnityFrameworkTargetGuid();
#else
                    targetId = project.TargetGuidByName("Unity-iPhone");
#endif

                    project.AddFrameworkToProject(targetId, "AppTrackingTransparency.framework", true);
                    project.AddFrameworkToProject(targetId, "AdSupport.framework", false);
                    project.AddFrameworkToProject(targetId, "StoreKit.framework", false);

                    project.WriteToFile(PBXProject.GetPBXProjectPath(buildPath));
                }

                /*
                 * PList
                 */
                PlistDocument plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(buildPath + "/Info.plist"));
                if (plist != null)
                {
                    // Get root
                    PlistElementDict rootDict = plist.root;

                    // Add NSUserTrackingUsageDescription
                    rootDict.SetString("NSUserTrackingUsageDescription", SettingsInspector.Settings.LocalizedPopupMessageDictionary[(int)SystemLanguage.English]);

                    // Check if SKAdNetworkItems already exists
                    PlistElementArray SKAdNetworkItems = null;
                    if (rootDict.values.ContainsKey("SKAdNetworkItems"))
                    {
                        try
                        {
                            SKAdNetworkItems = rootDict.values["SKAdNetworkItems"] as PlistElementArray;
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(string.Format("Could not obtain SKAdNetworkItems PlistElementArray: {0}", e.Message));
                        }
                    }

                    // If not exists, create it
                    if (SKAdNetworkItems == null)
                    {
                        SKAdNetworkItems = rootDict.CreateArray("SKAdNetworkItems");
                    }

                    if (SettingsInspector.Settings.SkAdNetworkIds != null)
                    {
                        List<string> networkIdsWithoutDuplicates = SettingsInspector.Settings.SkAdNetworkIds.Distinct().ToList();
                        string plistContent = File.ReadAllText(buildPath + "/Info.plist");
                        for (int i = 0; i < networkIdsWithoutDuplicates.Count; i++)
                        {
                            if (!plistContent.Contains(networkIdsWithoutDuplicates[i]))
                            {
                                PlistElementDict SKAdNetworkIdentifierDict = SKAdNetworkItems.AddDict();
                                SKAdNetworkIdentifierDict.SetString("SKAdNetworkIdentifier", networkIdsWithoutDuplicates[i]);
                            }
                        }
                    }

                    File.WriteAllText(buildPath + "/Info.plist", plist.WriteToString());
                }

                /*
                 * Localization Popup Message
                 */
                string mainTargetId = "";
#if UNITY_2019_3_OR_NEWER
                mainTargetId = project.GetUnityMainTargetGuid();
#else
                mainTargetId = project.TargetGuidByName("Unity-iPhone");
#endif

                foreach (var localizedMessagePair in SettingsInspector.Settings.LocalizedPopupMessageDictionary)
                {
                    int systemLanguage = localizedMessagePair.Key;
                    string localizedMessageString = localizedMessagePair.Value;

                    AddUserTrackingDescriptionLocalizedString(localizedMessageString, Get2LetterISOCodeFromSystemLanguage((SystemLanguage)systemLanguage), buildPath, project, mainTargetId);
                }

                project.WriteToFile(PBXProject.GetPBXProjectPath(buildPath));
            }
        }

        private static void AddUserTrackingDescriptionLocalizedString(string localizedUserTrackingDescription, string localeCode, string buildPath, PBXProject project, string targetGuid)
        {
            const string resourcesDirectoryName = "BalasoLocalizationResources";
            var resourcesDirectoryPath = Path.Combine(buildPath, resourcesDirectoryName);
            var localeSpecificDirectoryName = localeCode + ".lproj";
            var localeSpecificDirectoryPath = Path.Combine(resourcesDirectoryPath, localeSpecificDirectoryName);
            var infoPlistStringsFilePath = Path.Combine(localeSpecificDirectoryPath, "InfoPlist.strings");

            if (!SettingsInspector.Settings.UseLocalizationValues)
            {
                if (!File.Exists(infoPlistStringsFilePath)) return;

                File.Delete(infoPlistStringsFilePath);
                return;
            }

            // Create intermediate directories as needed.
            if (!Directory.Exists(resourcesDirectoryPath))
            {
                Directory.CreateDirectory(resourcesDirectoryPath);
            }

            if (!Directory.Exists(localeSpecificDirectoryPath))
            {
                Directory.CreateDirectory(localeSpecificDirectoryPath);
            }

            var localizedDescriptionLine = "\"NSUserTrackingUsageDescription\" = \"" + localizedUserTrackingDescription + "\";\n";
            // File already exists, update it in case the value changed between builds.
            if (File.Exists(infoPlistStringsFilePath))
            {
                var output = new List<string>();
                var lines = File.ReadAllLines(infoPlistStringsFilePath);
                var keyUpdated = false;
                foreach (var line in lines)
                {
                    if (line.Contains("NSUserTrackingUsageDescription"))
                    {
                        output.Add(localizedDescriptionLine);
                        keyUpdated = true;
                    }
                    else
                    {
                        output.Add(line);
                    }
                }

                if (!keyUpdated)
                {
                    output.Add(localizedDescriptionLine);
                }

                File.WriteAllText(infoPlistStringsFilePath, string.Join("\n", output.ToArray()) + "\n");
            }
            // File doesn't exist, create one.
            else
            {
                File.WriteAllText(infoPlistStringsFilePath, "/* Localized versions of Info.plist keys */\n" + localizedDescriptionLine);
            }

            var guid = project.AddFolderReference(localeSpecificDirectoryPath, Path.Combine(resourcesDirectoryName, localeSpecificDirectoryName), PBXSourceTree.Source);
            project.AddFileToBuild(targetGuid, guid);
        }

        private static string Get2LetterISOCodeFromSystemLanguage(SystemLanguage lang)
        {
            string res = "EN";
            switch (lang)
            {
                case SystemLanguage.Afrikaans: res = "AF"; break;
                case SystemLanguage.Arabic: res = "AR"; break;
                case SystemLanguage.Basque: res = "EU"; break;
                case SystemLanguage.Belarusian: res = "BY"; break;
                case SystemLanguage.Bulgarian: res = "BG"; break;
                case SystemLanguage.Catalan: res = "CA"; break;
                case SystemLanguage.Chinese: res = "ZH"; break;
                case SystemLanguage.Czech: res = "CS"; break;
                case SystemLanguage.Danish: res = "DA"; break;
                case SystemLanguage.Dutch: res = "NL"; break;
                case SystemLanguage.English: res = "EN"; break;
                case SystemLanguage.Estonian: res = "ET"; break;
                case SystemLanguage.Faroese: res = "FO"; break;
                case SystemLanguage.Finnish: res = "FI"; break;
                case SystemLanguage.French: res = "FR"; break;
                case SystemLanguage.German: res = "DE"; break;
                case SystemLanguage.Greek: res = "EL"; break;
                case SystemLanguage.Hebrew: res = "IW"; break;
                case SystemLanguage.Hungarian: res = "HU"; break;
                case SystemLanguage.Icelandic: res = "IS"; break;
                case SystemLanguage.Indonesian: res = "IN"; break;
                case SystemLanguage.Italian: res = "IT"; break;
                case SystemLanguage.Japanese: res = "JA"; break;
                case SystemLanguage.Korean: res = "KO"; break;
                case SystemLanguage.Latvian: res = "LV"; break;
                case SystemLanguage.Lithuanian: res = "LT"; break;
                case SystemLanguage.Norwegian: res = "NO"; break;
                case SystemLanguage.Polish: res = "PL"; break;
                case SystemLanguage.Portuguese: res = "PT"; break;
                case SystemLanguage.Romanian: res = "RO"; break;
                case SystemLanguage.Russian: res = "RU"; break;
                case SystemLanguage.SerboCroatian: res = "SH"; break;
                case SystemLanguage.Slovak: res = "SK"; break;
                case SystemLanguage.Slovenian: res = "SL"; break;
                case SystemLanguage.Spanish: res = "ES"; break;
                case SystemLanguage.Swedish: res = "SV"; break;
                case SystemLanguage.Thai: res = "TH"; break;
                case SystemLanguage.Turkish: res = "TR"; break;
                case SystemLanguage.Ukrainian: res = "UK"; break;
                case SystemLanguage.Unknown: res = "EN"; break;
                case SystemLanguage.Vietnamese: res = "VI"; break;
            }
            return res.ToLower();
        }
#endif
    }
}
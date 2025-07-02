using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class OnPostBuildProcess : MonoBehaviour
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            processForiOS(path);
        }
    }
    static void processForiOS(string path)
    {
        string pbxPath = PBXProject.GetPBXProjectPath(path);
        PBXProject pbx = new PBXProject();
        pbx.ReadFromString(File.ReadAllText(pbxPath));
        string target = pbx.GetUnityMainTargetGuid();
        pbx.SetBuildProperty(target, "ENABLE_BITCODE", "NO ");
        target = pbx.GetUnityFrameworkTargetGuid();
        pbx.SetBuildProperty(target, "ENABLE_BITCODE", "NO ");
        File.WriteAllText(pbxPath, pbx.WriteToString());
    }
}
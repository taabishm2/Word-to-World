using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class AssetBundleCreator
{
    [MenuItem("Tools/Build All FBX To AssetBundle")]
    public static void BuildFBXAssetBundleMenuItem()
    {
        BuildFBXAssetBundle();
    }

    private static void BuildFBXAssetBundle()
    {
        // Hardcoded folder path
        string folderPath = "Assets/Models";

        // Get all .fbx assets in the folder
        string[] fbxFiles = Directory.GetFiles(folderPath, "*.fbx", SearchOption.AllDirectories);

        // Create a list to store asset paths
        List<string> assetPaths = new List<string>();

        // Convert asset paths to relative paths
        foreach (string fbxFile in fbxFiles)
        {
            string assetPath = fbxFile.Replace(Application.dataPath, "Assets");
            assetPaths.Add(assetPath);
        }

        // Create an AssetBundleBuild array with a single entry
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
        buildMap[0].assetBundleName = "bundle_android";
        buildMap[0].assetNames = assetPaths.ToArray();

        // Build the AssetBundle
        string outputPath = "Assets/Builds";
        BuildPipeline.BuildAssetBundles(outputPath, buildMap, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

        Debug.Log("FBX AssetBundle built at: " + outputPath);
    }
}
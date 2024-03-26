using UnityEngine;
using UnityEditor;
using System.IO;

public class BundleBuilder
{
    [MenuItem("W2W/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/BuiltAssetBundles";
        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(assetBundleDirectory);

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }

}
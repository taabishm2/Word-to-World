using UnityEngine;
using UnityEditor;
using System.IO;

public class BundleBuilder
{
    [MenuItem("Tools/Build Manual AssetBundle")]
    static void Build()
    {
        string assetBundleDirectory = "Assets/Build AssetBundles";
        if (!System.IO.Directory.Exists(assetBundleDirectory))
        {
            System.IO.Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, 
            BuildAssetBundleOptions.None, 
            EditorUserBuildSettings.activeBuildTarget);
    }

}



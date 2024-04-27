using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class BoundingBox
{
    private static string assetsDirectory = "Assets/Scenes/"; // Path to the directory with FBX files
    private static string outputPath = "Assets/FBXDimensions.csv"; // Path to save the CSV file

    [MenuItem("Tools/Calculate All FBX Bounds")]
    private static void CalculateAllFBXBounds()
    {
        // Get all FBX files in the specified directory
        string[] fbxFiles = Directory.GetFiles(assetsDirectory, "*.fbx", SearchOption.AllDirectories);

        // Prepare a string builder for CSV content
        StringBuilder csvContent = new StringBuilder("FBX Name,Width,Height,Depth\n");

        foreach (string filePath in fbxFiles)
        {
            GameObject assetObject = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
            if (assetObject != null)
            {
                // Instantiate the GameObject to access its components
                GameObject tempInstance = GameObject.Instantiate(assetObject);
                tempInstance.hideFlags = HideFlags.HideAndDontSave;

                try
                {
                    Bounds totalBounds = new Bounds();
                    MeshFilter[] meshFilters = tempInstance.GetComponentsInChildren<MeshFilter>();
                    if (meshFilters.Length > 0)
                    {
                        bool hasBounds = false;
                        foreach (MeshFilter filter in meshFilters)
                        {
                            if (filter.sharedMesh != null)
                            {
                                if (!hasBounds)
                                {
                                    totalBounds = filter.sharedMesh.bounds;
                                    hasBounds = true;
                                }
                                else
                                {
                                    totalBounds.Encapsulate(filter.sharedMesh.bounds);
                                }
                            }
                        }

                        if (hasBounds)
                        {
                            // Append to CSV
                            csvContent.AppendLine($"{Path.GetFileNameWithoutExtension(filePath)},{totalBounds.size.x},{totalBounds.size.y},{totalBounds.size.z}");
                        }
                    }
                }
                finally
                {
                    // Clean up the temporary instance
                    GameObject.DestroyImmediate(tempInstance);
                }
            }
        }

        // Write to CSV file
        File.WriteAllText(outputPath, csvContent.ToString());
        Debug.Log("FBX dimensions saved to " + outputPath);
    }
}

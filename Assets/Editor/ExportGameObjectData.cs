using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;
using System.Text;

public class ExportGameObjectData : MonoBehaviour
{
    [MenuItem("Tools/Export GameObject Data to CSV")]
    public static void ExportCSV()
    {
        // Create a new StringBuilder for CSV content
        StringBuilder csvContent = new StringBuilder();
        string csvHeader = "Name,Rotation X,Rotation Y,Rotation Z,Scale X,Scale Y,Scale Z";
        csvContent.AppendLine(csvHeader);

        // Fetch all GameObjects in the scene
        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
        {
            // Get object's rotation and scale
            Vector3 rotation = obj.transform.rotation.eulerAngles;
            Vector3 scale = obj.transform.localScale;

            // Create a CSV formatted line
            string line = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                obj.name,
                rotation.x, rotation.y, rotation.z,
                scale.x, scale.y, scale.z);

            // Append this line to the CSV content
            csvContent.AppendLine(line);
        }

        // Path where the CSV file will be saved (in the project's Assets folder)
        string filePath = Path.Combine(Application.dataPath, "asset_list.csv");

        // Write the CSV content to a file
        File.WriteAllText(filePath, csvContent.ToString());

        // Refresh the AssetDatabase to show the new file in Unity Editor
        AssetDatabase.Refresh();

        Debug.Log("GameObject data exported to CSV file at: " + filePath);
    }
}

